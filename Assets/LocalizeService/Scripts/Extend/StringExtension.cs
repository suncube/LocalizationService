using System;
using Localization;

public static class StringExtension
{
	public static string Translate(this string _this)
	{
		return LocalizationService.Instance.GetTextByKey(_this);
	}

    public static string ParceNewLine(this string _this)
    {
        if (string.IsNullOrEmpty(_this))
            return string.Empty;

        return _this.Replace("\\n", Environment.NewLine);
    }
}