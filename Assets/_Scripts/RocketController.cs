using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour
{
	ParticleSystem flameParticleSystem;
	Transform targetTransform;
	float velocity = 1.5f;

	void Awake ()
	{
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
	}

	public void Launch (Transform origin, Transform targetTransform)
	{
		this.targetTransform = targetTransform;
		transform.position = origin.position;
		transform.rotation = origin.rotation;

		Vector3 direction = targetTransform.position - origin.position;
		GetComponent<Rigidbody> ().velocity = direction.normalized * velocity;
	}

	void OnTriggerEnter (Collider other)
	{
		TileController tileController = other.gameObject.GetComponent<TileController> ();
		if (tileController == null) {
			return;
		}
		GameController.instance.PlayPlop ();
		flameParticleSystem.Stop ();
		transform.GetChild (0).gameObject.SetActive (false);
		Destroy (gameObject, flameParticleSystem.duration);
	}

}
