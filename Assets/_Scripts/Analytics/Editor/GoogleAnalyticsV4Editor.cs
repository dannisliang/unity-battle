using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

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
		bool dirty = false;
		dirty |= VerifyValue (ref gav4.androidTrackingCode, TRACKING_CODE);
		dirty |= VerifyValue (ref gav4.IOSTrackingCode, TRACKING_CODE);
		dirty |= VerifyValue (ref gav4.otherTrackingCode, TRACKING_CODE);
		dirty |= VerifyValue (ref gav4.productName, PlayerSettings.productName);
		dirty |= VerifyValue (ref gav4.bundleIdentifier, PlayerSettings.bundleIdentifier);
		dirty |= VerifyValue (ref gav4.bundleVersion, PlayerSettings.bundleVersion);
		if (dirty) {
			Debug.LogWarning ("***" + target.name + " updated; Marking scene dirty");
			serializedObject.ApplyModifiedProperties ();
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}
	}

	bool VerifyValue (ref string field, string value)
	{
		if (!field.Equals (value)) {
			field = value;
			return true;
		}
		return false;
	}

}
