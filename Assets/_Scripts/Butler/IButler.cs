using UnityEngine;
using System.Collections;

public interface IButler
{

	//	event Game.ConnectStatusAction OnConnectChanged;

	void Init ();

	GameState GetConnectionStatus ();

	void NewGame ();

	void QuitGame ();


	int NumPlayers ();

	string GetLocalUsername ();


	void SendMessageToAll (bool reliable, byte[] data);

}
