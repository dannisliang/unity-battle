using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;

public class AnalyticsAssistant : MonoBehaviour
{
	//	static string CATEGORY = typeof(AnalyticsAssistant).Name;

	public static AnalyticsAssistant INSTANCE { get; private set; }

	public static GoogleAnalyticsV4 gav4;

	void Awake ()
	{
		if (INSTANCE != null && INSTANCE != this) {
			Destroy (gameObject);
			return;
		}
		INSTANCE = this;
		gav4 = GetComponent<GoogleAnalyticsV4> ();

		bool debug = Application.installMode == ApplicationInstallMode.DeveloperBuild || Application.installMode == ApplicationInstallMode.Editor;
		Debug.Log ("Application.installMode = " + Application.installMode);

//		gav4.dryRun = debug;
//		Debug.Log ("gav4.dryRun = " + gav4.dryRun);

//		gav4.logLevel = debug ? GoogleAnalyticsV4.DebugMode.VERBOSE : GoogleAnalyticsV4.DebugMode.WARNING;
//		gav4.logLevel = GoogleAnalyticsV4.DebugMode.VERBOSE;
		gav4.logLevel = GoogleAnalyticsV4.DebugMode.WARNING;
		Debug.Log ("gav4.logLevel = " + gav4.logLevel);

		#if UNITY_EDITOR
		if (Application.isEditor) {
			gav4.SetOnTracker (Fields.CLIENT_ID, "EDITOR");
		}

		if (!gav4.bundleVersion.Equals (PlayerSettings.bundleVersion)) {
			throw new Exception (
				typeof(AnalyticsAssistant).Name + ".bundleVersion (" + gav4.bundleVersion + ") " +
				"!= " + typeof(PlayerSettings).Name + ".bundleVersion (" + PlayerSettings.bundleVersion + ")" +
				"\nExit play mode, click on " + typeof(AnalyticsAssistant).Name + " game object, then save scene to fix this");
		}
		#endif
		var bundleVersion = gav4.bundleVersion + "-" + (debug ? "debug" : "prod");
		gav4.SetOnTracker (Fields.APP_VERSION, bundleVersion);
		Debug.Log ("bundleVersion=" + bundleVersion);

		if (debug) {
			gav4.SetOnTracker (Fields.DEVELOPER_ID, "fredsa");
		}

		if (!Application.isEditor) {
			gav4.SetOnTracker (Fields.SCREEN_RESOLUTION, Screen.width + "x" + Screen.height + " " + Screen.dpi + "DPI");
		}
	}

}
