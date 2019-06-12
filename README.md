

# Localization Service ![GitHub release](https://img.shields.io/github/release/suncube/LocalizationService.svg)

Plugin for localization keys. Works with **MeshText/UIText/TextMeshPro** 

Support [suncubestudio@gmail.com](mailto:suncubestudio@gmail.com) - subject of mail [Localization Sevice]

# How to start
 1) Import plugin from [Asset Store](http://u3d.as/miF) or copy from [GitHub](https://github.com/suncube/LocalizeService) 
 2) Create and Load  .CSV
 3) Add **UILocalization** component to **MeshText/UIText/TextMeshPro** 
 #### [HOW TO USE ](#how-to-use)
 
 - Help tools: [Analytic of Keys ](#help-tools)


# Create files of localization
Plugin use .CSV files for localization key/value. Create a table of localization and export it to a .CSV file. You can use for this google sheets, because all your team can edit this with sync.

 ![](/ReadmeSource/preview1.png)



Create for each language its own list.

![](/ReadmeSource/preview2.png)


  
**Save and export** each localization to file **“Localization” .CSV** file  or use [Download manager](#download-manager)

>  Copy/put the file to the resources (Resources\Localization). Rename
> the file to language name. [!!! Names of languages you can see
> here](https://docs.unity3d.com/ScriptReference/SystemLanguage.html*)

  

## Download Manager

It is possible to download the latest localization changes from your Google sheet. Plugin uses the [Google Sheets API](https://developers.google.com/sheets/) . If you want to use your project for working with tables [create here](https://console.developers.google.com/project) .

This tool can be opened by path **Window / Localization**

  

![](/ReadmeSource/preview3.png)


  
  

Next - open your localization page and copy google sheet **IDKey**

![](/ReadmeSource/preview4.png)

  
  

Copy and paste this to **SpreadSheetKey** :

  

![](/ReadmeSource/preview5.png)

> Check the path to localization files will be downloaded. (! it must be
> a folder **../Resources/Localization**)

Press **SET** button

  

![](/ReadmeSource/preview6.png)


  

Get session key (this case will be repeated if the key is expired or invalid)

![](/ReadmeSource/preview7.png)

Copy and paste this to **AccessCode** field:

![](/ReadmeSource/preview8.png)

Press **SET** button
  
![](/ReadmeSource/preview9.png)

  

Now you need to get list of localization , press GET LIST

  

![](/ReadmeSource/preview10.png)

  

A list of localizations will shown like:

![](/ReadmeSource/preview11.png)
  

1.  List can be updated
    
2.  Localization can be removed from the list of updated localizations
    
3.  Download selected list
    



## How to use
  
1) Select your text component on the scene **MeshText/UIText/TextMeshPro** and add the component UILocalization.

  

2) Set localization’s key or write to the Key field
    
 

![](/ReadmeSource/preview12.png)
  

If the entered key does not exist you will get an **error**. Check that the key in the localization!

  

![](/ReadmeSource/preview13.png)
  

New version include ability to **localize multikey**. This is very useful when you using tags.

  

![](/ReadmeSource/preview14.png)
  

You need enter the keys for each **id** {0...N}

![](/ReadmeSource/preview15.png)
  
  

#### Change the location in the code

  LocalizationService is singleton, for call this use
 
>       LocalizationService.Instance

  
-   When it is first started, it get and set system language by [Application.systemLanguage](https://docs.unity3d.com/ScriptReference/Application-systemLanguage.html). You can change logic of first initialize localization value in **LocalizationService.GetLocalization().**


  
for example, you can use this enum for languages values ​​in your project

  

    enum SystemLanguage
    
    LocalizationService.Instance.Localization = SystemLanguage.English.ToString();

  
  

Get the key in the code

  

     // for current set localization
    LocalizationService.Instance.GetTextByKey("localization1");


  

or

  

    "localization1".Translate();

  
  
  
  You can encode the transition to a new line by adding the translation value "**\n**".  UILocalization make this automaticly, but if you need made this from code use extension for string  **ParceNewLine()**.
 
    // key="line" value = "first line \n secondline"
        UIText.text = "line".Translate().ParceNewLine();
    
  

## Help Tools:

### Analytic Keys (in development)

Path to open **Window / Localization / Analytic keys**
This tool will allow you to analyze all keys that are used in the project (!Before starting you need save all changes in the current scene)

Press **"Find Use"**

![](/ReadmeSource/preview16.png)

![](/ReadmeSource/preview17.png)

  

1.  Update Analysis
    
2.  Search prefabs and scenes
    
3.  Places where there are errors when using
   
   ![](/ReadmeSource/preview18.png)
 

>**Errors Types:**
>*EmptyKey* - key not set
>*KeyNotFound* - key not found
*TextComponent_NotFound *- no supported component on GameObject - **MeshText / UIText / TextMeshPro**
> 
  


4.  List of keys that are not used
    
>! Does not check usage in the code of the project like LocalizationService.Instance.GetTextByKey("localization1")

![](/ReadmeSource/preview19.png)

#

[![Twitter](https://img.shields.io/badge/follow-Twitter-9cf.svg)](https://twitter.com/suncubestudio)
[![Facebook](https://img.shields.io/badge/follow-Facebook-blue.svg)](https://www.facebook.com/suncubestudio/)
[![YouTube](https://img.shields.io/badge/follow-YouTube-red.svg)](https://www.youtube.com/channel/UC4O9GHjx0ovyVYJgMg4aFMA?view_as=subscriber)
[![AssetStore](https://img.shields.io/badge/-AssetStore-lightgrey.svg)](https://assetstore.unity.com/publishers/14506)

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.donationalerts.com/r/suncube)
