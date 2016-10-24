using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SunCubeStudio.Localization
{
    public class LocalizationService : MonoBehaviour
    {
        public static string LocalizationFilePath = "Localization/localize";
        public static LocalizationService Instance;
        public Action OnChangeLocalization;

        private string _localization = "English";
        private Dictionary<string, Dictionary<string, string>> localizationLibrary;

        public string Localization
        {
            get { return _localization; }
            set
            {
                _localization = value;
                PlayerPrefs.SetString("LocalizeId", _localization);
                if (OnChangeLocalization != null)
                    OnChangeLocalization();
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        #region Localize Logic
        private void Initialize()
        {
            //initialize
            localizationLibrary = LoadLocalizeFileHelper();
            Localization = PlayerPrefs.GetString("LocalizeId","English");
        }

        private static Dictionary<string, Dictionary<string, string>>  ParseLocalizeFile(string[,] grid)
        {
            // language
           var  result = new Dictionary<string, Dictionary<string, string>>( grid.GetUpperBound(0)-1);
            for (int col = 1; col < grid.GetUpperBound(0); col++)
                result.Add(grid[col, 0], new Dictionary<string, string>(grid.GetUpperBound(1) - 1)); 

            for (int ln = 1; ln < grid.GetUpperBound(1); ln++)
                for (int col = 1; col < grid.GetUpperBound(0); col++)
                    result[grid[col, 0]].Add(grid[0, ln], grid[col, ln]);

            return result;
        }
        public string GetTextByKey(string key)
        {
            return GetTextByKeyWithLocalize(key, _localization);
        }

        public string GetTextByKeyWithLocalize(string key,string localize)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(localize)) return "[EMPTY]";

            Dictionary<string, string> dictionary;
            if (localizationLibrary.TryGetValue(localize, out dictionary))
            {
                string result;
                if (dictionary.TryGetValue(key, out result))
                    return result;
            }

            return string.Format("[ERROR KEY {0}]",key);
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
        public Dictionary<string, Dictionary<string, string>> LoadLocalizeFileHelper()
        {
            var languages = Resources.Load(LocalizationFilePath, typeof(TextAsset)) as TextAsset;
            if (languages == null) return null;
            var resultGrid = CSVReader.SplitCsvGrid(languages.text);
            return ParseLocalizeFile(resultGrid);
        }
        public static Dictionary<string, string> GetLocalizationsByKey(string key)
        {
            var languages = Resources.Load(LocalizationFilePath, typeof(TextAsset)) as TextAsset;
            if (languages == null) return null;
            var resultGrid = CSVReader.SplitCsvGrid(languages.text);
            var localizeFile = ParseLocalizeFile(resultGrid);
            var result = new Dictionary<string, string>();

            foreach (var loc in localizeFile)
            {
                string value;
                result.Add(loc.Key, loc.Value.TryGetValue(key, out value) ? value : String.Empty);
            }

            return result;
        }

        public static string[] GetLocalizationKeys()
        {
            var languages = Resources.Load(LocalizationFilePath, typeof(TextAsset)) as TextAsset;
            if (languages == null) return null;
            var resultGrid = CSVReader.SplitCsvGrid(languages.text);
            var localizeFile = ParseLocalizeFile(resultGrid);
            var result = new List<string>();

            foreach (var loc in localizeFile)
            {
                result.AddRange(loc.Value.Select(item => item.Key));
                return result.ToArray();
            }

            return result.ToArray();
        }

        #endregion Helpers
    }
}
