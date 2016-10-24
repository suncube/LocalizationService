using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SunCubeStudio.Localization
{
    [CustomEditor(typeof(UILocalization))]
    public class UILocalizationEditor : Editor
    {
        private readonly int lineHeight = 20;
        private bool lanquageFoldot =false;
        private int intPopup = 0;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var myTarget = (UILocalization) target;

            if (!myTarget.IsHasOutputHelper())
            {
                EditorGUILayout.HelpBox("[UI Text] or [Text Mesh] script were not added to GameObject ", MessageType.Error);
                return;
            }

            ShowAvailableKeyValues(myTarget);
            ShowLocalizeValues(myTarget);
        }
        private void ShowAvailableKeyValues(UILocalization myTarget)
        {
            var localizationKeys = LocalizationService.GetLocalizationKeys();

            var keyId = GetIdByKey(myTarget.Key, localizationKeys);
            if (keyId == -1)
            {
                keyId = 0;
                EditorGUILayout.HelpBox("KEY not found in localization file. ", MessageType.Error);
            }

            intPopup = keyId;

            var listId = new int[localizationKeys.Length];
            for (var i = 0; i < localizationKeys.Length; i++)
                listId[i] = i;
            intPopup = EditorGUILayout.IntPopup("List of Keys", intPopup, localizationKeys, listId);

            if (keyId != intPopup || string.IsNullOrEmpty(myTarget.Key))
                myTarget.Key = localizationKeys[intPopup];
        }
        private void ShowLocalizeValues(UILocalization myTarget)
        {
            EditorGUILayout.Space();
            lanquageFoldot = EditorGUILayout.Foldout(lanquageFoldot, "View Localization Values ");
            var dictionary = LocalizationService.GetLocalizationsByKey(myTarget.Key);
            if (dictionary != null)
            {
                foreach (var loc in dictionary)
                {
                    if (!lanquageFoldot) continue;
                    if (string.IsNullOrEmpty(loc.Value))
                        EditorGUILayout.TextField(loc.Key,"NOT FOUND");
                    else
                        EditorGUILayout.TextField(loc.Key, loc.Value);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("CSV Localization File not set. Check path to file " + LocalizationService.LocalizationFilePath+"", MessageType.Error);
            }
        }
        private int GetIdByKey(string key,string[] keys)
        {
            for (int index = 0; index < keys.Length; index++)
            {
                if (keys[index] == key)
                    return index;
            }
            return -1;
        }
    }

}