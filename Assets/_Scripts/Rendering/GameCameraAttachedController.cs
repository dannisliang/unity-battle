using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameCameraAttachedController : MonoBehaviour
{
	public Camera gameCamera;

	Transform targetTransform;

	void Awake ()
	{
		targetTransform = gameCamera.transform;
	}

	void Update ()
	{
		transform.position = targetTransform.position;
		transform.rotation = targetTransform.rotation;
	}
}
