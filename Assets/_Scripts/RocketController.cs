using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour
{
	Transform targetTransform;
	float velocity = 4f;

	public void Launch (Transform origin, Transform targetTransform)
	{
		this.targetTransform = targetTransform;
		transform.position = origin.position;
		transform.rotation = origin.rotation;

		Vector3 direction = targetTransform.position - origin.position;
		GetComponent<Rigidbody> ().velocity = direction.normalized * velocity;
	}

	void OnCollisionEnter (Collision collision)
	{
		Debug.Log (collision.gameObject.name);
	}

}
