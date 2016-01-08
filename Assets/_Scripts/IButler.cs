﻿using UnityEngine;
using System.Collections;

public interface IButler
{

	bool IsSignedIn ();

	bool IsGameConnected ();

	int GameSetupPercent ();


	void Init ();


	void SignIn (bool silent = false);

	void SignOut ();


	int GetConnectedParticipantCount ();

	string GetLocalUsername ();


	void SetupGame (bool withInvitation);

	void QuitGame ();


	void SendMessageToAll (bool reliable, byte[] data);

}
