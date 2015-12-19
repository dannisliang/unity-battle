using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridPlacementController : MonoBehaviour
{
	public GameObject tilePrefab;

	public void Awake ()
	{
		RecreateGrid ();
	}

	public void RecreateGrid ()
	{
		Utils.DestroyChildren (transform);
		CreateGrid ();
	}

	void CreateGrid ()
	{
		Vector3 pos = Vector3.zero;
		for (int x = 0; x < Utils.GRID_SIZE; x++) {
			for (int z = 0; z < Utils.GRID_SIZE; z++) {
				Position position = new Position (x, z);
				pos.x = (x - Utils.GRID_SIZE / 2f) + .5f;
				pos.z = (Utils.GRID_SIZE - 1 - z) + .5f;

				GameObject clone = Instantiate (tilePrefab, pos, Quaternion.identity) as GameObject;
				clone.transform.SetParent (transform, false);

				TileController tileController = clone.GetComponent<TileController> ();
				tileController.SetPosition (position);
			}
		}
	}

}
