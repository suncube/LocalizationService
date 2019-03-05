using System;
using UnityEngine;

namespace Localization
{
	public enum TextType
	{
		None,
		UiText,
		MeshText,
		TextMeshPro,
	}


    public class UILocalization : MonoBehaviour
    {
        protected TextObject _textObject;

		public string Key;

        #region Localize Logic

#if UNITY_EDITOR
        public void SetEditorValue(string text)
        {
            text = ParceText(text);
            if (_textObject != null)
            {
                _textObject.Text = text;
            }
        }
#endif
        private void Start()
		{
		    Initialize();
            OnChangeLocalization();
        }

        protected virtual void Initialize()
        {
            LocalizationService.Instance.OnChangeLocalization += OnChangeLocalization;

			_textObject = new TextObject(gameObject);
        }

        private void OnChangeLocalization()
        {
            Localize();
        }

        public virtual void Localize()
        {
			if(LocalizationService.IsLive && _textObject != null)
				SetTextValue(LocalizationService.Instance.GetTextByKey(Key));
        }

        protected void SetTextValue(string text)
        {
	        _textObject.Text = ParceText(text); 
        }

        private string ParceText(string text)
        {
            return text.ParceNewLine();
        }

        private void OnDestroy()
        {
			if(_textObject != null)
				_textObject.Delete();

	        _textObject = null;

			if (!LocalizationService.IsLive) return;

			LocalizationService.Instance.OnChangeLocalization -= OnChangeLocalization;
        }
        #endregion Localize Logic

        #region Helpers

		public TextType InitializeTextObject()
		{
			if(_textObject == null)
				_textObject = new TextObject(gameObject);

			return GetTextType();
		}

		public TextType GetTextType()
        {
			return _textObject.TextType;
		}

	    public bool IsHasTextObject()
	    {
		    return _textObject != null;
	    }

	    #endregion Helpers
	}
}