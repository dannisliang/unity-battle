using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
	Position position;

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
		transform.GetChild (1).gameObject.SetActive (highlight);
	}

}
