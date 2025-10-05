using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace RaytracedAudio
{
    public class AudioManager : MonoBehaviour
    {
        private List<AudioSource> _audioSources = new List<AudioSource>();
        private NativeList<float3> _audioPositions = new NativeList<float3>(Allocator.Persistent);

        public NativeList<float3> AudioPositions => _audioPositions;

        private void Awake()
        {
            AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            _audioSources.AddRange(sources);
            _audioPositions.Resize(_audioSources.Count, NativeArrayOptions.UninitializedMemory);
        }

        private void Update()
        {
            ComputeAudioPositions();
        }

        public void SetValues(NativeList<float> muffleValues)
        {
            for (int i = 0; i < _audioSources.Count; i++)
            {
                float value = MathUtils.MuffleCutoff((float)muffleValues[i] / (RaytracedAudioSettings.RaysPerSource * RaytracedAudioSettings.MaxRayBounces));
                _audioSources[i].GetComponent<AudioLowPassFilter>().cutoffFrequency = value;
            }
        }

        private void ComputeAudioPositions()
        {
            _audioPositions.Capacity = _audioSources.Count;
            for(int i = 0; i < _audioSources.Count; i++)
            {
                _audioPositions[i] = _audioSources[i].transform.position;
            }
        }
    }
}
