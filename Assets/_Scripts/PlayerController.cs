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
	TileController currentTileController;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

		layerMaskTileTheirs = LayerMask.GetMask ("Tile Theirs");

		reticle = Instantiate (reticlePrefab);
		reticle.transform.SetParent (transform, false);
	}

	void Update ()
	{	
		if (Input.GetKey (KeyCode.R)) {
			gameController.StartNewGame ();
		}

		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 100f);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100f, layerMaskTileTheirs)) {
			SetCurrentTileController (hit.collider.GetComponent<TileController> ());
			reticle.transform.position = hit.point - Camera.main.transform.forward * .1f;
			if (Utils.DidFire ()) {
				FireAt (hit.collider.transform);
			}
		} else {
			SetCurrentTileController (null);
		}
	}

	void SetCurrentTileController (TileController tileController)
	{
		if (tileController == currentTileController) {
			return;
		}
		
		if (currentTileController != null) {
			currentTileController.Highlight (false);
		}

		if (tileController != null) {
			tileController.Highlight (true);
		}

		currentTileController = tileController;
	}

	void FireAt (Transform targetTransform)
	{
		GameObject rocket = Instantiate (rocketPrefab);
		rocket.transform.SetParent (transform, false);
		rocket.GetComponent<RocketController> ().Launch (Camera.main.transform, targetTransform);
	}

}
