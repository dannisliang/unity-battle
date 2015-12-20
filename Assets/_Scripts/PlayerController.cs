using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
	public GameObject reticlePrefab;
	public GameObject rocketPrefab;

	LayerMask layerMaskTileTheirs;
	TileController currentTileController;

	void Start ()
	{
		layerMaskTileTheirs = LayerMask.GetMask ("Tile Theirs");
	}

	void Update ()
	{	
		if (Input.GetKey (KeyCode.R)) {
			GameController.instance.StartNewGame ();
		}

		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
#if UNITY_EDITOR
		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 13f);
#endif
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100f, layerMaskTileTheirs)) {
			SetCurrentTileController (hit.collider.GetComponent<TileController> ());
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
