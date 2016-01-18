using UnityEngine;
using System.Collections;

public class TrackMainCameraController : MonoBehaviour
{

	void Update ()
	{
		transform.localRotation = Camera.main.transform.localRotation;
	}
}
