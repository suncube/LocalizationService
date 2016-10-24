using System;
using UnityEngine;
using UnityEngine.UI;

namespace SunCubeStudio.Localization
{
    public class UILocalization : MonoBehaviour
    {
        public string _key;

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                Localize();
            }
        }

        private Text UiText;
        private TextMesh MeshText;

        #region Localize Logic
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            LocalizationService.Instance.OnChangeLocalization += OnChangeLocalization;
            UiText = gameObject.GetComponent<Text>();
            MeshText = gameObject.GetComponent<TextMesh>();

            OnChangeLocalization();
        }
        private void OnChangeLocalization()
        {
            Localize();
        }
        private void Localize()
        {
            SetTextValue(LocalizationService.Instance.GetTextByKey(_key));
        }
        private void SetTextValue(string text)
        {
            text = ParceText(text);

            if (UiText != null)
                UiText.text = text;

            if (MeshText != null)
                MeshText.text = text;

            // error check
            if (text == "[EMPTY]" || text == string.Format("[ERROR KEY {0}]", _key))
            {

                if (UiText != null)
                {
                    UiText.color = Color.red;
                }
                if (MeshText != null)
                {
                    MeshText.color = Color.red;
                }
            }

        }

        private string ParceText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\\n", Environment.NewLine); 
        }

        private void OnDestroy()
        {
            LocalizationService.Instance.OnChangeLocalization -= OnChangeLocalization;
        }
        #endregion Localize Logic

        #region Helpers
        public bool IsHasOutputHelper()
        {
            UiText = gameObject.GetComponent<Text>();
            MeshText = gameObject.GetComponent<TextMesh>();
          return  UiText != null || MeshText != null;
        }
        #endregion Helpers
    }
}