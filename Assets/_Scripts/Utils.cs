﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utils : MonoBehaviour
{
	public static int ignoreCurrentFire1FrameCount;
	public static int GRID_SIZE = 10;
	public static HideFlags NO_SAVE_NO_EDIT_HIDE_FLAGS = HideFlags.DontSave | HideFlags.NotEditable;

	public static void SetNoSaveNoEditHideFlags (Transform parent)
	{
		parent.gameObject.name = "[NO SAVE] " + parent.gameObject.name;
		parent.gameObject.hideFlags = NO_SAVE_NO_EDIT_HIDE_FLAGS;
		foreach (Transform t in parent.transform) {
			SetNoSaveNoEditHideFlags (t);
		}
	}

	public static void DestroyChildren (Transform transform)
	{
		for (int i = transform.childCount - 1; i >= 0; i--) {
			Transform t = transform.GetChild (i);
			if (Application.isPlaying) {
				Destroy (t.gameObject);
			} else {
//				DestroyImmediate (t.gameObject);
#if UNITY_EDITOR
				Undo.DestroyObjectImmediate (t.gameObject);
#else
#endif
			}
		}
	}

	public static void IgnoreCurrentFire1 ()
	{
		ignoreCurrentFire1FrameCount = Time.frameCount;
	}

	public static bool DidFire ()
	{
		return Input.GetButtonUp ("Fire1") && ignoreCurrentFire1FrameCount != Time.frameCount;
	}

}
