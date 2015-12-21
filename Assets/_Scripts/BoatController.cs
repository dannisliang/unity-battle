using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour
{
	float boatHeight = .2f;

	Boat boat;

	public void Configure (Boat boat)
	{
		this.boat = boat;
		string boatSuffix = boat.positions [0] + "-" + boat.positions [boat.Size () - 1];
		name += " (" + boat.Size () + " units) " + boatSuffix;
		float len = (float)boat.Size ();

		transform.localPosition = new Vector3 (boat.GetX (), Utils.GRID_SIZE - boat.GetZ (), 0f);
		transform.GetChild (0).name += boatSuffix;
		transform.GetChild (0).localPosition = new Vector3 (boat.horizontal ? boat.Size () / 2f : .5f, boat.horizontal ? -.5f : -len / 2f, -.5f * boatHeight);
		transform.GetChild (0).localScale = new Vector3 (boat.horizontal ? len : 1f, boat.horizontal ? 1f : len, boatHeight);

		Vector3 colliderPos = new Vector3 (.5f, -.5f, -.5f * boatHeight);
		for (int i = 0; i < boat.Size (); i++) {
			GameObject child = new GameObject ();
			child.name = name + " Collider " + boat.positions [i];
			child.transform.SetParent (transform, false);
			child.layer = gameObject.layer;

			BoxCollider collider = child.AddComponent<BoxCollider> ();
			collider.isTrigger = true;
			collider.transform.localPosition = new Vector3 (boat.horizontal ? i + .5f : .5f, boat.horizontal ? -.5f : -i - .5f, -.5f * boatHeight);
			collider.transform.localScale = new Vector3 (1f, 1f, boatHeight);
		}
	}
	
}
