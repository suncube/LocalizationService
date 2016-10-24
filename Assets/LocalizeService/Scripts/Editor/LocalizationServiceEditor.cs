using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace SunCubeStudio.Localization
{
    [CustomEditor(typeof (LocalizationService))]
    public class LocalizationServiceEditor : Editor
    {
        private readonly int lineHeight = 20;
        private Dictionary<string, bool> lanquageFoldot = new Dictionary<string, bool>();
        private Dictionary<string, Vector2> lanquageScroll = new Dictionary<string, Vector2>();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            LocalizationService myTarget = (LocalizationService)target;
            EditorGUILayout.LabelField("Current Localization ", myTarget.Localization);
            EditorGUILayout.LabelField("Localization File Path", LocalizationService.LocalizationFilePath);
            EditorGUILayout.Space();

            var localize = myTarget.LoadLocalizeFileHelper();
            if (localize != null)
            {
               
                foreach (var loc in localize)
                {
                    if (!lanquageFoldot.ContainsKey(loc.Key))
                        lanquageFoldot.Add(loc.Key,false);

                    bool isShow;
                    if(lanquageFoldot.TryGetValue(loc.Key,out isShow))
                        lanquageFoldot[loc.Key] = EditorGUILayout.Foldout(isShow, loc.Key);

                    if (!isShow) continue;

                    if (!lanquageScroll.ContainsKey(loc.Key))
                        lanquageScroll.Add(loc.Key, new Vector2());

                    lanquageScroll[loc.Key] = EditorGUILayout.BeginScrollView(lanquageScroll[loc.Key]);
                    var values = GetTextOutputOfDictionary(loc.Value);
                    EditorGUILayout.TextArea(values, GUILayout.Height(loc.Value.Count * lineHeight));
                    EditorGUILayout.EndScrollView();
                }

            }
            else
            {
                EditorGUILayout.HelpBox("CSV Localization File not set. Check path to file! " + LocalizationService.LocalizationFilePath, MessageType.Error);
            }
        }

        public string GetTextOutputOfDictionary(Dictionary<string, string> dict)
        {
            var textOutput = string.Format("size = {0} \n",dict.Count);
            return dict.Aggregate(textOutput, (current, item) => current + string.Format("[{0}] = {1} \n", item.Key, item.Value));
        }
    }
}