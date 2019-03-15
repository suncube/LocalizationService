using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializeField]
public class DownloaderSettings
{
    public string spreadSheetKey = "";
    public string loadingDir = "./Assets/LocalizeService/Resources/Localization";
    public string accessCode = "";

    private const string SettingName = "DownloaderSettings.json";
    private const string SettingPath = "./Assets/LocalizeService/";
    private static string Path {
        get { return string.Format(SettingPath+"{0}", SettingName); }
    }

    public static DownloaderSettings GetInstance()
    {
        return new DownloaderSettings();
    }

    public static DownloaderSettings Load()
    {
        if (!File.Exists(Path))
            return new DownloaderSettings();

        using (StreamReader streamReader = File.OpenText(Path))
        {
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<DownloaderSettings>(jsonString);
        }
    }

    public void Save()
    {
        string jsonString = JsonUtility.ToJson(this);
        if (!File.Exists(Path))
            Directory.CreateDirectory(SettingPath);

        using (StreamWriter streamWriter = File.CreateText(Path))
        {
            streamWriter.Write(jsonString);
        }
    }
}

