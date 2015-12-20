﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour
{
	public static GameController instance;
	public GameObject reticle;

	public BoatPlacementController boatPlacementController;
	public AudioClip plop;

	AudioSource source;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (this);
			return;
		}
		instance = this;
		source = GetComponent<AudioSource> ();
	}

	public void StartNewGame ()
	{
		boatPlacementController.RecreateBoats ();
	}

	public void PlayPlop ()
	{
		source.PlayOneShot (plop);
	}

	public void SetReticleActive (bool active)
	{
		reticle.SetActive (active);
	}

}
