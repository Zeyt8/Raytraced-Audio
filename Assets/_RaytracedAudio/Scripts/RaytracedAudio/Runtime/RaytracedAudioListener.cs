using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace RaytracedAudio
{
    public class RaytracedAudioListener : MonoBehaviour
    {
        [SerializeField] private AudioSource _source; //temporary

        private void Update()
        {
            Vector3 listenerPosition = transform.position;
            // TODO: maybe cache settings?
            float muffle = PerformRaySearch(listenerPosition, RaytracedAudioSettings.RaysPerSource, RaytracedAudioSettings.MaxRayBounces);
            _source.GetComponent<AudioLowPassFilter>().cutoffFrequency = Mathf.Lerp(10, 22000, 0.01f / Mathf.Log(muffle + 1));
        }

        private float PerformRaySearch(Vector3 origin, int rayCount, int maxRayBounce)
        {
            int totalHits = 0;

            for (int i = 0; i < rayCount; i++)
            {
                MathUtils.FibonacciSphereDirection(i, rayCount, out float3 dir);
                int hits = TraceRay(origin, dir, maxRayBounce);
                totalHits += hits;
            }
            Debug.Log(totalHits);
            return 1 - (float)totalHits / (rayCount);
        }

        private int TraceRay(float3 origin, float3 direction, int maxRayBounce)
        {
            float3 currentOrigin = origin;
            float3 currentDirection = direction;
            int totalHits = 0;

            for (uint bounce = 0; bounce < maxRayBounce; bounce++)
            {
                Ray ray = new Ray(currentOrigin, currentDirection);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Debug.DrawLine(currentOrigin, hitInfo.point, Color.white, 1f);
                    currentOrigin = hitInfo.point;
                    currentDirection = Vector3.Reflect(currentDirection, hitInfo.normal);
                    if (!Physics.Linecast(currentOrigin + (float3)hitInfo.normal * 0.01f, _source.transform.position, RaytracedAudioSettings.RaycastLayerMask))
                    {
                        Debug.DrawLine(currentOrigin, _source.transform.position, Color.green, 1f);
                        totalHits++;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return totalHits;
        }
    }
}
