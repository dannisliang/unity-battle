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

		t0 = Time.time;

		Vector3 aim = end.position - start.position;

		bezier.t0.position = start.position;
		bezier.t0.rotation = start.rotation;

		bezier.t3.position = end.position;
		bezier.t3.rotation = end.rotation;
		
		bezier.t1.position = start.position + .3f * aim + Deviation (bezier.t0);
		bezier.t1.rotation = start.rotation;

		bezier.t2.position = end.position - .3f * aim + Deviation (bezier.t3);
		bezier.t2.rotation = end.rotation;

		transform.position = start.position;
		transform.rotation = start.rotation;
	}

	Vector3 Deviation (Transform transform)
	{
		return Utils.RandomSign () * UnityEngine.Random.Range (4f, 6f) * transform.right
		+ Utils.RandomSign () * UnityEngine.Random.Range (4f, 6f) * transform.up;
	}

	void OnDestroy ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Destroy (bezier.gameObject);
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
