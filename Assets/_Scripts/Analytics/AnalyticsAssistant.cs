using UnityEngine;
using System.Collections;

public class AnalyticsAssistant : MonoBehaviour
{
	static string CATEGORY = typeof(AnalyticsAssistant).Name;

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

		if (Application.isEditor) {
			gav4.SetOnTracker (Fields.CLIENT_ID, "EDITOR");
		}

		gav4.SetOnTracker (Fields.APP_VERSION, gav4.bundleVersion + "-" + (debug ? "debug" : "prod"));

		if (debug) {
			gav4.SetOnTracker (Fields.DEVELOPER_ID, "fredsa");
		}

		gav4.SetOnTracker (Fields.SCREEN_RESOLUTION, Screen.width + "x" + Screen.height + " " + Screen.dpi + "DPI");

		gav4.LogEvent (CATEGORY, "Init", null, 0);
	}

}
