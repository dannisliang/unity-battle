using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{

	public int gridSize = 10;
	public GameObject gridPrefab;

	public Color color1 = new Color (1f, 0f, 0f, .4f);
	public Color color2 = new Color (0f, 1f, 0f, .4f);

	void Start ()
	{	
		Quaternion rot = Quaternion.FromToRotation (Vector3.up, Vector3.back);
		GameObject grid1 = Instantiate (gridPrefab) as GameObject;
		GameObject grid2 = Instantiate (gridPrefab, new Vector3 (0f, 0.2f, gridSize), rot) as GameObject;
		grid1.name = "Grid (Mine)";
		grid2.name = "Grid (Theirs)";
		grid1.GetComponent<GridHolderController> ().RecreateGrid (gridSize, Color.red);
		grid2.GetComponent<GridHolderController> ().RecreateGrid (gridSize, Color.green);
	}
	
}
