using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class RocketController : MonoBehaviour
{
	const float DURATION = 3f;

	public BezierController bezierPrefab;

	ParticleSystem flameParticleSystem;
	AudioSource source;
	float[] fizzleOutTimes;
	Collider other;
	Action callback;
	float t0;
	BezierController bezier;

	void Awake ()
	{
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
		source = GetComponent<AudioSource> ();

		bezier = Instantiate (bezierPrefab);
		bezier.name += "(" + gameObject.name + ")";
	}

	void Update ()
	{
		float t = (Time.time - t0) / DURATION;
		Vector3 position = bezier.GetPoint (t);
		Vector3 velocity = bezier.GetVelocity (t);
		transform.position = position;
		if (velocity.sqrMagnitude > 0f) {
			transform.rotation = Quaternion.LookRotation (velocity);
		}

		if (fizzleOutTimes != null) {
			source.volume = (fizzleOutTimes [1] - Time.time) / (fizzleOutTimes [1] - fizzleOutTimes [0]);
		}
	}

	public void Launch (PosRot start, PosRot end, Action callback)
	{
		this.callback = callback;

		bezier.pr0 = start;
		bezier.pr1 = start;
		bezier.pr2 = end;
		bezier.pr3 = end;

		t0 = Time.time;
		transform.position = start.position;
	}

	void OnDestroy ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Destroy (bezier);
		if (callback != null) {
			callback ();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		// prevent additional collisions
		GetComponent<Collider> ().enabled = false;

		this.other = other;

		Invoke ("Explode", .3f * flameParticleSystem.duration);
		FizzleOut (flameParticleSystem.duration);
	}

	void Explode ()
	{
		if (other.gameObject.layer == BattleController.layerTileTheirs.layer) {
			PositionMarkerController positionMakerController = other.gameObject.GetComponent<PositionMarkerController> ();
			//			TileController tileController = other.gameObject.GetComponent<TileController> ();
			BattleController.instance.Strike (Whose.Theirs, positionMakerController.position);
			RealtimeBattle.EncodeAndSendHit (positionMakerController.position);
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
