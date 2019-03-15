using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloaderSettings : ScriptableObject
{
    public string spreadSheetKey = "";
    public string loadingDir = "./Assets/LocalizeService/Resources/Localization";
    public string accessCode = "";


    public static DownloaderSettings GetInstance()
    {
        return new DownloaderSettings();
    }

    public void Load()
    {

    }

    public void Save()
    {

    }
}
