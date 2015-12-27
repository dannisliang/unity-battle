using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class RocketController : MonoBehaviour
{
	ParticleSystem flameParticleSystem;
	AudioSource source;
	//	Transform targetTransform;
	float velocity = 1.5f;
	float[] fizzleOutTimes;
	Collider other;

	void Awake ()
	{
		GameController.instance.SetIsFiring (true);
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
		source = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		if (Input.GetButtonUp ("Fire1")) {
			Time.timeScale = 10f;
		}
		if (fizzleOutTimes != null) {
			source.volume = (fizzleOutTimes [1] - Time.time) / (fizzleOutTimes [1] - fizzleOutTimes [0]);
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
		// prevent additional collisions
		GetComponent<Collider> ().enabled = false;

		// restore time scale
		Time.timeScale = 1f;
		this.other = other;

		Invoke ("Explode", .3f * flameParticleSystem.duration);
		FizzleOut (flameParticleSystem.duration);
	}

	void Explode ()
	{
		if (other.gameObject.layer == GameController.layerTileTheirs.layer) {
			PositionMakerController positionMakerController = other.gameObject.GetComponent<PositionMakerController> ();
			//			TileController tileController = other.gameObject.GetComponent<TileController> ();
			GameController.instance.PlayWaterPlop ();
			GameController.instance.PlaceMarker (positionMakerController.position, false);
		} else if (other.gameObject.layer == GameController.layerBoatTheirs.layer) {
			PositionMakerController positionMakerController = other.gameObject.GetComponent<PositionMakerController> ();
			//			BoatController boatController = other.gameObject.GetComponentInParent<BoatController> ();
			Debug.logger.Log ("HIT " + positionMakerController.position);
			//			boatController.Hit (positionMakerController.position);
			GameController.instance.PlayShipExplosionAfter (1f);
			GameController.instance.PlaceMarker (positionMakerController.position, true);
		} else {
			Debug.LogError ("Unexpected collision with " + other);
		}
	}

	void FizzleOut (float duration)
	{
		fizzleOutTimes = new float[]{ Time.time, Time.time + duration };
		flameParticleSystem.Stop ();
		//disable mesh
		transform.GetChild (0).gameObject.SetActive (false);
		Destroy (gameObject, duration);
	}

}
