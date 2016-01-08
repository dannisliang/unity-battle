using UnityEngine;
using System.Collections;

public interface IButler
{

	//	event Game.ConnectStatusAction OnConnectChanged;

	void Init ();

	ConnectionStatus GetConnectionStatus ();


	void SignIn (bool silent = false);

	void SignOut ();


	int NumPlayers ();

	string GetLocalUsername ();


	void SetupGame (bool withInvitation);

	void QuitGame ();


	void SendMessageToAll (bool reliable, byte[] data);

}
