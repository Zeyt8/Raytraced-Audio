using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace RaytracedAudio
{
    public class RaytracedAudioSettings : ScriptableObject
    {
        private const string _levelSettingsPath = "Assets/Resources/RaytracedAudio/Settings.asset";

        private static RaytracedAudioSettings _instance = null;
        public static RaytracedAudioSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<RaytracedAudioSettings>("RaytracedAudio/Settings");
                }
#if UNITY_EDITOR
                if (_instance == null)
                {
                    _instance = ScriptableObject.CreateInstance<RaytracedAudioSettings>();
                    Directory.CreateDirectory("Assets/Resources/RaytracedAudio/");
                    AssetDatabase.CreateAsset(_instance, _levelSettingsPath);
                    AssetDatabase.SaveAssets();
                }
#endif
                return _instance;
            }
        }

        public static int MaxRayBounces => Instance._maxRayBounces;
        public static int RaysPerSource => Instance._raysPerSource;
        public static LayerMask RaycastLayerMask => Instance._raycastLayerMask;

        [SerializeField, Min(0)] private int _maxRayBounces = 5;
        [SerializeField, Min(0)] private int _raysPerSource = 10;
        [SerializeField] private LayerMask _raycastLayerMask = ~0;

#if UNITY_EDITOR
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(Instance);
        }
#endif
    }
}
