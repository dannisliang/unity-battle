using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour
{
	[HideInInspector]
	public Boat boat{ get; private set; }

	public void Configure (Boat boat)
	{
		this.boat = boat;
		string boatSuffix = boat.positions [0] + "-" + boat.positions [boat.Size () - 1];
		name += " (" + boat.Size () + " units) " + boatSuffix;
		float len = (float)boat.Size ();

		transform.localPosition = new Vector3 (boat.GetPosition (0).x, Utils.GRID_SIZE - 1 - boat.GetPosition (0).y, 0f);
		Transform meshChild = transform.GetChild (0);
		meshChild.name += boatSuffix;
		meshChild.localPosition = new Vector3 (boat.horizontal ? boat.Size () / 2f : .5f, boat.horizontal ? .5f : 1f - len / 2f, .5f * Utils.BOAT_HEIGHT);
		meshChild.localScale = new Vector3 (boat.horizontal ? len : 1f, boat.horizontal ? 1f : len, Utils.BOAT_HEIGHT);

		for (int i = 0; i < boat.Size (); i++) {
			GameObject child = new GameObject ();
			child.name = name + " Collider " + boat.positions [i];
			child.transform.SetParent (transform, false);
			child.layer = gameObject.layer;
			PositionMarkerController positionMakerController = child.AddComponent<PositionMarkerController> ();
			positionMakerController.position = boat.GetPosition (i);

//			BoxCollider collider = child.AddComponent<BoxCollider> ();
//			collider.isTrigger = true;
//			collider.transform.localPosition = new Vector3 (boat.horizontal ? i + .5f : .5f, boat.horizontal ? .5f : -i + .5f, -.5f * boatHeight);
//			collider.transform.localScale = new Vector3 (1f, 1f, boatHeight);
		}
	}
	
}
