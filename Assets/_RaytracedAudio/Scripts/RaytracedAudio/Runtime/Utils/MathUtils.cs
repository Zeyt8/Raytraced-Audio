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
    }
}
