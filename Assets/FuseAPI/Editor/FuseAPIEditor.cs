﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(FuseAPI))]
public class FuseAPIEditor : Editor
{
	public static readonly string ICON_PATH = "/Plugins/Android/res/drawable/ic_launcher.png";
	public static readonly int ICON_HEIGHT = 72;
	public static readonly int ICON_WIDTH = 72;

	private static bool _iconFoldout = false;

	private FuseAPI _self;
	private Texture2D _logo, _icon;
	private string _error, _newIconPath;
	private bool _p31Android, _p31iOS, _unibill;

	private void OnEnable()
	{
		string logoPath = "Assets/FuseAPI/logo.png";

		//Fix logo import settings
		TextureImporter importer = AssetImporter.GetAtPath(logoPath) as TextureImporter;
		if(importer != null)
		{
			importer.textureType = TextureImporterType.GUI;
			AssetDatabase.WriteImportSettingsIfDirty(logoPath);
		}

		_self = (FuseAPI)target;
		_logo = Resources.LoadAssetAtPath(logoPath, typeof(Texture2D)) as Texture2D;
		_icon = null;
		_error = null;
		_newIconPath = null;
		
		_p31Android = DoClassesExist("GoogleIABManager", "GoogleIAB", "GooglePurchase");
		_p31iOS = DoClassesExist("StoreKitManager", "StoreKitTransaction");
		_unibill = DoClassesExist("Unibiller");
	}

	private void OnDisable()
	{
		Resources.UnloadAsset(_logo);
		if(_icon != null)
			DestroyImmediate(_icon, true);
	}
	
	public override void OnInspectorGUI()
	{
		GUILayout.Space(8);
		if(_logo != null)
			GUILayout.Label(_logo);
		GUILayout.Space(12);

		_self.AndroidGameID = EditorGUILayout.TextField("Android Game ID", _self.AndroidGameID);
		_self.iOSGameID = EditorGUILayout.TextField("iOS Game ID", _self.iOSGameID);
		
		GUILayout.Space(8);

		_self.GCM_SenderID = EditorGUILayout.TextField("GCM Sender ID", _self.GCM_SenderID);
		_self.registerForPushNotifications = EditorGUILayout.Toggle("Push Notifications", _self.registerForPushNotifications);
		_self.logging = EditorGUILayout.Toggle("Logging", _self.logging);
		_self.persistent = EditorGUILayout.Toggle("Persistent", _self.persistent);

		GUILayout.Space(16);

#if !UNITY_3_5
		bool oldAndroidIAB = _self.androidIAB;
		bool oldandroidUnibill = _self.androidUnibill;
		bool oldiosStoreKit = _self.iosStoreKit;
		bool oldiosUnibill = _self.iosUnibill;

		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.BeginVertical();
		GUI.enabled = _p31Android;
		_self.androidIAB = EditorGUILayout.Toggle("Android Prime31 Billing", _self.androidIAB);

		GUI.enabled = _unibill;
		_self.androidUnibill = EditorGUILayout.Toggle("Android Unibill Billing", _self.androidUnibill);
		EditorGUILayout.EndVertical();

		GUILayout.Space(12);
		
		EditorGUILayout.BeginVertical();
		GUI.enabled = _p31iOS;
		_self.iosStoreKit = EditorGUILayout.Toggle("iOS Prime31 Billing", _self.iosStoreKit);

		GUI.enabled = _unibill;
		_self.iosUnibill = EditorGUILayout.Toggle("iOS Unibill Billing", _self.iosUnibill);
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		GUI.enabled = true;

		CheckToggle(_self.androidIAB, oldAndroidIAB, BuildTargetGroup.Android, "USING_PRIME31_ANDROID");
		CheckToggle(_self.androidUnibill, oldandroidUnibill, BuildTargetGroup.Android, "USING_UNIBILL_ANDROID");
		CheckToggle(_self.iosStoreKit, oldiosStoreKit, BuildTargetGroup.iPhone, "USING_PRIME31_IOS");
		CheckToggle(_self.iosUnibill, oldiosUnibill, BuildTargetGroup.iPhone, "USING_UNIBILL_IOS");
#endif
		GUILayout.Space(4);

		if(_iconFoldout = EditorGUILayout.Foldout(_iconFoldout, "Android notification icon"))
		{
			if(_icon == null)
			{
				_icon = new Texture2D(ICON_WIDTH, ICON_HEIGHT);
				_icon.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + ICON_PATH));
			}

			string path = Application.dataPath + ICON_PATH;

			GUILayout.Space(10);
			
			if(GUILayout.Button("Click to select icon:", EditorStyles.label))
			{
				_newIconPath = EditorUtility.OpenFilePanel("Choose icon", Application.dataPath, "png");
			}

			GUILayout.BeginHorizontal();
			if(GUILayout.Button(_icon, EditorStyles.objectFieldThumb, GUILayout.Height(75), GUILayout.Width(75)))
			{
				_newIconPath = EditorUtility.OpenFilePanel("Choose icon", Application.dataPath, "png");
			}

			if(_error == null)
				EditorGUILayout.HelpBox("Your texture must be " + ICON_WIDTH + "x" + ICON_HEIGHT + " pixels", MessageType.None);
			else
				EditorGUILayout.HelpBox(_error, MessageType.Error);

			GUILayout.EndHorizontal();

			if(!string.IsNullOrEmpty(_newIconPath) && _newIconPath != path)
			{
				try
				{
					var bytes = System.IO.File.ReadAllBytes(_newIconPath);
					string header = System.Text.Encoding.ASCII.GetString(bytes, 1, 3);
					_icon.LoadImage(bytes);
					if((bytes[0] == 'J' && header == "FIF") || (bytes[0] == (byte)0x89 && header == "PNG"))
					{
						if(_icon.height == ICON_HEIGHT && _icon.width == ICON_WIDTH)
						{
							System.IO.File.WriteAllBytes(path, _icon.EncodeToPNG());
							_newIconPath = null;
							_error = null;
						}
						else
						{
							_error = "The image is not " + ICON_WIDTH + "x" + ICON_HEIGHT + " pixels.";
						}
					}
					else
					{
						_error = "File is not a valid image.";
					}
				}
				catch
				{
					_error = "File could not be read.";
				}
			}
			else
			{
				_newIconPath = null;
				_error = null;
			}
		}
		else if(_icon != null)
		{
			DestroyImmediate(_icon);
			_icon = null;
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	private bool DoClassesExist(params string[] classes)
	{
		var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			select type.Name;

		return (classes == null)
			|| (classes.Length == 0)
			|| (classes.Length == 1 && types.Contains(classes[0]))
			|| (classes.Length == classes.Intersect(types).Distinct().Count());
	}

#if !UNITY_3_5
	private void CheckToggle(bool newVal, bool oldVal, BuildTargetGroup buildGroup, string tag)
	{
		if(oldVal != newVal)
		{
			string oldDef = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
			
			if(oldDef.Contains(tag) && !newVal)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, string.Join(";", oldDef.Split(';').Where(s => s != tag).ToArray()));
			}
			else if(!oldDef.Contains(tag) && newVal)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, tag + (oldDef.Length != 0 ? ";" + oldDef : ""));
			}
		}
	}
#endif


	[MenuItem ("FuseSDK/Regenerate Prefab", false, 0)]
	static void RegeneratePrefab()
	{
		var oldFuse = AssetDatabase.LoadAssetAtPath("Assets/FuseAPI/FuseSDK.prefab", typeof(FuseAPI)) as FuseAPI;
		
		// re-create the prefab
		Debug.Log("Creating new prefab...");
		GameObject temp = new GameObject();
		
		// add script components
		Debug.Log("Adding script components...");

#if UNITY_3_5
		temp.active = true;
#else
		temp.SetActive(true);
#endif


		var newFuse = temp.AddComponent<FuseAPI>();

		if(oldFuse != null)
		{
			// copy fields
			Debug.Log("Copying settings into new prefab...");

			newFuse.AndroidGameID = oldFuse.AndroidGameID;
			newFuse.iOSGameID = oldFuse.iOSGameID;
			newFuse.GCM_SenderID = oldFuse.GCM_SenderID;

			DestroyImmediate(oldFuse, true);
		}
		
		// delete the prefab
		Debug.Log("Deleting old prefab...");
		AssetDatabase.DeleteAsset("Assets/FuseAPI/FuseSDK.prefab");
		
		// save the prefab
		Debug.Log("Saving new prefab...");
		PrefabUtility.CreatePrefab("Assets/FuseAPI/FuseSDK.prefab", temp);
		DestroyImmediate (temp); // Clean up our Object
		AssetDatabase.SaveAssets();
	}

	[MenuItem("FuseSDK/Update Android Manifest", false, 1)]
	public static void UpdateAndroidManifest()
	{
		FusePostProcess.UpdateAndroidManifest(PlayerSettings.bundleIdentifier);
	}

	[MenuItem("FuseSDK/Open Documentation", false, 20)]
	static void OpenDocumentation()
	{
		Application.OpenURL(@"http://wiki.adrally.com/index.php/Unity");
	}
	
	[MenuItem("FuseSDK/Open GitHub Project", false, 21)]
	static void GoToGitHUb()
	{
		Application.OpenURL(FuseAPIUpdater.LATEST_SDK_URL);
	}
	
	[MenuItem("FuseSDK/Check For Updates", false, 40)]
	static void CheckForUpdate()
	{
		FuseAPIUpdater.CheckForUpdates(true);
	}
}

public class FuseSDKPrefs : EditorWindow
{
	public enum DownloadType
	{
		AutoDownloadAndImport = 0,
		AutoDownloadAndPromtForImport = 1,
		AutoDownloadOnly = 2,
		GoToWebsite = 3,
		AskEverytime = 4,
	}

	enum UpdateType
	{
		Never = 0,
		MajorReleases = 1,
		MinorReleases = 2,
		Bugfixes = 3,
	}

	[MenuItem("FuseSDK/Preferences", false, 60)]
	static void Init()
	{
		FuseSDKPrefs me = GetWindowWithRect(typeof(FuseSDKPrefs), new Rect(0, 0, 400, 60), true, "Fuse SDK Preferences") as FuseSDKPrefs;
		me.ShowUtility();
	}

	void OnGUI()
	{
		UpdateType updateStream = (UpdateType)Mathf.Min(EditorPrefs.GetInt(FuseAPIUpdater.AUTOUPDATE_KEY, 4) + 1, (int)UpdateType.Bugfixes);
		DownloadType autoDL = (DownloadType)EditorPrefs.GetInt(FuseAPIUpdater.AUTODOWNLOAD_KEY, 1);

		UpdateType newStream = (UpdateType)EditorGUILayout.EnumPopup("Auto Update Checking", updateStream);
		
		if(updateStream != newStream)
			EditorPrefs.SetInt(FuseAPIUpdater.AUTOUPDATE_KEY, ((int)newStream) - 1);

		if(newStream != UpdateType.Never)
		{
			EditorGUILayout.Space();
			DownloadType newDL = (DownloadType)EditorGUILayout.EnumPopup("'Download now' action", autoDL);

			if(newDL != autoDL)
				EditorPrefs.SetInt(FuseAPIUpdater.AUTODOWNLOAD_KEY, (int)newDL);
		}
	}
}