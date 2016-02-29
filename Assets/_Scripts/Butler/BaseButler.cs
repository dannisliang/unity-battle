using UnityEngine;
using System.Collections;

public abstract  class BaseButler : MonoBehaviour
{
	public GameState gameState { get; private set; }

	public event Game.GameStateChange OnGameStateChange;

	void Awake ()
	{
		enabled = false;
	}

	public GameState GetGameState ()
	{
		return gameState;
	}

	public virtual void SetGameState (GameState gameState)
	{
		this.gameState = gameState;
		OnGameStateChange (gameState);
	}

	protected void CheckInternetReachability ()
	{
		Game.instance.SetErrorFailureReasonText (null);
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			Game.instance.SetErrorFailureReasonText ("— No internet connection —");
			SetGameState (GameState.GAME_WAS_TORN_DOWN);
			return;
		}
	}

	public abstract void NewGame ();

	public abstract void StartGamePlay ();

	public abstract void PauseGamePlay ();

	public abstract void QuitGame ();


	public abstract int NumPlayers ();

	public abstract string GetLocalUsername ();


	public abstract void SendMessageToAll (bool reliable, byte[] data);

}
