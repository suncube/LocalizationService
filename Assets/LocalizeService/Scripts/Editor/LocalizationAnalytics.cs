using System.Collections.Generic;
using Localization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Localization
{
	public enum KeyStatus
	{
		EmptyKey,
		KeyNotFound,
		TextComponent_NotFound,
		Actual
	}

	public class FindInfo
	{
		public KeyStatus KeyStatus { get; private set; }
		public string Name { get; private set; }
		public string Path { get; private set; }
		public string Key { get; private set; }

		public FindInfo(KeyStatus keyStatus, string name, string path, string key)
		{
			KeyStatus = keyStatus;
			Name = name;
			Path = path;
			Key = key;
		}
	}

	public class LocalizationAnalytics : EditorWindow
	{

		[MenuItem("Window/Localization/Analytic Keys")]
		private static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof (LocalizationAnalytics));
		}

		private string[] localizationKeys = null;
		private Dictionary<string, bool> useKeys = null;
		private List<FindInfo> sceneInfo = null;
		private List<FindInfo> prefabInfo = null;
		private Vector2 scrollPosTab1;
		private Vector2 scrollPosTab2;
		private Vector2 scrollPosTab3;
		private int tab;

		private void Analytics()
		{
			SetLocalizationList();

			FindSceneUse();
			FindPrefabUse();
			//FindCodeUse();
		}

		private void SetLocalizationList()
		{
			localizationKeys = LocalizationService.GetLocalizationKeys();
			useKeys = new Dictionary<string, bool>();

			foreach (var localizationKey in localizationKeys)
			{
				useKeys.Add(localizationKey, false);
			}
		}

		private void FindSceneUse()
		{
			sceneInfo = new List<FindInfo>();

			foreach (var settingsScene in EditorBuildSettings.scenes)
			{
				EditorSceneManager.OpenScene(settingsScene.path, OpenSceneMode.Single);
				Scene scene = SceneManager.GetActiveScene();

				var rootGameObjects = scene.GetRootGameObjects();
				foreach (var o in rootGameObjects)
				{
					var componentInChildren = o.gameObject.GetComponentsInChildren<UILocalization>();
				    sceneInfo.AddRange(GetKeyInfo(componentInChildren,scene.name));
                    
				}
			}
		}

	    private void FindPrefabUse()
		{
			prefabInfo = new List<FindInfo>();

			string[] allPrefabs = GetAllPrefabs();
			foreach (string prefab in allPrefabs)
			{
				var load = AssetDatabase.LoadMainAssetAtPath(prefab);
				GameObject go;
				try
				{
					go = (GameObject) load;
					var componentInChildren = go.GetComponentsInChildren<UILocalization>();

				    prefabInfo.AddRange(GetKeyInfo(componentInChildren, prefab));
				}
				catch
				{
					Debug.Log("For some reason, prefab " + prefab + " won't cast to GameObject");

				}
			}
		}

	    private FindInfo[] GetKeyInfo(UILocalization[] componentList, string path)
	    {
	        List<FindInfo> result = new List<FindInfo>();
            foreach (var component in componentList)
	        {
	            if (component.InitializeTextObject() == TextType.None)
	            {
	                result.Add(new FindInfo(KeyStatus.TextComponent_NotFound, component.gameObject.name, path, component.Key));

	            }

                var uiLocalizationKeys = component as UILocalizationKeys;
	            if (uiLocalizationKeys != null)
	            {
	                foreach (var key in uiLocalizationKeys.Keys)
	                    result.Add(new FindInfo(GetKeyStatus(key), component.gameObject.name, path, key));
	            }
	            else
	            {
	                result.Add(new FindInfo(GetKeyStatus(component.Key), component.gameObject.name, path,
	                    component.Key));
	            }
	        }

	        return result.ToArray();
	    }

	    private KeyStatus GetKeyStatus(string key)
	    {
	        if (string.IsNullOrEmpty(key))
	            return KeyStatus.EmptyKey;
	        else if (!useKeys.ContainsKey(key))
	            return KeyStatus.KeyNotFound;


	        useKeys[key] = true;

	        return KeyStatus.Actual;
	    }

        private void FindCodeUse()
		{
			//string projectPath = Application.dataPath;
			//projectPath = projectPath.Substring(0, projectPath.IndexOf("Assets"));
			//string[] allAssets = AssetDatabase.GetAllAssetPaths();

			//
			// Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
			//

			//foreach (string asset in allAssets)
			//{
			//	int indexCS = asset.IndexOf(".cs");
			//	int indexJS = asset.IndexOf(".js");
			//	if (indexCS != -1 || indexJS != -1)
			//	{
			//		try
			//		{
			//			System.IO.FileStream FS = new System.IO.FileStream(projectPath + asset, System.IO.FileMode.Open,
			//				System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
			//			System.IO.StreamReader SR = new System.IO.StreamReader(FS);

			//			string line;
			//			int lineNum = 0;
			//			while (!SR.EndOfStream)
			//			{
			//				lineNum++;
			//				line = SR.ReadLine();
			//				int index = line.IndexOf("GetTextByKeyWithLocalize");
			//				if (index != -1)
			//				{
			//					Debug.Log(asset + "  " + lineNum);
			//					Debug.Log(line);
			//				}
			//			}
			//		}
			//		catch
			//		{
			//		}
			//	}
			//}
		}

		private void DrawHead(string[] columns, GUIStyle style)
		{

			EditorGUILayout.BeginHorizontal();

			GUI.backgroundColor = Color.gray;

			foreach (var column in columns)
			{
				EditorGUILayout.LabelField(column, style);
			}
			
			EditorGUILayout.EndHorizontal();
			//
		}

		private void OnGUI()
		{
			titleContent.text = "Analytic Keys";

			GUILayout.BeginVertical();


			bool sceneUse = sceneInfo != null && sceneInfo.Count > 0;
			bool prefabUse = prefabInfo != null && prefabInfo.Count > 0;

			string buttonText = "Find Use";
			if (sceneUse || prefabUse)
			{
				buttonText = "Refresh";
			}

			if (GUILayout.Button(buttonText))
			{
				Analytics();
			}

			if (!sceneUse && !prefabUse)
			{
				EditorGUILayout.HelpBox("The search takes place in [Scenes in Build] and [Prefabs].\nSearch in code is not supported.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Save your changes for current scene! ", MessageType.Warning);

				GUILayout.EndVertical();
				return;
			}


			tab = GUILayout.Toolbar(tab, new string[] {"Actual Use ", "Error Use", "Unused Keys"});

			GUI.backgroundColor = Color.gray;
			GUIStyle styleList = new GUIStyle(EditorStyles.textField);
			styleList.normal.textColor = Color.white;
			GUIStyle styleCategory = new GUIStyle(EditorStyles.textField);
			styleCategory.normal.textColor = Color.black;


			if (tab == 0)
			{
				string category = string.Empty;
				scrollPosTab1 = EditorGUILayout.BeginScrollView(scrollPosTab1);

				GUILayout.Label("Scene", EditorStyles.boldLabel);
				DrawHead(new []{ "[ KEY ]" ,"OBJECT_NAME"},styleList);
				if (sceneInfo != null)
				{
					foreach (var findInfo in sceneInfo)
					{
						if (findInfo.KeyStatus != KeyStatus.Actual)
							continue;

						if (category != findInfo.Path)
						{
							category = findInfo.Path;
							GUI.backgroundColor = Color.cyan;
							EditorGUILayout.LabelField(findInfo.Path, styleCategory);
							GUI.backgroundColor = Color.gray;
						}

						EditorGUILayout.BeginHorizontal();

						EditorGUILayout.LabelField(string.Format("[ {0} ]", findInfo.Key), styleList);
						EditorGUILayout.LabelField(findInfo.Name, styleList);

						EditorGUILayout.EndHorizontal();
					}
				}
				GUILayout.Label("Prefabs", EditorStyles.boldLabel);
				DrawHead(new[] { "[ KEY ]", "OBJECT_NAME" }, styleList);
				if (prefabInfo != null)
				{
					foreach (var findInfo in prefabInfo)
					{
						if (findInfo.KeyStatus != KeyStatus.Actual)
							continue;
						if (category != findInfo.Path)
						{
							category = findInfo.Path;
							GUI.backgroundColor = Color.cyan;
							EditorGUILayout.LabelField(findInfo.Path, styleCategory);
							GUI.backgroundColor = Color.gray;
						}

						EditorGUILayout.BeginHorizontal();

						EditorGUILayout.LabelField(string.Format("[ {0} ]", findInfo.Key), styleList);
						EditorGUILayout.LabelField(findInfo.Name, styleList);

						EditorGUILayout.EndHorizontal();

					}
				}

				EditorGUILayout.EndScrollView();
			}
			else if (tab == 1)
			{
				string category = string.Empty;
				scrollPosTab2 = EditorGUILayout.BeginScrollView(scrollPosTab2);

				GUILayout.Label("Scene", EditorStyles.boldLabel);
				DrawHead(new[] { "KEY_STATUS", "[ KEY ]", "OBJECT_NAME" }, styleList);
				if (sceneInfo != null)
				{

					foreach (var findInfo in sceneInfo)
					{
						if (findInfo.KeyStatus == KeyStatus.Actual)
							continue;

						if (category != findInfo.Path)
						{
							category = findInfo.Path;
							GUI.backgroundColor = Color.cyan;
							EditorGUILayout.LabelField(findInfo.Path, styleCategory);
							GUI.backgroundColor = Color.gray;
						}

						EditorGUILayout.BeginHorizontal();

						GUI.backgroundColor = Color.red;
						EditorGUILayout.LabelField(string.Format("({0})", findInfo.KeyStatus), styleList);
						GUI.backgroundColor = Color.gray;

						EditorGUILayout.LabelField(string.Format("[ {0} ]", findInfo.Key), styleList);
						EditorGUILayout.LabelField(findInfo.Name, styleList);

						EditorGUILayout.EndHorizontal();
					}
				}

				GUILayout.Label("Prefabs", EditorStyles.boldLabel);
				DrawHead(new[] { "KEY_STATUS", "[ KEY ]", "OBJECT_NAME" }, styleList);
				if (prefabInfo != null)
				{
					foreach (var findInfo in prefabInfo)
					{
						if (findInfo.KeyStatus == KeyStatus.Actual)
							continue;

						if (category != findInfo.Path)
						{
							category = findInfo.Path;
							GUI.backgroundColor = Color.cyan;
							EditorGUILayout.LabelField(findInfo.Path, styleCategory);
							GUI.backgroundColor = Color.gray;
						}

						EditorGUILayout.BeginHorizontal();

						GUI.backgroundColor = Color.red;
						EditorGUILayout.LabelField(string.Format("({0})", findInfo.KeyStatus), styleList);
						GUI.backgroundColor = Color.gray;

						EditorGUILayout.LabelField(string.Format("[ {0} ]", findInfo.Key), styleList);
						EditorGUILayout.LabelField(  findInfo.Name, styleList);

						EditorGUILayout.EndHorizontal();
					}
				}

				EditorGUILayout.EndScrollView();
			}
			else if (tab == 2)
			{
				scrollPosTab3 = EditorGUILayout.BeginScrollView(scrollPosTab3);

				foreach (var useKey in useKeys)
				{
					if (useKey.Value)
						continue;

					EditorGUILayout.SelectableLabel(useKey.Key, styleList, GUILayout.Height(20));

				}

				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("This keys that are not used. ", MessageType.Info);
				EditorGUILayout.HelpBox("Search in the code is not supported.", MessageType.Warning);

				EditorGUILayout.EndScrollView();
			}


			GUILayout.EndVertical();
		}

		public static string[] GetAllPrefabs()
		{
			string[] temp = AssetDatabase.GetAllAssetPaths();
			List<string> result = new List<string>();
			foreach (string s in temp)
			{
				if (s.Contains(".prefab")) result.Add(s);
			}
			return result.ToArray();
		}
	}
}