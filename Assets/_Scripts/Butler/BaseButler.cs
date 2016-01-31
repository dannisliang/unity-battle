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

	public abstract void NewGame ();

	public abstract void StartGamePlay ();

	public abstract void PauseGamePlay ();

	public abstract void QuitGame ();


	public abstract int NumPlayers ();

	public abstract string GetLocalUsername ();


	public abstract void SendMessageToAll (bool reliable, byte[] data);

}
