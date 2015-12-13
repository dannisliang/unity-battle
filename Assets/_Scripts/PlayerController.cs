using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{

	public int gridSize = 10;
	public GameObject gridPrefab;

	Color color1 = new Color (1f, .3f, 0f, .15f);
	Color color2 = new Color (.3f, .8f, .3f, .15f);

	void Start ()
	{	
		Quaternion rot = Quaternion.FromToRotation (Vector3.up, Vector3.back);
		GameObject grid1 = Instantiate (gridPrefab) as GameObject;
		GameObject grid2 = Instantiate (gridPrefab, new Vector3 (0f, 0.2f, gridSize), rot) as GameObject;
		grid1.name = "Grid (Mine)";
		grid2.name = "Grid (Theirs)";
		grid1.GetComponent<GridHolderController> ().RecreateGrid (gridSize, color1);
		grid2.GetComponent<GridHolderController> ().RecreateGrid (gridSize, color2);
	}
	
}
