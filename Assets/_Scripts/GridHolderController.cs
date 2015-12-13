using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[ExecuteInEditMode]
public class GridHolderController : MonoBehaviour
{
	public int gridSize = 10;
	public GameObject tilePrefab;
	public Material material;

	public void RecreateGrid ()
	{
		Utils.DestroyChildren (transform);
		CreateChildren ();
	}

	void CreateChildren ()
	{
		Vector3 pos = Vector3.zero;
		for (int x = 0; x < gridSize; x++) {
			for (int z = 0; z < gridSize; z++) {
				pos.x = (x - gridSize / 2f) + .5f;
				pos.z = (gridSize - 1 - z) + .5f;
				GameObject clone = Instantiate (tilePrefab, pos, Quaternion.identity) as GameObject;
				clone.transform.SetParent (transform, false);
				string col = "" + (char)(65 + x);
				string row = "" + (z + 1);
				clone.name += " " + col + row;

				clone.transform.GetChild (0).GetComponent<MeshRenderer> ().material = material;
//				clone.transform.GetChild (1).GetComponent<TextMesh> ().text = col + row;
			}
		}
	}

}
