using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Localization
{
	public class LocalizationService : MonoSingleton<LocalizationService>
	{
		private const string DefaultLocalizationName = "English";
		public static string LocalizationPath = "Localization/";

		public static string LocalizationFilePath
		{
			get { return LocalizationPath + _localization; }
		}

		public Action OnChangeLocalization;

		private static string _localization = "English";
		private Dictionary<string, string> localizationLibrary = new Dictionary<string, string>();

		public string Localization
		{
			get { return _localization; }
			set
			{
				_localization = value;
				localizationLibrary = LoadLocalizeFileHelper();
				SetLocalization(value);

				OnChangeLocalization.SafeInvoke();
			}
		}

		private void Awake()
		{
			Initialize();
		}

		#region Localize Logic

		private void Initialize()
		{
			Localization = GetLocalization();
			localizationLibrary = LoadLocalizeFileHelper();
		}

		private static Dictionary<string, string> ParseLocalizeFile(string[,] grid)
		{
			var result = new Dictionary<string, string>(grid.GetUpperBound(0) + 1);

			for (int ln = 1; ln <= grid.GetUpperBound(1); ln++)
				for (int col = 1; col <= grid.GetUpperBound(0); col++)
				{
					if (string.IsNullOrEmpty(grid[0, ln])
					    || string.IsNullOrEmpty(grid[col, ln])) continue;

					if (!result.ContainsKey(grid[0, ln]))
						result.Add(grid[0, ln], grid[col, ln]);
					else
					{
						Debug.LogError(string.Format("Key {0} already exist", grid[0, ln]));
					}
				}
			return result;
		}

		public string GetTextByKey(string key)
		{
			if (string.IsNullOrEmpty(key)) return "[EMPTY]";

			string keyValue;
			if (localizationLibrary.TryGetValue(key, out keyValue))
			{
				return keyValue;
			}

			return string.Format("[ERROR KEY {0}]", key);
		}

		// todo Integrate this to your PlayerPref Manager
		private string GetLocalization()
		{
		    SystemLanguage language = Application.systemLanguage;

		    if (language == SystemLanguage.ChineseSimplified)
		    {
		        language = SystemLanguage.Chinese;
		    }

            return PlayerPrefs.GetString("localization", language.ToString());
		}

		private void SetLocalization(string localize)
		{
			PlayerPrefs.SetString("localization", localize);
		}

		#endregion Localize Logic

		#region Helpers

		public string[] GetLocalizations()
		{
			var result = new string[localizationLibrary.Count];
			var i = 0;
			foreach (var loc in localizationLibrary)
			{
				result[i] = loc.Key;
				i++;
			}
			return result;
		}

		public Dictionary<string, string> LoadLocalizeFileHelper()
		{
			var languages = Resources.Load(LocalizationFilePath, typeof (TextAsset)) as TextAsset;
			if (languages == null)
			{
				// todo load any available???
				if (Localization != DefaultLocalizationName)
					LoadDefault();

				return null;
			}
			var resultGrid = CSVReader.SplitCsvGrid(languages.text);
			return ParseLocalizeFile(resultGrid);
		}

		private void LoadDefault()
		{
			Localization = DefaultLocalizationName;
		}

		public static string[] GetLocalizationKeys()
		{
			var languages = Resources.Load(LocalizationFilePath, typeof (TextAsset)) as TextAsset;
			if (languages == null) return null;
			var resultGrid = CSVReader.SplitCsvGrid(languages.text);
			var localizeFile = ParseLocalizeFile(resultGrid);
			return localizeFile.Keys.ToArray();
		}

	    public static Dictionary<string, string> GetLocalizationKeyValue()
	    {
	        var languages = Resources.Load(LocalizationFilePath, typeof(TextAsset)) as TextAsset;
	        if (languages == null) return null;
	        var resultGrid = CSVReader.SplitCsvGrid(languages.text);
	        var localizeFile = ParseLocalizeFile(resultGrid);
	        return localizeFile;
	    }

        #endregion Helpers
    }

}