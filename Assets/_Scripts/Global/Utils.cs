﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utils : MonoBehaviour
{
	public const float RESTART_DELAY = 10f;
	public const float DUMMY_PLAY_GAMES_REAL_TIME_ASYNC_DELAY = .1f;
	public const float DUMMY_PLAY_GAMES_REAL_TIME_REPLAY_DELAY = .1f;
	public const float AI_DELAY = 5f;

	public const int GRID_SIZE = 3;
	public const float BOAT_HEIGHT = .01f;
	public const float CLEARANCE_HEIGHT = .001f;

	public const HideFlags NO_SAVE_NO_EDIT_HIDE_FLAGS = HideFlags.DontSave | HideFlags.NotEditable;

	public static float RandomSign ()
	{
		return UnityEngine.Random.value > .5f ? 1f : -1f;
	}

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

	public static bool DidFire ()
	{
		return Input.GetButtonDown ("Fire1");
	}

}
