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
		BattleshipController.instance.SetIsFiring (true);
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

	public void MaybeLaunch (Transform origin, Transform targetTransform)
	{
//		this.targetTransform = targetTransform;
		transform.position = origin.position;
		transform.rotation = origin.rotation;

		Vector3 direction = targetTransform.position - origin.position;
		GetComponent<Rigidbody> ().velocity = direction.normalized * velocity;
	}

	void OnDestroy ()
	{
		BattleshipController.instance.SetIsFiring (false);
	}

	void OnTriggerEnter (Collider other)
	{
		// prevent additional collisions
		GetComponent<Collider> ().enabled = false;

		// restore time scale
		Time.timeScale = 1f;
		this.other = other;

		Position position = other.GetComponent<PositionMarkerController> ().position;
		RealtimeBattleship.EncodeAndSend (position);
		
		Invoke ("Explode", .3f * flameParticleSystem.duration);
		FizzleOut (flameParticleSystem.duration);
	}

	void Explode ()
	{
		if (other.gameObject.layer == BattleshipController.layerTileTheirs.layer) {
			PositionMarkerController positionMakerController = other.gameObject.GetComponent<PositionMarkerController> ();
			//			TileController tileController = other.gameObject.GetComponent<TileController> ();
			BattleshipController.instance.Strike (true, positionMakerController.position);
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
