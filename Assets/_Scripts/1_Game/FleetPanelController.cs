using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FleetPanelController : MonoBehaviour
{
	public BoatPlacementController boatPlacementController;
	public GameObject shipPrefab;

	void OnEnable ()
	{
		BattleController.instance.OnBattleState += UpdateSelf;
	}

	void OnDisable ()
	{
		BattleController.instance.OnBattleState -= UpdateSelf;
	}

	void UpdateSelf (Whose whoseTurn, bool firing, Whose loser)
	{
		Boat[] boats = boatPlacementController.grid.boats;
		if (boats != null) {
			for (int i = 0; i < boats.Length; i++) {
				GameObject clone = Game.instance.InstantiateTemp (shipPrefab);
				Vector3[] fourCornersArray = new Vector3[4];
				GetComponent<RectTransform> ().GetWorldCorners (fourCornersArray);
				if (boatPlacementController.whose == Whose.Ours) {
					clone.transform.position = fourCornersArray [2] - (2.5f + i * 1.4f) * transform.up - (float)boats [i].Size () * transform.right;
				} else {
					clone.transform.position = fourCornersArray [1] - (2.5f + i * 1.4f) * transform.up;
				}
				clone.transform.rotation = Quaternion.FromToRotation (-clone.transform.forward, -transform.forward);
				clone.transform.localScale = new Vector3 ((float)boats [i].Size () / 5f, 1f, 1f);
			}
		}
	}
}
