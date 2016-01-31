using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;
using System.Collections.Generic;

public class ButlerFirebase : BaseButler
{

	public const string FIREBASE_URL = "https://sauer-battle.firebaseIO.com";

	IFirebase firebase;
	IFirebase lobby;
	IFirebase players;
	IFirebase game;
	//	IFirebase ourMoves;
	//	IFirebase theirMoves;

	List<string> playersInLobby;
	List<string> incomingInvites;

	string uniqueId;
	string opponentId;
	bool initialized;

	void OnEnable ()
	{
		if (!initialized) {
			uniqueId = SystemInfo.deviceUniqueIdentifier;
	
			firebase = Firebase.CreateNew (FIREBASE_URL);
			firebase.Error += OnError;
	
			lobby = firebase.Child ("Lobby");
			players = firebase.Child ("Players");
//			ourMoves = games.Child (uniqueId);
	
			initialized = true;
		}

		playersInLobby = new List<string> ();
		incomingInvites = new List<string> ();

//		ClearData (uniqueId);
//		ClearData ("ai");
//	
		//		ours.ChildAdded += OpponentSentUsMove;
		//		ours.ChildChanged += OpponentSentUsMove;

		StartCoroutine ("ConnectWithOpponent");
//		StartCoroutine ("AiCoroutine");
	}

	void OnDisable ()
	{
		if (!initialized) {
			return;
		}

		StopCoroutine ("AiCoroutine");
		StopCoroutine ("ConnectWithOpponent");

		lobby.ChildRemoved -= LobbyChildRemoved;
		lobby.ChildAdded -= LobbyChildAdded;
		Debug.Log (lobby.Key + "/" + uniqueId + "/Invites.ChildAdded -= InviteReceived");
		lobby.Child (uniqueId).Child ("Invites").ChildAdded -= InviteReceived;
		
		ClearPlayerData (uniqueId);
		ClearPlayerData ("ai");

		playersInLobby = null;
		incomingInvites = null;
	}

	void InviteReceived (object sender, ChangedEventArgs e)
	{
		Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> incomingInvites.Add()"));
		incomingInvites.Add (e.DataSnapshot.Key);
//
//		if (opponentId == null && candidateOpponentId == e.DataSnapshot.Key) {
//			Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> ACCEPT counter invite"));
//			opponentId = e.DataSnapshot.Key;
//			return;
//		}
//		if (opponentId == null && candidateOpponentId == null) {
//			int index = playersInLobby.IndexOf (e.DataSnapshot.Key);
//			if (index == 0) {
//				Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> NO-OP as already set as next (counter) invite"));
//				return;	
//			}
//			if (index != -1) {
//				Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> IGNORE player we don't think is in the lobby"));
//			}
//			Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> Set as next to (counter) invite"));
//			playersInLobby.RemoveAt (index);
//			playersInLobby.Insert (0, e.DataSnapshot.Key);
//		}
//		Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> IGNORE invite"));
	}

	void ClearPlayerData (string id)
	{
		lobby.Child (id).SetValue ((string)null);
		players.Child (id).SetValue ((string)null);
	}

	void OnError (object sender, ErrorEventArgs e)
	{
		Debug.Log (MakeMessage ("OnError", sender, e, "==> YIKES!"));
	}

	void LobbyChildAdded (object sender, ChangedEventArgs e)
	{
		if (e.DataSnapshot.Key == uniqueId) {
			Debug.Log (MakeMessage ("LobbyChildAdded", sender, e, "==> Ignoring self"));
			return;
		}
		string gs = e.DataSnapshot.Child ("GameState").StringValue;
		if (gs != GameState.SETTING_UP_GAME.ToString ()) {
			Debug.Log (MakeMessage ("LobbyChildAdded", sender, e, "==> Ignoring GameState=" + gs));
			return;
		}
		Debug.Log (MakeMessage ("LobbyChildAdded", sender, e, "==> playersInLobby.Add()"));
		playersInLobby.Add (e.DataSnapshot.Key);
	}

	void LobbyChildRemoved (object sender, ChangedEventArgs e)
	{
		if (opponentId == e.DataSnapshot.Key) {
			Debug.Log (MakeMessage ("LobbyChildRemoved", sender, e, "==> Opponent disappeared. TODO: end game"));
			opponentId = null;
			return;
		}
		if (playersInLobby.Contains (e.DataSnapshot.Key)) {
			Debug.Log (MakeMessage ("LobbyChildRemoved", sender, e, "==> Removing candidate opponent"));
			playersInLobby.Remove (e.DataSnapshot.Key);
		}
		Debug.Log (MakeMessage ("LobbyChildRemoved", sender, e, "==> Ignoring lobby departure"));
	}

	IEnumerator ConnectWithOpponent ()
	{
		while (opponentId == null) {
			yield return new WaitForSeconds (.5f);
			if (incomingInvites.Count > 0) {
				string id = incomingInvites [0];
				incomingInvites.Remove (id);
				Debug.Log ("Entertaining invite from " + id);

				if (playersInLobby.Remove (id)) {
					Debug.Log ("Sending counter (RSVP) invite to " + id);
					lobby.Child (id).Child ("Invites").Child (uniqueId).SetValue ("Let's play");
					opponentId = id;
				} else {
					Debug.Log ("Ignoring invite from player not in lobby: " + id);
				}
			} else if (playersInLobby.Count > 0) {
				string id = playersInLobby [0];
				playersInLobby.Remove (id);
				Debug.Log ("Extending new invite to " + id);
				lobby.Child (id).Child ("Invites").Child (uniqueId).SetValue ("Let's play");

				float expirationTime = Time.unscaledTime + 5f;
				while (Time.unscaledTime < expirationTime) {
					if (incomingInvites.Contains (id)) {
						opponentId = id;
					}
//					Debug.Log ("(Still) waiting for incomingInvites to contain " + id);
					yield return new WaitForSeconds (1f);
				}
				if (opponentId == null) {
					Debug.Log ("Recinding invite to " + id);
					lobby.Child (id).Child ("Invites").Child (uniqueId).SetValue ((string)null);
				}
			}
		}

		Debug.Log ("opponentId = " + opponentId);

//		game.Child (uniqueId).Child ("Grid").SetValue (BattleController.instance.boatsOursPlacementController.grid.ToString ());

		Debug.Log ("Setting our game state => SELECTING");
		SetGameState (GameState.SELECTING_VIEW_MODE);
//
//		Debug.Log ("invite expired; retracting invite");
//		lobby.Child (uniqueId).Child ("GameState").SetValue (gameState.ToString ());
//		lobby.Child (candidateOpponentId).Child ("Invites").Child (uniqueId).SetValue ((string)null);


		yield  return null;
	}

	//	void OpponentSentUsMove (object sender, ChangedEventArgs e)
	//	{
	//		Debug.Log (MakeMessage ("OpponentSentUsMove", sender, e, "==> "));
	//	}
	
	string MakeMessage (string prefix, object sender, ChangedEventArgs e, string action)
	{
		return prefix + ": " + e.DataSnapshot.Key + "=>" + e.DataSnapshot.StringValue + "\n" + action;
	}

	string MakeMessage (string prefix, object sender, ErrorEventArgs e, string iGnoringSelf)
	{
		return prefix + ": Error=" + e.Error;
	}

	public override void NewGame ()
	{
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
		SetGameState (GameState.SETTING_UP_GAME);
//		SetGameState (GameState.SELECTING_VIEW_MODE);
	}

	int aiMessageCount;

	IEnumerator AiCoroutine ()
	{
//		Debug.Log ("AI sending invite: " + lobby.Key + "/" + uniqueId + "/ZZZ=0");
//		lobby.Child (uniqueId).Child ("Invites").Child ("ZZZ").SetValue (0);

		yield return new WaitForSeconds (1f);
		ClearPlayerData ("ai");

		lobby.Child ("ai").Child ("GameState").SetValue (GameState.SETTING_UP_GAME.ToString ());
		yield return new WaitForSeconds (1f);

		Debug.Log ("AI sending invite: " + lobby.Key + "/" + uniqueId + "/ai=Let's play");
		lobby.Child (uniqueId).Child ("Invites").Child ("ai").SetValue ("Let's play");

//		yield return new WaitForSeconds (5f);
//		Debug.Log ("AI sending invite: " + lobby.Key + "/" + uniqueId + "/xx=0");
//		lobby.Child (uniqueId).Child ("Invites").Child ("xx").SetValue (0);

//		yield return new WaitForSeconds (5f);
//		Debug.Log ("AI sending invite: " + lobby.Key + "/" + uniqueId + "/bar=0");
//		lobby.Child (uniqueId).Child ("Invites").Child ("bar").SetValue (0);


		yield return new WaitForSeconds (1f);
		Debug.Log ("AI view mode -> SELECTING");
		lobby.Child ("ai").Child ("GameState").SetValue (GameState.SELECTING_VIEW_MODE.ToString ());

		yield return new WaitForSeconds (5f);
		players.Child ("ai").Child ("Grid").SetValue ("ai-grid-data-here");
	

//			yield return new WaitForSeconds (1f);
//			ai.Child (++aiMessageCount + "Aim").SetValue ("A1");
//	
//			yield return new WaitForSeconds (1f);
//			ai.Child (aiMessageCount + "Aim").SetValue ("B2");
//	
//			yield return new WaitForSeconds (1f);
//			ai.Child (aiMessageCount + "Aim").SetValue ("C#");
//	
//			yield return new WaitForSeconds (1f);
//			ai.Child (++aiMessageCount + "Fire").SetValue ("C2");
	}

	public override void StartGamePlay ()
	{
		Assert.AreEqual (GameState.SELECTING_VIEW_MODE, gameState);
		SetGameState (GameState.PLAYING);
	}

	public override void PauseGamePlay ()
	{
		Assert.AreEqual (GameState.PLAYING, gameState);
		SetGameState (GameState.SELECTING_VIEW_MODE);
	}

	public override void QuitGame ()
	{
		if (gameState == GameState.SETTING_UP_GAME || gameState == GameState.PLAYING || gameState == GameState.SELECTING_VIEW_MODE) {
			SetGameState (GameState.TEARING_DOWN_GAME);
		}
		if (gameState == GameState.TEARING_DOWN_GAME) {
			SetGameState (GameState.GAME_WAS_TORN_DOWN);
		}
	}

	public override int NumPlayers ()
	{
		throw new System.NotImplementedException ();
	}

	public override string GetLocalUsername ()
	{
		throw new System.NotImplementedException ();
	}

	public override void SendMessageToAll (bool reliable, byte[] data)
	{
		var encoded = Convert.ToBase64String (data);
		players.Child (uniqueId).Push ().SetValue (encoded);
		Debug.Log ("SendMessageToAll: " + (char)data [0] + " " + encoded);
		Game.instance.OnRealTimeMessageReceived (reliable, "firebaseAiSenderId", data);
	}

	public override void SetGameState (GameState gameState)
	{
		base.SetGameState (gameState);
	
		switch (gameState) {
		case GameState.GAME_WAS_TORN_DOWN:
			SetGameState (GameState.SELECTING_GAME_TYPE);
			break;
		case GameState.SELECTING_GAME_TYPE:
			break;
		case GameState.SETTING_UP_GAME:
			ClearPlayerData (uniqueId);
			lobby.ChildRemoved += LobbyChildRemoved;
			lobby.ChildAdded += LobbyChildAdded;
			lobby.Child (uniqueId).Child ("GameState").SetValue (gameState.ToString ());

//			Debug.Log ("add pre event handler invite: " + lobby.Key + "/" + uniqueId + "/pre-handler-added=0");
//			lobby.Child (uniqueId).Child ("Invites").Child ("pre-handler-added").SetValue (0);

			Debug.Log (lobby.Key + "/" + uniqueId + "/Invites.ChildAdded += InviteReceived");
			lobby.Child (uniqueId).Child ("Invites").ChildAdded += InviteReceived;
			lobby.Child (uniqueId).Child ("Invites").Error += (object sender, ErrorEventArgs e) => {
				Debug.Log ("Invites Error: " + e.Error);
			};

			//			lobby.Child (uniqueId).Child ("Invites").ChildChanged += (object sender, ChangedEventArgs e) => {
//				Debug.Log ("ChildChanged " + e.DataSnapshot.Key);
//			};
//			lobby.Child (uniqueId).Child ("Invites").ChildMoved += (object sender, ChangedEventArgs e) => {
//				Debug.Log ("ChildMoved " + e.DataSnapshot.Key);
//			};
//			lobby.Child (uniqueId).Child ("Invites").ChildRemoved += (object sender, ChangedEventArgs e) => {
//				Debug.Log ("ChildRemoved " + e.DataSnapshot.Key);
//			};


//			Debug.Log ("add post event handler invite: " + lobby.Key + "/" + uniqueId + "/post-handler-added=0");
//			lobby.Child (uniqueId).Child ("Invites").Child ("post-handler-added").SetValue (0);


			break;
		case GameState.TEARING_DOWN_GAME:
		case GameState.AUTHENTICATING:
		case GameState.SELECTING_VIEW_MODE:
			lobby.Child (uniqueId).Child ("GameState").SetValue (gameState.ToString ());
			break;
		case GameState.PLAYING:
//				lobby.Child (uniqueId).SetValue ((string)null);
			break;
		default:
			throw new NotImplementedException ();
		}
	}

	public override string ToString ()
	{
		return string.Format ("[{0}: gameState={1}]", name, gameState);
	}

}
