using UnityEngine;
using System.Collections;

public class ReticleGazeController : MonoBehaviour
{
	const float velocity = 8f;

	Vector3 targetPos;

	void Awake ()
	{
		gameObject.SetActive (false);
	}

	void Update ()
	{
		transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, Time.deltaTime * velocity);
	}

	public void SetTargetPosition (Position position)
	{
		if (position != null) {
			targetPos = position.AsGridLocalPosition (Marker.Target);
		}
	}

}
