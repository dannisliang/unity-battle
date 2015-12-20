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
		Vector3 scale = Vector2.one / (float)Utils.GRID_SIZE;
		for (int x = 0; x < Utils.GRID_SIZE; x++) {
			for (int z = 0; z < Utils.GRID_SIZE; z++) {
				Position position = new Position (x, z);
				pos.x = x / (float)Utils.GRID_SIZE - .5f + .5f / (float)Utils.GRID_SIZE;
				pos.y = z / (float)Utils.GRID_SIZE - .5f + .5f / (float)Utils.GRID_SIZE;

				GameObject clone = Instantiate (tilePrefab) as GameObject;
				clone.transform.SetParent (transform, false);
				clone.transform.localPosition = pos;
				clone.transform.localScale = scale;

				TileController tileController = clone.GetComponent<TileController> ();
				tileController.SetPosition (position);
			}
		}
	}

}
