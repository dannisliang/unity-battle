﻿using UnityEngine;
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
	Action callback;
	float t0;
	BezierController bezier;
	Whose atWhose;
	Position targetPosition;

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

		if (fizzleOutTimes == null) {
			if (t >= 1f) {
				Explode ();
				FizzleOut (flameParticleSystem.duration);
			}
		}

		if (fizzleOutTimes != null) {
			source.volume = (fizzleOutTimes [1] - Time.time) / (fizzleOutTimes [1] - fizzleOutTimes [0]);
		}
	}

	public void Launch (Whose atWhose, Position targetPosition, PosRot start, PosRot end, Action callback)
	{
		this.callback = callback;
		this.atWhose = atWhose;
		this.targetPosition = targetPosition;

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
		return Utils.RandomSign () * UnityEngine.Random.Range (6f, 8f) * transform.right
		+ Utils.RandomSign () * UnityEngine.Random.Range (6f, 8f) * transform.up;
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

	void Explode ()
	{
		if (atWhose == Whose.Theirs) {
			BattleController.instance.Strike (atWhose, targetPosition);
			RealtimeBattle.EncodeAndSendHit (targetPosition);
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
