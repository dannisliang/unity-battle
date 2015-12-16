using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public BoatPlacementController boatPlacementController;

	public void StartNewGame ()
	{
		boatPlacementController.RecreateBoats ();
	}
}
