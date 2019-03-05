using System;
using UnityEngine;
using UnityEngine.UI;

namespace Localization
{

    public class TextObject
	{
		public string Text
		{
			set
			{
				switch (TextType)
				{
					case TextType.UiText:
						UiText.text = value;
						break;
					case TextType.MeshText:
						MeshText.text = value;
						break;
					case TextType.TextMeshPro:
						MeshProText.GetType().GetProperty("text").SetValue(MeshProText,value, null);
						break;
				}
			}

		    get
		    {
		        switch (TextType)
		        {
		            case TextType.UiText:
		                return UiText.text;

		            case TextType.MeshText:
		                return MeshText.text;
		            case TextType.TextMeshPro:
		                return (string) MeshProText.GetType().GetProperty("text").GetValue(MeshProText, null);
		        }

		        return String.Empty;
		    }
		}
		public TextType TextType { get; private set; }

		private Component MeshProText = null;
		private Text UiText = null;
		private TextMesh MeshText = null;

		public TextObject(GameObject gameObject)
		{
			TextType = TextType.None;

			UiText = gameObject.GetComponent<Text>();
			if (UiText)
			{
				TextType = TextType.UiText;
				return;
			}

			MeshText = gameObject.GetComponent<TextMesh>();
			if (MeshText)
			{
				TextType = TextType.MeshText;
				return;
			}
            
			var type = TypeHelper.GetTypeByName("TextMeshPro", "TextMeshPro");
			if (type != null)
			{
				MeshProText = gameObject.GetComponent(type);
				if (MeshProText != null)
				{
					TextType = TextType.TextMeshPro;
                    return;
				}
			}
		   
            type = TypeHelper.GetTypeByName("TextMeshProUGUI", "TextMeshPro");
		    if (type != null)
		    {
		        MeshProText = gameObject.GetComponent(type);
		        if (MeshProText != null)
		        {
		            TextType = TextType.TextMeshPro;
                    return;
		        }
		    }

            

        }

		public void Delete()
		{
			MeshProText = null;
			UiText = null;
			MeshText = null;
		}
	}
}