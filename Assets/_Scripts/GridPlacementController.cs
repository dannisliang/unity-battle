using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridPlacementController : MonoBehaviour
{
	public GameObject tilePrefab;

	float tileColliderHeight = .2f;

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
		Vector3 pos = new Vector3 (0f, 0f, .5f * tileColliderHeight);
		Vector3 scale = new Vector3 (1f, 1f, tileColliderHeight);
		for (int x = 0; x < Utils.GRID_SIZE; x++) {
			for (int z = 0; z < Utils.GRID_SIZE; z++) {
				Position position = new Position (x, z);
				pos.x = x + .5f;
				pos.y = Utils.GRID_SIZE - z - .5f;

				GameObject clone = Instantiate (tilePrefab) as GameObject;
				clone.transform.SetParent (transform, false);
				clone.transform.localPosition = pos;
				clone.transform.localScale = scale;

				TileController tileController = clone.GetComponent<TileController> ();
				tileController.SetPosition (position);

#if UNITY_EDITOR
				Undo.RegisterCreatedObjectUndo (clone, "Create " + clone);
#endif
			}
		}
	}

}
