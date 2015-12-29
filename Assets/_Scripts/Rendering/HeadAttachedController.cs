using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HeadAttachedController : MonoBehaviour
{

	Transform targetTransform;

	void Awake ()
	{
		targetTransform = Camera.main.transform;
	}

	void Update ()
	{
		transform.position = targetTransform.position;
		transform.rotation = targetTransform.rotation;
	}
}
