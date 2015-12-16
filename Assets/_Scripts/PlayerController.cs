using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
	GameController gameController;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
	}

	void Update ()
	{	
		if (Input.GetButton ("Fire1")) {
			gameController.StartNewGame ();
		}
	}
	
}
