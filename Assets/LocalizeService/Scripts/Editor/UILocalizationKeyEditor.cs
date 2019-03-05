using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(UILocalizationKeys))]
    [CanEditMultipleObjects]
    public class UILocalizationKeyEditor : UILocalizationEditor
    {
        protected override void Initialize()
        {
            base.Initialize();

        }
        void ReadOnlyTextField(string label, string text)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
        }

        Vector2 scroll;
        public override void OnInspectorGUI()
        {
            var myTarget = (UILocalizationKeys)target;
            if (!AddingTypeTest(myTarget))
                return;

            if (localizationKeys == null)
                localizationKeys = LocalizationService.GetLocalizationKeys();

            GUILayout.Label("KEY");

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinHeight(50), GUILayout.ExpandHeight(true));

            EditorGUI.BeginChangeCheck();
            KeyValue.stringValue = EditorGUILayout.TextArea(KeyValue.stringValue, GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndScrollView();

            if (myTarget.GetParceKeys().Length != myTarget.Keys.Length)
            {
                if (GUILayout.Button("Refresh keys"))
                    myTarget.CreateLocalKeys();
            }

            var keys = myTarget.Keys;
            GUILayout.Space(15);

            if (keys.Length == 0)
            {
                EditorGUILayout.HelpBox("Key {1..n} in TEXT not found", MessageType.Error);
            }

            for (var index = 0; index < keys.Length; index++)
            {
                GUILayout.Label("{" + index + "}");

                keys[index] = EditorGUILayout.TextField("", keys[index]);
                if (!string.IsNullOrEmpty(keys[index]))
                {
                    ShowLocalizeValues(myTarget, index);
                    KeyValidCheck(keys[index]);
                }

                GUILayout.Space(10);
            }

            if (GUILayout.Button("Preview"))
            {
                UpdateValue(myTarget);
            }
        }

        private void ShowLocalizeValues(UILocalizationKeys myTarget,int index)
        {
            string searchValue = myTarget.Keys[index].ToLower();

            const int maxCount = 5;
            Dictionary<string, int> helpKeys = new Dictionary<string, int>(maxCount);
            for (int i = 0; i < searchKeys.Length; i++)
            {
                string key = searchKeys[i];
                if (key == searchValue)
                {
                    return;
                }
                if (key.Contains(searchValue))
                {
                    helpKeys.Add(key, i);
                    if (helpKeys.Count == maxCount)
                    {
                        break;
                    }
                }
            }
            if (helpKeys.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();

            var color = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            foreach (KeyValuePair<string, int> kvp in helpKeys)
            {
                if (GUILayout.Button(kvp.Key))
                {
                    myTarget.Keys[index] = localizationKeys[kvp.Value];
                    UpdateValue(myTarget);
                    GUI.FocusControl(string.Empty);
                }
            }
            GUI.backgroundColor = color;
        }

        private void UpdateValue(UILocalizationKeys myTarget)
        {
            var result = new string[myTarget.Keys.Length];
            for (var index = 0; index < myTarget.Keys.Length; index++)
            {
                var myTargetKey = myTarget.Keys[index];
                int keyIndex = GetIdByKey(myTargetKey, searchKeys);
                if (keyIndex != -1)
                {
                    result[index] = localizationValues[keyIndex];
                    
                    myTarget.Keys[index] = localizationKeys[keyIndex];
                }
                else
                {
                    result[index] = "";
                }
            }

            myTarget.SetEditorValue(result);
        }
    }
}