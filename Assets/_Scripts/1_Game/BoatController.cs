using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour
{
	[HideInInspector]
	public Boat boat{ get; private set; }

	public Whose whose;

	public Material highlightMaterial;

	MeshRenderer meshRenderer;
	Material initialMaterial;

	void Awake ()
	{
		meshRenderer = gameObject.GetComponentInChildren<MeshRenderer> ();
		initialMaterial = meshRenderer.material;
	}

	public void Identify (bool highlight, Position position)
	{
		meshRenderer.material = highlight ? highlightMaterial : initialMaterial;
		BattleController.instance.Identify (highlight ? boat : null, highlight ? position : null);
	}

	public void Configure (Boat boat, bool aboveMarkers)
	{
		this.boat = boat;
		float height = -.5f * Utils.BOAT_HEIGHT - (aboveMarkers ? 2 * Utils.CLEARANCE_HEIGHT : 0);
		string boatSuffix = boat.positions [0] + "-" + boat.positions [boat.Size () - 1];
		name += " (" + boat.Size () + " units) " + boatSuffix;
		float len = (float)boat.Size ();

		transform.localPosition = new Vector3 (boat.GetPosition (0).x, Utils.GRID_SIZE.y - 1 - boat.GetPosition (0).y, 0f);
		Transform meshChild = transform.GetChild (0);
		meshChild.name += boatSuffix;
		meshChild.localPosition = new Vector3 (boat.horizontal ? boat.Size () / 2f : .5f, boat.horizontal ? .5f : 1f - len / 2f, height);
		meshChild.localScale = new Vector3 ((boat.horizontal ? len : 1f) - .15f, (boat.horizontal ? 1f : len) - .15f, 1f);

		// only normal (not sunk) boats have a ship child
		if (transform.childCount > 1) {
			Transform shipChild = transform.GetChild (1);
			shipChild.name += boatSuffix;
			shipChild.localPosition = new Vector3 (boat.horizontal ? 0f : 1f, boat.horizontal ? 0f : 1f - len, height);
			shipChild.localScale = new Vector3 (len / 5f, 1f, 1f);
			shipChild.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, boat.horizontal ? 0f : 90f));

			ShipController shipController = shipChild.GetComponent<ShipController> ();
			shipController.SetDamage (0, boat.Size ());
		}

		for (int i = 0; i < boat.Size (); i++) {
			GameObject child = new GameObject ();
			child.name = name + " Collider " + boat.positions [i];
			child.transform.SetParent (transform, false);
			child.layer = gameObject.layer;
			PositionMarkerController positionMakerController = child.AddComponent<PositionMarkerController> ();
			positionMakerController.position = boat.GetPosition (i);

			// add after PositionMarkerController
			child.AddComponent<BoatIdentificationController> ();

			BoxCollider collider = child.AddComponent<BoxCollider> ();
			collider.isTrigger = true;
			collider.transform.localPosition = new Vector3 (boat.horizontal ? i + .5f : .5f, boat.horizontal ? .5f : -i + .5f, -Utils.BOAT_HEIGHT);
			collider.transform.localScale = new Vector3 (1f, 1f, 0f);
		}
	}
	
}
