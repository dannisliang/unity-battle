using UnityEngine;
using System.Collections;

public interface IButler
{

	event Game.GameStateChange OnGameStateChange;

	void Init ();

	GameState GetGameState ();

	void NewGame ();

	void QuitGame ();


	int NumPlayers ();

	string GetLocalUsername ();


	void SendMessageToAll (bool reliable, byte[] data);

}
