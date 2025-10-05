using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace RaytracedAudio
{
    public class RaytracedAudioListener : MonoBehaviour
    {
        [SerializeField] private AudioManager _audioManager; //temp

        private NativeList<float> _muffleValues = new NativeList<float>(Allocator.Persistent);
        private NativeList<float> _echoValues = new NativeList<float>(Allocator.Persistent);

        private AudioReverbFilter _reverbFilter;

        private void Awake()
        {
            _reverbFilter = GetComponent<AudioReverbFilter>();
            _echoValues.Resize(RaytracedAudioSettings.RaysPerSource, NativeArrayOptions.UninitializedMemory);
        }

        private void Update()
        {
            float3 listenerPosition = transform.position;
            NativeList<float3> audioPositions = _audioManager.AudioPositions;
            // TODO: maybe cache settings?
            PerformRaySearch(listenerPosition, audioPositions, RaytracedAudioSettings.RaysPerSource, RaytracedAudioSettings.MaxRayBounces);
            float echoValue = 0;
            for (int i = 0; i < _echoValues.Length; i++)
            {
                echoValue += _echoValues[i];
                _echoValues[i] = 0;
            }
            echoValue /= _echoValues.Length;
            _audioManager.SetValues(_muffleValues);
            if (echoValue < RaytracedAudioSettings.SmallRoomSize)
            {
                _reverbFilter.enabled = false;
            }
            else
            {
                _reverbFilter.enabled = true;
                _reverbFilter.decayTime = MathUtils.EchoLerp(RaytracedAudioSettings.SmallRoomSize, 20, echoValue);
            }
        }

        private void PerformRaySearch(float3 origin, NativeList<float3> audioPositions, int rayCount, int maxRayBounce)
        {
            _muffleValues.Resize(audioPositions.Length, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < _muffleValues.Length; i++)
            {
                _muffleValues[i] = 0;
            }
            for (int i = 0; i < rayCount; i++)
            {
                MathUtils.FibonacciSphereDirection(i, rayCount, out float3 dir);
                TraceRay(i, origin, dir, audioPositions, maxRayBounce);
            }
        }

        private void TraceRay(int index, float3 origin, float3 direction, NativeList<float3> audioPositions, int maxRayBounce)
        {
            float3 currentOrigin = origin;
            float3 currentDirection = direction;

            float averageDistance = 0;
            int validDistances = 0;

            for (uint bounce = 0; bounce < maxRayBounce; bounce++)
            {
                Ray ray = new Ray(currentOrigin, currentDirection);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    currentOrigin = hitInfo.point;
                    currentDirection = Vector3.Reflect(currentDirection, hitInfo.normal);
                    for (int i = 0; i < audioPositions.Length; i++)
                    {
                        if (!Physics.Linecast(currentOrigin + (float3)hitInfo.normal * 0.01f, audioPositions[i], RaytracedAudioSettings.RaycastLayerMask))
                        {
                            _muffleValues[i] += 1;
                        }
                    }
                    if (!Physics.Linecast(currentOrigin + (float3)hitInfo.normal * 0.01f, origin, RaytracedAudioSettings.RaycastLayerMask))
                    {
                        averageDistance += math.distance(currentOrigin, origin);
                        validDistances++;
                    }
                }
                else
                {
                    break;
                }
            }

            _echoValues[index] = averageDistance / validDistances;
        }
    }
}
