using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour
{

	Boat boat;

	public void Configure (Boat boat)
	{
		this.boat = boat;
		name += " (" + boat.Size () + " units)";

		Vector3 pos = Vector3.zero;
		pos.x = boat.GetX () - .5f * Utils.GRID_SIZE + .5f;
		pos.z = Utils.GRID_SIZE - .5f - boat.GetZ ();
		transform.localPosition = pos;

		Vector3 scale = Vector3.one;
		scale.x = boat.horizontal ? boat.Size () : 1f;
		scale.z = boat.horizontal ? 1f : boat.Size ();
		transform.localScale = scale;

	}
	
}
