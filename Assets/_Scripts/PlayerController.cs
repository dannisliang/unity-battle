using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
	public GameObject reticlePrefab;
	public GameObject rocketPrefab;

	GameController gameController;
	GameObject reticle;
	LayerMask layerMaskTileTheirs;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		layerMaskTileTheirs = LayerMask.GetMask ("Tile Theirs");

		reticle = Instantiate (reticlePrefab);
		reticle.transform.SetParent (transform, false);
	}

	void Update ()
	{	
		if (Input.GetButton ("Fire1")) {
			gameController.StartNewGame ();
		}

		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 100f);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100f, layerMaskTileTheirs)) {
			reticle.transform.position = hit.point - Camera.main.transform.forward * .1f;
			FireAt (hit.collider.transform);
		}
	}

	void FireAt (Transform targetTransform)
	{
		if (Time.frameCount % 120 != 0)
			return;
		GameObject rocket = Instantiate (rocketPrefab);
		rocket.transform.SetParent (transform, false);
		rocket.GetComponent<RocketController> ().Launch (Camera.main.transform, targetTransform);
	}

}
