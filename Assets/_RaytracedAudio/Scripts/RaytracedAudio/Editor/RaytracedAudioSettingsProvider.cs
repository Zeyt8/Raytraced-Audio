using UnityEditor;

namespace RaytracedAudio.Editor
{
    public static class RaytracedAudioSettingsProvider
    {
        private const string _preferencesPath = "Project/Raytraced Audio";

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider(_preferencesPath, SettingsScope.Project)
            {
                label = "Raytraced Audio",
                guiHandler = (searchContext) =>
                {
                    SerializedObject settings = RaytracedAudioSettings.GetSerializedSettings();
                    settings.Update();
                    SerializedProperty property = settings.GetIterator();
                    property.NextVisible(true);
                    while (property.NextVisible(false))
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                    settings.ApplyModifiedProperties();
                },
            };
        }
    }
}
