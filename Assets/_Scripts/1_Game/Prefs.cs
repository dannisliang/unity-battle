using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Prefs
{
	public delegate void PrefChangedAction (bool vrMode);

	public static event PrefChangedAction OnVrModeChanged;

	public static bool VrMode {
		get {
			return PlayerPrefs.GetInt ("VrMode") == 1;
		}
		set {
			PlayerPrefs.SetInt ("VrMode", value ? 1 : 0);
			if (OnVrModeChanged != null) {
				OnVrModeChanged (value);
			}
		}
	}

}
