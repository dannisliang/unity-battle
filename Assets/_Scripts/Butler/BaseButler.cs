using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public abstract class BaseButler : MonoBehaviour
{
	protected GameState gameState;

	virtual protected void OnEnable ()
	{
		Game.instance.OnGameStateChange += UpdateGameState;
	}

	virtual protected void OnDisable ()
	{
		Game.instance.OnGameStateChange -= UpdateGameState;
	}

	void UpdateGameState (GameState state)
	{
		gameState = state;
	}

	protected void CheckInternetReachability ()
	{
		Game.instance.SetErrorFailureReasonText (null);
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			Game.instance.SetErrorFailureReasonText ("— No internet connection —");
			Game.instance.SetGameState (GameState.GAME_WAS_TORN_DOWN);
			return;
		}
	}

	public abstract void NewGame ();

	public abstract void QuitGame ();


	public abstract int NumPlayers ();

	public abstract string GetLocalUsername ();


	public abstract void SendMessageToAll (bool reliable, ref byte[] data);

}
