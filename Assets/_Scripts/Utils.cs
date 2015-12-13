using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{

	public static int GRID_SIZE = 10;

	public static void DestroyChildren (Transform transform)
	{
		for (int i = transform.childCount - 1; i >= 0; i--) {
			Transform t = transform.GetChild (i);
			if (Application.isPlaying) {
				Destroy (t.gameObject);
			} else {
				DestroyImmediate (t.gameObject);
			}
		}
	}
}
