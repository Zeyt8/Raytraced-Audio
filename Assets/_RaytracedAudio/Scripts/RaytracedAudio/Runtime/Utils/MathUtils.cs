using Unity.Mathematics;
using Unity.Burst;

namespace RaytracedAudio
{
    [BurstCompile]
    public static class MathUtils
    {
        [BurstCompile]
        public static void FibonacciSphereDirection(long i, long N, out float3 point)
        {
            float offset = 2f / N;
            float increment = math.PI * (3f - math.sqrt(5f));

            float y = ((i * offset) - 1f) + (offset / 2f);
            float r = math.sqrt(1f - y * y);
            float phi = i * increment;

            float x = math.cos(phi) * r;
            float z = math.sin(phi) * r;

            point =  math.normalize(new float3(x, y, z));
        }

        [BurstCompile]
        public static float MuffleCutoff(float value)
        {
            return math.lerp(10, 22000, 0.01f / math.log(2 - value));
        }

        [BurstCompile]
        public static float EchoLerp(float min, float max, float value)
        {
            return math.remap(min, max, 0.1f, 2, value);
        }
    }
}
