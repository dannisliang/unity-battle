using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[ExecuteInEditMode]
public class GridHolderController : MonoBehaviour
{
	public int gridSize = 8;
	public GameObject tilePrefab;

	void OnValidate ()
	{
//		Debug.Log ("OnValidate()");
//		RecreateGrid ();
	}

	public void RecreateGrid ()
	{
		Utils.DestroyChildren (transform);
		CreateChildren ();
	}

	void CreateChildren ()
	{
		Vector3 pos = Vector3.zero;
		for (int j = 0; j < gridSize; j++) {
			for (int i = 0; i < gridSize; i++) {
				pos.x = (gridSize - i) + .5f;
				pos.z = (gridSize - j) + .5f;
				GameObject clone = Instantiate (tilePrefab, pos, Quaternion.identity) as GameObject;
				clone.transform.SetParent (transform, false);
				string col = "" + (char)(65 + j);
				string row = "" + (i + 1);
				clone.name += " " + col + row;

				clone.GetComponentInChildren<TextMesh> ().text = col + row;
			}
		}
	}

}
