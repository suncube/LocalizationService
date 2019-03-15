using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Net;
using System.IO;

namespace Localization
{
	public class LocalizationDownloader : EditorWindow
	{
		[MenuItem("Window/Localization/Downloader")]
		private static void ShowWindow()
		{
			LocalizationDownloader window = EditorWindow.GetWindow(typeof (LocalizationDownloader)) as LocalizationDownloader;
			window.Init();
		}
		
		// Access Settings
		private const string PREF_ACCESS_CODE = "accessCode";
		private const string PREF_SHEET_KEY = "spreadSheetKey";
		private static string ACCESS_TOKEN = "";
		private static string REFRESH_TOKEN = "";

		// Google Application settings
		private static string CLIENT_ID = "35584708058-glpfem3u6unhf8d4k8gq20gtleo54m7a.apps.googleusercontent.com";
		static string CLIENT_SECRET = "sh8wUKb9uoYv8Cchs0iLfRiH";
		private static string SCOPE = "https://spreadsheets.google.com/feeds";
		private static string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
		private static string TOKEN_TYPE = "refresh";

		private static GOAuth2RequestFactory RefreshAuthenticate()
		{
			OAuth2Parameters parameters = new OAuth2Parameters()
			{
				RefreshToken = ACCESS_TOKEN,
				AccessToken = REFRESH_TOKEN,
				ClientId = CLIENT_ID,
				ClientSecret = CLIENT_SECRET,
				//Scope = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds",
				Scope = "https://spreadsheets.google.com/feeds",
				AccessType = "offline",
				TokenType = "refresh"
			};
			OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
			return new GOAuth2RequestFactory("spreadsheet", "MySpreadsheetIntegration-v1", parameters);
		}

		//
		private string appName = "UnityEditor";
		private string urlRoot = "https://spreadsheets.google.com/feeds/spreadsheets/";
		private Vector2 scrollPosition;
		private float progress = 100;
		private string progressMessage = "";
		private bool isConnect = false;
		private SpreadsheetFeed spreadsheetFeed;
		private int localizationCount = 0;

	    private DownloaderSettings settings;
        private List<string> wantedSheetNames = new List<string>();

		private void OnGUI()
		{
		    if (settings == null)
		    {
                Close();
      
		    }

		    titleContent.text = "Downloader";
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView);

			GUILayout.BeginVertical();
			{
				GUILayout.Label("Settings", EditorStyles.boldLabel);

			    settings.spreadSheetKey = EditorGUILayout.TextField("Spread sheet key", settings.spreadSheetKey);
			    settings.loadingDir = EditorGUILayout.TextField("Path to loacalization files", settings.loadingDir);

				if (IsTokkenEmpty)
				{
					GUILayout.BeginHorizontal();
				    settings.accessCode = EditorGUILayout.TextField("Access code", settings.accessCode);
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("SET", EditorStyles.toolbarButton, GUILayout.Width(50)))
					{
						GetAccessCode(false);

					    if (settings != null)
					        settings.Save();
                    }
					GUILayout.EndHorizontal();

					GUILayout.EndVertical();
					EditorGUILayout.EndScrollView();
					return;
				}

				GUI.backgroundColor = Color.white;
			    settings.accessCode = EditorGUILayout.TextField("Access code", settings.accessCode);
				GUILayout.Label("");
				GUILayout.Label("Localizations:", EditorStyles.boldLabel);

				if (wantedSheetNames.Count == 0)
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Get List"))
					{
						wantedSheetNames.Clear();
						GetLocalizationList();
					}

					GUILayout.EndVertical();
					EditorGUILayout.EndScrollView();
					return;
				}

				if (GUILayout.Button("Refresh List"))
				{
					wantedSheetNames.Clear();
					GetLocalizationList();
				}

				int _removeId = -1;
				for (int i = 0; i < wantedSheetNames.Count; i++)
				{
					GUILayout.BeginHorizontal();

					EditorGUILayout.LabelField(wantedSheetNames[i]);
					if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
					{
						_removeId = i;
					}
					GUILayout.EndHorizontal();
				}

				if (_removeId >= 0)
					wantedSheetNames.RemoveAt(_removeId);

				if (wantedSheetNames.Count == 0)
				{
					GUILayout.EndVertical();
					EditorGUILayout.EndScrollView();
					return;
				}

				string downloadText = "";

				if (wantedSheetNames.Count == localizationCount)
					downloadText = "Download all sheets";
				else
					downloadText = string.Format("Download {0} sheets", wantedSheetNames.Count);

				GUILayout.Label("");

				GUI.backgroundColor = Color.green;

				if (GUILayout.Button(downloadText, GUILayout.Height(50)))
				{
					progress = 0;
					DownloadLocalizationToCsv();
				}

				GUI.backgroundColor = Color.white;
				if ((progress < 100) && (progress > 0))
				{
					if (EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100))
					{
						progress = 100;
						EditorUtility.ClearProgressBar();
					}
				}
				else
				{
					EditorUtility.ClearProgressBar();
				}
			}
			GUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}


		public void Init()
		{
			progress = 100;
			progressMessage = "";

		    LoadSettings();
            
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
		}

        private void LoadSettings()
        {
            settings = DownloaderSettings.Load();
        }

	    private void OnDestroy()
	    {
	        if (settings != null)
	            settings.Save();
	    }

	    private bool IsTokkenEmpty
		{
			get { return (ACCESS_TOKEN == "" && REFRESH_TOKEN == ""); }
		}

		private void GetLocalizationList()
		{

			if (string.IsNullOrEmpty(settings.spreadSheetKey))
			{
				Debug.LogError("spreadSheetKey can not be null!");
				return;
			}

			PlayerPrefs.SetString(PREF_SHEET_KEY, settings.spreadSheetKey);
			PlayerPrefs.Save();

			if (IsTokkenEmpty)
			{
				EditorUtility.ClearProgressBar();
				GetAccessCode();
				return;
			}

			progressMessage = "Authenticating...";
			GOAuth2RequestFactory requestFactory = RefreshAuthenticate();
			SpreadsheetsService service = new SpreadsheetsService(appName);
			service.RequestFactory = requestFactory;

			SpreadsheetQuery query = new SpreadsheetQuery();
			query.Uri = new System.Uri(urlRoot + settings.spreadSheetKey);
			progressMessage = "Get list of spreadsheets...";

			spreadsheetFeed = service.Query(query);
			if ((spreadsheetFeed == null) || (spreadsheetFeed.Entries.Count <= 0))
			{
				Debug.LogError("Not found any data!");
				EditorUtility.ClearProgressBar();
				return;
			}

			AtomEntry mySpreadSheet = spreadsheetFeed.Entries[0];
			AtomLink link = mySpreadSheet.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);
			WorksheetQuery sheetsQuery = new WorksheetQuery(link.HRef.ToString());
			WorksheetFeed sheetsFeed = service.Query(sheetsQuery);

			foreach (WorksheetEntry sheet in sheetsFeed.Entries)
			{
				wantedSheetNames.Add(sheet.Title.Text);
			}
			localizationCount = wantedSheetNames.Count;
		}

		private void DownloadLocalizationToCsv()
		{
			if (string.IsNullOrEmpty(settings.spreadSheetKey))
			{
				Debug.LogError("spreadSheetKey can not be null!");
				return;
			}

			PlayerPrefs.SetString(PREF_SHEET_KEY, settings.spreadSheetKey);
			PlayerPrefs.Save();

			if (IsTokkenEmpty)
			{
				EditorUtility.ClearProgressBar();
				GetAccessCode();
				return;
			}

			progressMessage = "Authenticating...";
			GOAuth2RequestFactory requestFactory = RefreshAuthenticate();
			SpreadsheetsService service = new SpreadsheetsService(appName);
			service.RequestFactory = requestFactory;

			progress = 5;
			EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
	
			SpreadsheetQuery query = new SpreadsheetQuery();
			query.Uri = new System.Uri(urlRoot + settings.spreadSheetKey);
			progressMessage = "Get list of spreadsheets...";

			EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
			SpreadsheetFeed feed = service.Query(query);
			if ((feed == null) || (feed.Entries.Count <= 0))
			{
				Debug.LogError("Not found any data!");
				progress = 100;
				EditorUtility.ClearProgressBar();
				return;
			}
			progress = 10;
			AtomEntry mySpreadSheet = feed.Entries[0];

			progressMessage = "Get entries from spreadsheets...";
			EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
			AtomLink link = mySpreadSheet.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);
			WorksheetQuery sheetsQuery = new WorksheetQuery(link.HRef.ToString());
			WorksheetFeed sheetsFeed = service.Query(sheetsQuery);
			progress = 15;

			
			foreach (WorksheetEntry sheet in sheetsFeed.Entries)
			{
				if ((wantedSheetNames.Count <= 0) || (wantedSheetNames.Contains(sheet.Title.Text)))
				{
					progressMessage = string.Format("Processing {0}...", sheet.Title.Text);
					EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);

					AtomLink cellsLink = sheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);
					CellQuery cellsQuery = new CellQuery(cellsLink.HRef.ToString());
					CellFeed cellsFeed = service.Query(cellsQuery);

					CreateCSVFile(sheet.Title.Text, settings.loadingDir, cellsFeed.Entries);
					if (wantedSheetNames.Count <= 0)
						progress += 85/(sheetsFeed.Entries.Count);
					else
						progress += 85/wantedSheetNames.Count;
				}
			}
			progress = 100;
			AssetDatabase.Refresh();
		}

		private void CreateCSVFile(string fileName, string outputDirectory, AtomEntryCollection cells)
		{
			if (!outputDirectory.EndsWith("/"))
				outputDirectory += "/";
			Directory.CreateDirectory(outputDirectory);

			using (CSVWriter writer = new CSVWriter(outputDirectory + fileName + ".csv"))
			{
				CSVRow line = null;
				uint row = 0;
				foreach (CellEntry curCell in cells)
				{
					if (line == null)
					{
						line = new CSVRow();
						line.Add(curCell.Cell.Value);
						row = curCell.Cell.Row;
					}
					else
					{
						if (row == curCell.Cell.Row)
						{
							line.Add(curCell.Cell.Value);
						}
						else
						{
							writer.WriteRow(line);
							line = new CSVRow();
							line.Add(curCell.Cell.Value);
							row = curCell.Cell.Row;
						}
					}
				}

				if (line != null)
				{
					writer.WriteRow(line);
				}
			}
		}

		void GetAccessCode(bool withDownload = true)
		{
			// OAuth 2.0.
			OAuth2Parameters parameters = new OAuth2Parameters();

			parameters.ClientId = CLIENT_ID;

			parameters.ClientSecret = CLIENT_SECRET;

			parameters.RedirectUri = REDIRECT_URI;

			parameters.Scope = SCOPE;

			parameters.AccessType = "offline"; 

			parameters.TokenType = TOKEN_TYPE;

			string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
			Debug.Log(authorizationUrl);
			Debug.Log("Please visit the URL above to authorize your OAuth "
			          + "request token.  Once that is complete, type in your access code to "
			          + "continue...");

			parameters.AccessCode = settings.accessCode;

			if (parameters.AccessCode == "")
			{
				Debug.LogWarning("Access code is blank!");
				EditorUtility.ClearProgressBar();
				Application.OpenURL(authorizationUrl);
				return;
			}

			Debug.Log("Get access token.");

			try
			{
				OAuthUtil.GetAccessToken(parameters);
				string accessToken = parameters.AccessToken;
				string refreshToken = parameters.RefreshToken;
				Debug.Log("OAuth Access Token: " + accessToken + "\n");
				ACCESS_TOKEN = accessToken;
				Debug.Log("OAuth Refresh Token: " + refreshToken + "\n");
				REFRESH_TOKEN = refreshToken;
				PlayerPrefs.SetString(PREF_ACCESS_CODE, settings.accessCode);
				PlayerPrefs.Save();

				if (withDownload)
					DownloadLocalizationToCsv();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				EditorUtility.ClearProgressBar();
				Application.OpenURL(authorizationUrl);
				return;
			}
		}

		public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2) certificate);
						if (!chainIsValid)
						{
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}
	}
}