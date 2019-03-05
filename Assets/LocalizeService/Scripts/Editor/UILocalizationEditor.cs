
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(UILocalization))]
    [CanEditMultipleObjects]
    public class UILocalizationEditor : Editor
	{
	    protected string[] localizationKeys = null;
	    protected static string[] searchKeys = null;
	    protected static string[] localizationValues = null;

	    protected SerializedProperty KeyValue;

        private void OnEnable()
        {
            Initialize();
        }

	    protected virtual void Initialize()
	    {
	        Dictionary<string, string> localization = LocalizationService.GetLocalizationKeyValue();
	        localizationKeys = localization.Keys.ToArray();
	        searchKeys = localization.Keys.Select(t => t.ToLower()).ToArray();
	        localizationValues = localization.Values.ToArray();

	        KeyValue = serializedObject.FindProperty("Key");
        }


	    public override void OnInspectorGUI()
		{
			var myTarget = (UILocalization)target;

            if (!AddingTypeTest(myTarget))
		        return;

		    EditorGUILayout.LabelField("KEY");

		    EditorGUI.BeginChangeCheck();
            KeyValue.stringValue = EditorGUILayout.TextArea(KeyValue.stringValue);
		    if (EditorGUI.EndChangeCheck())
		    {
		        serializedObject.ApplyModifiedProperties();
		    }


            if(string.IsNullOrEmpty(myTarget.Key)) return;

		    ShowLocalizeValues(myTarget);
		    var check = KeyValidCheck(myTarget.Key);

		    if (check && GUILayout.Button("Preview"))
		        UpdateValue(myTarget);

        }


	    private void ShowLocalizeValues(UILocalization myTarget)
	    {
	        string searchValue = myTarget.Key.ToLower();

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
	                myTarget.Key = localizationKeys[kvp.Value];
	                UpdateValue(myTarget);
	                GUI.FocusControl(string.Empty);
	            }
	        }
	        GUI.backgroundColor = color;
        }

	    private void UpdateValue(UILocalization myTarget)
	    {
	        int keyIndex = GetIdByKey(myTarget.Key, searchKeys);
	        if (keyIndex != -1)
	        {
	            myTarget.SetEditorValue(localizationValues[keyIndex]);
	            // update lower case to real one
	            myTarget.Key = localizationKeys[keyIndex];
	        }
	    }

        protected bool KeyValidCheck(string key)
	    {
	        var idByKey = GetIdByKey(key, localizationKeys);
	        if (idByKey == -1)
	        {
	            EditorGUILayout.HelpBox("KEY not found in localization file. ", MessageType.Error);
	            return false;
	        }
            return true; 
        }

	    protected bool AddingTypeTest(UILocalization myTarget)
	    {
	        if (myTarget.InitializeTextObject() == TextType.None)
	        {
	            EditorGUILayout.HelpBox("TEXT component not found \n\n[UIText|MeshText|TextMeshPro]", MessageType.Error);
	            return false;
            }

	        return true;
	    }

	    protected int GetIdByKey(string key, string[] keys)
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
