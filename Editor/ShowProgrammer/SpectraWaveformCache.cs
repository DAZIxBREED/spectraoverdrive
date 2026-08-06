using System;
using UnityEngine;

namespace SpectraOverdrive.Editor
{
    [Serializable]
    public sealed class SpectraWaveformData
    {
        public int sampleRate;
        public int channels;
        public int sourceSamples;
        public float duration;
        public float[] minimum;
        public float[] maximum;
        public string error;

        public int BucketCount { get { return minimum == null ? 0 : minimum.Length; } }

        public void Draw(Rect rect, float visibleStart, float visibleEnd, Color color)
        {
            if (BucketCount == 0 || rect.width <= 1f || visibleEnd <= visibleStart) return;
            float center = rect.y + rect.height * 0.5f;
            float halfHeight = rect.height * 0.46f;
            Color previous = GUI.color;
            GUI.color = color;
            int pixels = Mathf.Max(1, Mathf.FloorToInt(rect.width));
            for (int x = 0; x < pixels; x++)
            {
                float time = Mathf.Lerp(visibleStart, visibleEnd, x / (float)pixels);
                int bucket = Mathf.Clamp(Mathf.FloorToInt(time / Mathf.Max(0.0001f, duration) * BucketCount), 0, BucketCount - 1);
                float high = maximum[bucket];
                float low = minimum[bucket];
                Rect line = new Rect(rect.x + x, center - high * halfHeight, 1f, Mathf.Max(1f, (high - low) * halfHeight));
                GUI.DrawTexture(line, Texture2D.whiteTexture);
            }
            GUI.color = previous;
        }
    }

    public static class SpectraWaveformCache
    {
        public static SpectraWaveformData Build(AudioClip clip, int maximumBuckets)
        {
            SpectraWaveformData data = new SpectraWaveformData();
            if (clip == null)
            {
                data.error = "No authoring audio clip is assigned.";
                data.minimum = new float[0];
                data.maximum = new float[0];
                return data;
            }

            data.sampleRate = clip.frequency;
            data.channels = clip.channels;
            data.sourceSamples = clip.samples;
            data.duration = clip.length;
            int buckets = Mathf.Clamp(maximumBuckets, 64, 65536);
            buckets = Mathf.Min(buckets, Mathf.Max(1, clip.samples));
            data.minimum = new float[buckets];
            data.maximum = new float[buckets];
            for (int i = 0; i < buckets; i++)
            {
                data.minimum[i] = 1f;
                data.maximum[i] = -1f;
            }

            int chunkFrames = Mathf.Min(262144, Mathf.Max(1024, clip.samples));
            float[] buffer = new float[chunkFrames * Mathf.Max(1, clip.channels)];
            int frameOffset = 0;
            try
            {
                while (frameOffset < clip.samples)
                {
                    int frames = Mathf.Min(chunkFrames, clip.samples - frameOffset);
                    if (buffer.Length != frames * clip.channels) buffer = new float[frames * clip.channels];
                    if (!clip.GetData(buffer, frameOffset))
                        throw new InvalidOperationException("AudioClip.GetData returned false. Use Decompress On Load for waveform authoring.");
                    for (int frame = 0; frame < frames; frame++)
                    {
                        float mixed = 0f;
                        for (int channel = 0; channel < clip.channels; channel++)
                            mixed += buffer[frame * clip.channels + channel];
                        mixed /= Mathf.Max(1, clip.channels);
                        int sourceFrame = frameOffset + frame;
                        int bucket = Mathf.Min(buckets - 1, (int)((long)sourceFrame * buckets / Mathf.Max(1, clip.samples)));
                        data.minimum[bucket] = Mathf.Min(data.minimum[bucket], mixed);
                        data.maximum[bucket] = Mathf.Max(data.maximum[bucket], mixed);
                    }
                    frameOffset += frames;
                }
                for (int i = 0; i < buckets; i++)
                {
                    if (data.minimum[i] > data.maximum[i])
                    {
                        data.minimum[i] = 0f;
                        data.maximum[i] = 0f;
                    }
                }
            }
            catch (Exception exception)
            {
                data.error = exception.Message;
                data.minimum = new float[0];
                data.maximum = new float[0];
            }
            return data;
        }
    }
}
