using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(GoogleAnalyticsV4))]
public class GoogleAnalyticsV4Editor : Editor
{
	
	const string TRACKING_CODE = "UA-75513401-1";

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		CheckValues ();
	}

	void CheckValues ()
	{
		GoogleAnalyticsV4 gav4 = (GoogleAnalyticsV4)target;
		gav4.androidTrackingCode = TRACKING_CODE;
		gav4.IOSTrackingCode = TRACKING_CODE;
		gav4.otherTrackingCode = TRACKING_CODE;
		gav4.productName = PlayerSettings.productName;
		gav4.bundleIdentifier = PlayerSettings.bundleIdentifier;
		gav4.bundleVersion = PlayerSettings.bundleVersion;
		serializedObject.ApplyModifiedProperties ();
	}

}
