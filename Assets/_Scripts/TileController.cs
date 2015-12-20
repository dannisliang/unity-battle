using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
	Position position;
	//	Material material;
	//	Color color0, color1;

	void Awake ()
	{
//		material = transform.GetChild (0).GetComponent<MeshRenderer> ().material;
//		color0 = material.color;
//		color1 = material.color;
//		color1.a = 1f;
	}

	public Position GetPosition ()
	{
		return position;
	}

	public void SetPosition (Position position)
	{
		this.position = position;
		gameObject.name += " " + position;
	}

	public void Highlight (bool highlight)
	{
//		material.color = highlight ? color1 : color0;
//		transform.GetChild (1).gameObject.SetActive (highlight);
	}

}
