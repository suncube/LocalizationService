using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Localization
{
    public class UILocalizationKeys : UILocalization
    {
        public string[] Keys = new string[]{};

        protected override void Initialize()
        {
            base.Initialize();
            //Key = _textObject.Text;
        }

        public override void Localize()
        {
            if (!LocalizationService.IsLive || _textObject == null) 
                return;

            string[] values = new string[Keys.Length];

            for (var index = 0; index < values.Length; index++)
            {
                values[index] = LocalizationService.Instance.GetTextByKey(Keys[index]);
            }
            SetTextValue(string.Format(Key, values));
        }

#if UNITY_EDITOR
        public void SetEditorValue(string[] keyValues)
        {
            SetTextValue(string.Format(Key, keyValues));
        }
#endif
        #region Helpers

        public string[] GetParceKeys()
        {
            //if (_textObject == null)
            //    _textObject = new TextObject(gameObject);

            //Key = _textObject.Text;
            if (string.IsNullOrEmpty(Key))
            {
                return new string[]{};
            }

            var matches = Regex.Matches(Key, "{\\d}");
            return matches
                .OfType<Match>()
                .Select(m => m.Value)
                .Distinct().ToArray();
        }

        public void CreateLocalKeys()
        {
            var matchCollection = GetParceKeys();

            var newKeys = new string[matchCollection.Length];
            for (var index = 0; index < matchCollection.Length; index++)
            {

                if (index < Keys.Length)
                {
                    newKeys[index] = Keys[index];
                }
                else
                {

                    newKeys[index] = string.Empty;
                }
            }

            Keys = newKeys;
        }

#endregion
    }
}