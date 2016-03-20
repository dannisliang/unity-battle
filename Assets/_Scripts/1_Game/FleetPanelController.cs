﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetPanelController : MonoBehaviour
{
	public BoatPlacementController boatPlacementController;
	public GameObject shipPrefab;

	Color neutral;
	Color red = Color.red;

	GameObject[] ships;
	Vector3 startPos;

	void Awake ()
	{
		// Clockwise corner numbers 0-3, starting in bottom left
		Vector3[] fourCornersArray = new Vector3[4];
		GetComponent<RectTransform> ().GetWorldCorners (fourCornersArray);
		startPos = .5f * (fourCornersArray [1] + fourCornersArray [2]) + .3f * transform.forward;

		ships = new GameObject[Grid.fleet.Length];
		for (int i = 0; i < ships.Length; i++) {
			ships [i] = Instantiate (shipPrefab);
			ships [i].transform.position = startPos - (2f + i * 1.4f) * transform.up;
			ships [i].transform.rotation = Quaternion.FromToRotation (-ships [i].transform.forward, -transform.forward);
			ships [i].transform.localScale = new Vector3 ((float)Grid.fleet [i].size / 5f, 1f, 1f);
		}
		neutral = ships [0].gameObject.GetComponentInChildren<MeshRenderer> ().material.color;
	}

	void OnEnable ()
	{
		BattleController.instance.OnBattleState += UpdateSelf;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= UpdateSelf;
	}

	void UpdateSelf (Whose whoseTurn, bool firing, Whose loser)
	{
		Boat[] boats = boatPlacementController.grid.boats;
		if (boats != null) {
			for (int i = 0; i < boats.Length; i++) {
				Color color = GetColor (boats [i].HitCount (), boats [i].Size ());
				ships [i].gameObject.GetComponentInChildren<MeshRenderer> ().material.color = color;
			}
		}
	}

	Color GetColor (int hits, int size)
	{
		if (hits == size) {
			return Color.black;
		}
		float damage = (float)hits / (size - 1);
		return Color.Lerp (neutral, red, damage);
	}

}