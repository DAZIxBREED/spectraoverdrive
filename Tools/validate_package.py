#!/usr/bin/env python3
"""Static package audit for SpectraOverdrive source releases."""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
EXPECTED_VERSION = "1.5.2"


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


def stripped_code(source: str) -> str:
    output: list[str] = []
    index = 0
    state = "code"
    while index < len(source):
        current = source[index]
        following = source[index + 1] if index + 1 < len(source) else ""
        if state == "code":
            if current == "/" and following == "/":
                output.extend("  ")
                state = "line"
                index += 2
                continue
            if current == "/" and following == "*":
                output.extend("  ")
                state = "block"
                index += 2
                continue
            if current == '"':
                output.append(" ")
                state = "string"
                index += 1
                continue
            if current == "'":
                output.append(" ")
                state = "char"
                index += 1
                continue
            output.append(current)
            index += 1
            continue
        if state == "line":
            output.append("\n" if current == "\n" else " ")
            if current == "\n":
                state = "code"
            index += 1
            continue
        if state == "block":
            if current == "*" and following == "/":
                output.extend("  ")
                state = "code"
                index += 2
            else:
                output.append("\n" if current == "\n" else " ")
                index += 1
            continue
        quote = '"' if state == "string" else "'"
        if current == "\\":
            output.extend("  ")
            index += 2
        elif current == quote:
            output.append(" ")
            state = "code"
            index += 1
        else:
            output.append("\n" if current == "\n" else " ")
            index += 1
    if state in {"string", "char", "block"}:
        fail(f"unterminated {state} in C# source")
    return "".join(output)


def validate_delimiters(path: Path) -> None:
    source = stripped_code(path.read_text(encoding="utf-8"))
    pairs = {")": "(", "]": "[", "}": "{"}
    stack: list[tuple[str, int]] = []
    for index, character in enumerate(source):
        if character in "([{":
            stack.append((character, index))
        elif character in pairs:
            if not stack or stack[-1][0] != pairs[character]:
                fail(f"{path.relative_to(ROOT)} has unmatched {character} at {index}")
            stack.pop()
    if stack:
        fail(f"{path.relative_to(ROOT)} has unclosed {stack[-1][0]}")


def public_arrays(source: str) -> set[str]:
    pattern = re.compile(
        r"public\s+(?:string|int|float|bool|Color|Vector4)\[\]\s+(\w+)\s*="
    )
    return set(pattern.findall(source))


def validate_json_files() -> None:
    for path in sorted(list(ROOT.rglob("*.json")) + list(ROOT.rglob("*.asmdef"))):
        try:
            json.loads(path.read_text(encoding="utf-8"))
        except (OSError, UnicodeDecodeError, json.JSONDecodeError) as error:
            fail(f"invalid JSON in {path.relative_to(ROOT)}: {error}")


def validate_unique_types(sources: list[Path]) -> int:
    declared: dict[str, list[str]] = {}
    pattern = re.compile(r"\b(?:class|struct|enum|interface)\s+(\w+)")
    for path in sources:
        for name in pattern.findall(stripped_code(path.read_text(encoding="utf-8"))):
            declared.setdefault(name, []).append(str(path.relative_to(ROOT)))
    duplicates = {name: paths for name, paths in declared.items() if len(paths) > 1}
    if duplicates:
        detail = "; ".join(
            f"{name}: {', '.join(paths)}" for name, paths in sorted(duplicates.items())
        )
        fail("duplicate declared C# types: " + detail)
    return len(declared)


def validate_local_shader_includes() -> int:
    shader_files = sorted(
        list(ROOT.rglob("*.shader"))
        + list(ROOT.rglob("*.hlsl"))
        + list(ROOT.rglob("*.cginc"))
    )
    include_pattern = re.compile(r'#include\s+["<]([^">]+)[">]')
    built_in_prefixes = (
        "Unity", "Packages/", "HLSLSupport", "AutoLight", "Lighting",
        "UnityCG", "VRChat",
    )
    for path in shader_files:
        for include in include_pattern.findall(path.read_text(encoding="utf-8")):
            if include.startswith(built_in_prefixes):
                continue
            candidates = (
                path.parent / include,
                ROOT / include,
                ROOT / "Runtime/Shaders" / include,
            )
            if not any(candidate.exists() for candidate in candidates):
                fail(
                    f"unresolved local shader include {include} from "
                    f"{path.relative_to(ROOT)}"
                )
    return len(shader_files)


def main() -> None:
    validate_json_files()
    package = json.loads((ROOT / "package.json").read_text(encoding="utf-8"))
    if package.get("version") != EXPECTED_VERSION:
        fail(f"package version is {package.get('version')}, expected {EXPECTED_VERSION}")
    readme = (ROOT / "README.md").read_text(encoding="utf-8")
    if not readme.startswith(f"# SpectraOverdrive {EXPECTED_VERSION}\n"):
        fail("README release heading does not match package version")
    show_json = (ROOT / "Editor/ImportExport/SpectraShowJson.cs").read_text(encoding="utf-8")
    if f'createdWith = "{EXPECTED_VERSION}"' not in show_json:
        fail("portable-show createdWith version does not match package version")
    release_validator = (ROOT / "Editor/Validation/SpectraReleaseReadinessValidator.cs").read_text(encoding="utf-8")
    if f'generatorVersion = "{EXPECTED_VERSION}"' not in release_validator:
        fail("release-readiness generator version does not match package version")

    sources = sorted(ROOT.rglob("*.cs"))
    if not sources:
        fail("no C# sources found")
    for source in sources:
        validate_delimiters(source)
    declared_type_count = validate_unique_types(sources)
    shader_file_count = validate_local_shader_includes()

    compiled_source = (ROOT / "Runtime/Shows/SpectraCompiledShow.cs").read_text(
        encoding="utf-8"
    )
    runtime_source = (ROOT / "Runtime/Playback/SpectraShowRuntimePlayer.cs").read_text(
        encoding="utf-8"
    )
    compiler_source = (ROOT / "Editor/ShowProgrammer/SpectraShowCompiler.cs").read_text(
        encoding="utf-8"
    )
    compiled_arrays = public_arrays(compiled_source)
    runtime_arrays = public_arrays(runtime_source)
    missing_runtime = sorted(compiled_arrays - runtime_arrays)
    if missing_runtime:
        fail("compiled arrays missing from runtime player: " + ", ".join(missing_runtime))
    missing_bakes = sorted(
        name
        for name in compiled_arrays
        if f"player.{name} = source.{name};" not in compiler_source
    )
    if missing_bakes:
        fail("compiled arrays missing from runtime bake: " + ", ".join(missing_bakes))
    missing_initializers = sorted(
        name for name in compiled_arrays if f"result.{name} =" not in compiler_source
    )
    if missing_initializers:
        fail("compiled arrays missing compiler initialization: " + ", ".join(missing_initializers))

    forbidden = re.compile(r"\bNotImplementedException\b|\bTODO\b|\bFIXME\b|\bHACK\b")
    executable_hits: list[str] = []
    for source in sources:
        clean = stripped_code(source.read_text(encoding="utf-8"))
        if forbidden.search(clean):
            executable_hits.append(str(source.relative_to(ROOT)))
    if executable_hits:
        fail("stub markers found in executable C#: " + ", ".join(executable_hits))

    print(
        f"SpectraOverdrive {EXPECTED_VERSION} static audit passed: "
        f"{len(sources)} C# files, {declared_type_count} declared types, "
        f"{len(compiled_arrays)} compiled arrays, and {shader_file_count} shader files."
    )


if __name__ == "__main__":
    main()
