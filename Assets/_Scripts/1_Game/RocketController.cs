using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof(CardboardAudioSource))]
public class RocketController : MonoBehaviour
{
	float rocketFlightTime;

	public BezierController bezierPrefab;

	ParticleSystem flameParticleSystem;
	CardboardAudioSource source;
	float[] fizzleOutTimes;
	Action callback;
	float t0;
	BezierController bezier;
	Whose atWhose;
	Position targetPosition;

	void Awake ()
	{
		flameParticleSystem = GetComponentInChildren<ParticleSystem> ();
		source = GetComponent<CardboardAudioSource> ();

		bezier = Instantiate (bezierPrefab);
		bezier.name += "(" + gameObject.name + ")";
	}

	void Update ()
	{
		float t = (Time.time - t0) / rocketFlightTime;
		Vector3 position = bezier.GetPoint (t);
		Vector3 velocity = bezier.GetVelocity (t);
		transform.position = position;
		if (velocity.sqrMagnitude > 0f) {
			transform.rotation = Quaternion.LookRotation (velocity);
		}

		if (fizzleOutTimes == null) {
			if (t >= 1f) {
				BattleController.instance.Strike (atWhose, targetPosition);
				FizzleOut (Mathf.Max (.3f, .3f * rocketFlightTime));
				if (callback != null) {
					callback ();
				}
			}
		}

		if (fizzleOutTimes != null) {
			source.volume = (fizzleOutTimes [1] - Time.time) / (fizzleOutTimes [1] - fizzleOutTimes [0]);
		}
	}

	public void Launch (Whose atWhose, Position targetPosition, PosRot start, PosRot end, float flightTime, Action callback)
	{
		this.callback = callback;
		this.atWhose = atWhose;
		this.targetPosition = targetPosition;
		this.rocketFlightTime = flightTime;

		t0 = Time.time;
		float distance = Vector3.Distance (start.position, end.position);

		bezier.t0.position = start.position;
		bezier.t0.rotation = start.rotation;

		bezier.t3.position = end.position;
		bezier.t3.rotation = end.rotation;
		
		bezier.t1.position = start.position + .3f * distance * bezier.t0.forward + Deviation (bezier.t0);
		bezier.t1.rotation = start.rotation;

		bezier.t2.position = end.position - .3f * distance * bezier.t3.forward;
		bezier.t2.rotation = end.rotation;

		transform.position = start.position;
		transform.rotation = start.rotation;
	}

	Vector3 Deviation (Transform transform)
	{
		return Utils.RandomSign () * UnityEngine.Random.Range (3f, 4f) * transform.right
		+ Utils.RandomSign () * UnityEngine.Random.Range (3f, 4f) * transform.up;
	}

	void OnDestroy ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Destroy (bezier.gameObject);
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
