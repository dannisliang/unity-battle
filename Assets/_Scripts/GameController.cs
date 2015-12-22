using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class GameController : MonoBehaviour
{
	public static GameController instance;
	public static LayerInfo layerTileTheirs;
	public static LayerInfo layerBoatTheirs;

	public GameObject boatHitPrefab;
	public GameObject gridTheirs;
	public GameObject reticle;
	public BoatPlacementController boatPlacementController;
	public AudioClip plop;

	bool firing;
	AudioSource source;

	void Awake ()
	{
		if (instance != null && instance != this) {
			Destroy (this);
			return;
		}
		instance = this;
		layerTileTheirs = new LayerInfo ("Tile Theirs");
		layerBoatTheirs = new LayerInfo ("Boat Theirs");
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

	public bool IsFiring ()
	{
		return firing;
	}

	public void SetIsFiring (bool firing)
	{
		this.firing = firing;
		reticle.SetActive (!firing);
	}

}
