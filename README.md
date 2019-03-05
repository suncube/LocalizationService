

# Localization Service 

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

 
![](http://drive.google.com/uc?export=view&id=1CfFUej_LybuFx4-tnbTyCU54QY4FWXQY)



Create for each language its own list.


![](http://drive.google.com/uc?export=view&id=1jWXRVy1RTz7Khs7SSw_mK3TyxN12uDTq)


  
**Save and export** each localization to file **“Localization” .CSV** file  or use [Download manager](#download-manager)

>  Copy/put the file to the resources (Resources\Localization). Rename
> the file to language name. [!!! Names of languages you can see
> here](https://docs.unity3d.com/ScriptReference/SystemLanguage.html*)

  

## Download Manager

It is possible to download the latest localization changes from your Google sheet. Plugin uses the [Google Sheets API](https://developers.google.com/sheets/) . If you want to use your project for working with tables [create here](https://console.developers.google.com/project) .

This tool can be opened by path **Window / Localization**

  

![](http://drive.google.com/uc?export=view&id=1POMWHWgCvuSZvqwhITahqMk166ubpxnT)


  
  

Next - open your localization page and copy google sheet **IDKey**

![](http://drive.google.com/uc?export=view&id=1THMj9Ik3dd8090Mm--JYQCE9Gjdwg75z)

  
  

Copy and paste this to **SpreadSheetKey** :

  

![](http://drive.google.com/uc?export=view&id=1OqQgV9LJDJRET0nx_AoNcjAmt1hKuPTu)

> Check the path to localization files will be downloaded. (! it must be
> a folder **../Resources/Localization**)

Press **SET** button

  

![](http://drive.google.com/uc?export=view&id=1ZKfUweYaLstRGGvfaM3p4-s5GlLoRgW-)


  

Get session key (this case will be repeated if the key is expired or invalid)

![](http://drive.google.com/uc?export=view&id=1UNcmosdPSbhPFGGNd_68fW275Y24O1dH)

Copy and paste this to **AccessCode** field:

![](http://drive.google.com/uc?export=view&id=1_y4Voz3iJ0s3po1t_qVrMM6SVTTXKoPl)

Press **SET** button
  
![](http://drive.google.com/uc?export=view&id=1ZKfUweYaLstRGGvfaM3p4-s5GlLoRgW-)

  

Now you need to get list of localization , press GET LIST

  

![](http://drive.google.com/uc?export=view&id=1NLN-WXNafcUsTT6Qs49C4059VFzYEljw)

  

A list of localizations will shown like:

![](http://drive.google.com/uc?export=view&id=1BkARwFRQF5YOsogp-z2noe7Fxh2rZoiA)

  

1.  List can be updated
    
2.  Localization can be removed from the list of updated localizations
    
3.  Download selected list
    



## How to use
  
1) Select your text component on the scene **MeshText/UIText/TextMeshPro** and add the component UILocalization.

  

2) Set localization’s key or write to the Key field
    
 

![](http://drive.google.com/uc?export=view&id=138CDoGSe2fUmX4ULj5yIBtAY76NaWsss)

  

If the entered key does not exist you will get an **error**. Check that the key in the localization!

  

![](http://drive.google.com/uc?export=view&id=1TLBkYY4qqygXIa-8QxKd95pDTzSs09Il)

  

New version include ability to **localize multikey**. This is very useful when you using tags.

  

![](http://drive.google.com/uc?export=view&id=1ujBIw4QAHTqNQpV-9uCrgnC34Lwqmsmp)

  

You need enter the keys for each **id** {0...N}

![](http://drive.google.com/uc?export=view&id=1sQtiYGgRZ7UH1fs1nzyV7Cew5T6ec6HN)

  
  

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
![](http://drive.google.com/uc?export=view&id=1eLY5IMKBuoQLQlw44PdLBKy7Rd84zTj0)

![](http://drive.google.com/uc?export=view&id=1IybG15ecxqNC1mqy_YE8g_cdq5F--Efa)

  

1.  Update Analysis
    
2.  Search prefabs and scenes
    
3.  Places where there are errors when using
   
   ![](http://drive.google.com/uc?export=view&id=1M3gDBS_wrS8dbfZOciFtf2AsTQz96l2C)
 

>**Errors Types:**
>*EmptyKey* - key not set
>*KeyNotFound* - key not found
*TextComponent_NotFound *- no supported component on GameObject - **MeshText / UIText / TextMeshPro**
> 
  


4.  List of keys that are not used
    
>! Does not check usage in the code of the project like LocalizationService.Instance.GetTextByKey("localization1")

![](http://drive.google.com/uc?export=view&id=1TEZSxNZTKiiImsffA6yf-qxEhnvF9fYh)
