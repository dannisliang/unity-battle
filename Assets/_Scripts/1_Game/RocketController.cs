using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class RocketController : MonoBehaviour
{
	ParticleSystem flameParticleSystem;
	AudioSource source;
	float velocity = 1.5f;
	float[] fizzleOutTimes;
	Collider other;
	Action callback;

	void Awake ()
	{
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
		source = GetComponent<AudioSource> ();
	}

	void Update ()
	{
		Time.timeScale = Mathf.Min (10f, Time.timeScale + 1.5f * Time.deltaTime);
		if (fizzleOutTimes != null) {
			source.volume = (fizzleOutTimes [1] - Time.time) / (fizzleOutTimes [1] - fizzleOutTimes [0]);
		}
	}

	public void Launch (Transform origin, Transform targetTransform, Action callback)
	{
		this.callback = callback;

		Vector3 direction = targetTransform.position - origin.position;
		transform.position = origin.position;
		transform.rotation = Quaternion.LookRotation (direction);
		GetComponent<Rigidbody> ().velocity = direction.normalized * velocity;
	}

	void OnDestroy ()
	{
		// restore time scale
		Time.timeScale = 1f;
		callback ();
	}

	void OnTriggerEnter (Collider other)
	{
		// prevent additional collisions
		GetComponent<Collider> ().enabled = false;

		this.other = other;

		Position position = other.GetComponent<PositionMarkerController> ().position;
		RealtimeBattle.EncodeAndSend (position);
		
		Invoke ("Explode", .3f * flameParticleSystem.duration);
		FizzleOut (flameParticleSystem.duration);
	}

	void Explode ()
	{
		if (other.gameObject.layer == BattleController.layerTileTheirs.layer) {
			PositionMarkerController positionMakerController = other.gameObject.GetComponent<PositionMarkerController> ();
			//			TileController tileController = other.gameObject.GetComponent<TileController> ();
			BattleController.instance.Strike (Whose.Theirs, positionMakerController.position);
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
