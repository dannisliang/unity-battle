using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour
{
	ParticleSystem flameParticleSystem;
	//	Transform targetTransform;
	float velocity = 1.5f;

	void Awake ()
	{
		GameController.instance.SetIsFiring (true);
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
	}

	void Update ()
	{
		if (Input.GetButtonUp ("Fire1")) {
			Time.timeScale = 10f;
		}
	}

	public void Launch (Transform origin, Transform targetTransform)
	{
//		this.targetTransform = targetTransform;
		transform.position = origin.position;
		transform.rotation = origin.rotation;

		Vector3 direction = targetTransform.position - origin.position;
		GetComponent<Rigidbody> ().velocity = direction.normalized * velocity;
	}

	void OnDestroy ()
	{
		GameController.instance.SetIsFiring (false);
	}

	void OnTriggerEnter (Collider other)
	{
		Time.timeScale = 1f;
		if (other.gameObject.layer == GameController.layerTileTheirs.layer) {
			//TileController tileController = other.gameObject.GetComponent<TileController> ();
			GameController.instance.PlayPlop ();
			flameParticleSystem.Stop ();
			transform.GetChild (0).gameObject.SetActive (false);
			Destroy (gameObject, flameParticleSystem.duration);
			return;
		}

		if (other.gameObject.layer == GameController.layerBoatTheirs.layer) {
			BoatController boatController = other.gameObject.GetComponentInParent<BoatController> ();
			Debug.Log ("HIT " + boatController.boat);
			boatController.Hit ();
		}
	}

}
