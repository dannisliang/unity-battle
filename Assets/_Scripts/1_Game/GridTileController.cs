﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridTileController : MonoBehaviour
{
	public GameObject tilePrefab;

	float tileColliderHeight = .2f;

	public void OnEnable ()
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
		for (int x = 0; x < Utils.GRID_SIZE.x; x++) {
			for (int z = 0; z < Utils.GRID_SIZE.y; z++) {
				Position position = new Position (x, z);
				pos.x = x + .5f;
				pos.y = Utils.GRID_SIZE.y - z - .5f;

				GameObject clone = Game.InstantiateTemp (tilePrefab);
				clone.transform.SetParent (transform, false);
				clone.transform.localPosition = pos;
				clone.transform.localScale = scale;

				PositionMarkerController markerController = clone.GetComponent<PositionMarkerController> ();
				markerController.position = position;

				//Utils.SetNoSaveNoEditHideFlags (clone.transform);
#if UNITY_EDITOR
				Undo.RegisterCreatedObjectUndo (clone, "Create " + clone);
#endif
			}
		}
	}

}
