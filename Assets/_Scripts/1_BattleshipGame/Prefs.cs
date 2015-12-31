using UnityEngine;
using System.Collections;

public class Prefs
{

	public static bool VrMode {
		get {
			return PlayerPrefs.GetInt ("VrMode") == 1;
		}
		set {
			PlayerPrefs.SetInt ("VrMode", value ? 1 : 0);
		}
	}

}
