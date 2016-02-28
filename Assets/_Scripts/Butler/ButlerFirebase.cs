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
	IFirebase ourInvites;
	IFirebase opponentMovesRef;

	List<string> playersInLobby;
	List<string> incomingInvites;
	Queue<string> opponentMoves;

	string uniqueId;
	string opponentId;
	bool initialized;

	void OnEnable ()
	{
		if (!initialized) {
			uniqueId = SystemInfo.deviceUniqueIdentifier;
	
			firebase = Firebase.CreateNew (FIREBASE_URL);
			firebase.Error += OnError; // note: handler is never unregistered
	
			lobby = firebase.Child ("Lobby");
			players = firebase.Child ("Players");

			ourInvites = lobby.Child (uniqueId).Child ("Invites");
	
			initialized = true;
		}

		playersInLobby = new List<string> ();
		incomingInvites = new List<string> ();
		opponentMoves = new Queue<string> ();

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

		Debug.Log ("lobby.ChildRemoved -= LobbyChildRemoved");
		lobby.ChildRemoved -= LobbyChildRemoved;

		Debug.Log ("lobby.ChildAdded -= LobbyChildAdded");
		lobby.ChildAdded -= LobbyChildAdded;

		Debug.Log (lobby.Key + "/" + uniqueId + "/Invites.ChildAdded -= InviteReceived");
		ourInvites.ChildAdded -= InviteReceived;


		if (opponentMovesRef != null) {
			Debug.Log (players.Key + "/" + opponentId + ".ChildAdded -= OpponentMoved");
			opponentMovesRef.ChildAdded -= OpponentMoved;
		}

		if (opponentId != null) {
			opponentId = null;
		}

		ClearPlayerData (uniqueId);
		ClearPlayerData ("ai");

		playersInLobby = null;
		incomingInvites = null;
	}

	void InviteReceived (object sender, ChangedEventArgs e)
	{
		Debug.Log (MakeMessage ("InviteReceived", sender, e, " ==> incomingInvites.Add()"));
		incomingInvites.Add (e.DataSnapshot.Key);
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
			yield return new WaitForEndOfFrame ();
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
					yield return new WaitForEndOfFrame ();
				}
				if (opponentId == null) {
					Debug.Log ("Recinding invite to " + id);
					lobby.Child (id).Child ("Invites").Child (uniqueId).SetValue ((string)null);
				}
			}
		}

		Debug.Log ("STARTING GAME WITH opponentId = " + opponentId);

		Debug.Log ("Setting our game state => SELECTING");
		SetGameState (GameState.SELECTING_VIEW_MODE);

		Debug.Log (players.Key + "/" + opponentId + ".ChildAdded += OpponentMoved");
		opponentMovesRef = players.Child (opponentId);
		opponentMovesRef.ChildAdded += OpponentMoved;


		while (opponentId != null) {
			if (opponentMoves.Count > 0) {
				string encoded = opponentMoves.Dequeue ();
				byte[] data = Convert.FromBase64String (encoded);
				Game.instance.OnRealTimeMessageReceived (true, opponentId, data);
			}
			yield return new WaitForEndOfFrame ();
		}

	}

	void OpponentMoved (object sender, ChangedEventArgs e)
	{
		Debug.Log (MakeMessage ("OpponentMoved", sender, e, " ==> enqueueing"));
		opponentMoves.Enqueue (e.DataSnapshot.StringValue);
	}

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
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			Game.instance.SetErrorFailureReasonText ("— No internet connection —");
			SetGameState (GameState.GAME_WAS_TORN_DOWN);
			return;
		}
		Assert.AreEqual (GameState.SELECTING_GAME_TYPE, gameState);
		SetGameState (GameState.AUTHENTICATING);
		SetGameState (GameState.SETTING_UP_GAME);
	}

	//	int aiMessageCount;
	//
	//	IEnumerator AiCoroutine ()
	//	{
	//		yield return new WaitForSeconds (1f);
	//		ClearPlayerData ("ai");
	//
	//		lobby.Child ("ai").Child ("GameState").SetValue (GameState.SETTING_UP_GAME.ToString ());
	//		yield return new WaitForSeconds (1f);
	//
	//		Debug.Log ("AI sending invite: " + lobby.Key + "/" + uniqueId + "/ai=Let's play");
	//		lobby.Child (uniqueId).Child ("Invites").Child ("ai").SetValue ("Let's play");
	//
	//		yield return new WaitForSeconds (1f);
	//		Debug.Log ("AI view mode -> SELECTING");
	//		lobby.Child ("ai").Child ("GameState").SetValue (GameState.SELECTING_VIEW_MODE.ToString ());
	//
	//		yield return new WaitForSeconds (5f);
	//		players.Child ("ai").Child ("Grid").SetValue ("ai-grid-data-here");
	//	}

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

			Debug.Log ("lobby.ChildRemoved += LobbyChildRemoved");
			lobby.ChildRemoved += LobbyChildRemoved;

			Debug.Log ("lobby.ChildAdded += LobbyChildAdded");
			lobby.ChildAdded += LobbyChildAdded;

			lobby.Child (uniqueId).Child ("GameState").SetValue (gameState.ToString ());

			Debug.Log (lobby.Key + "/" + uniqueId + "/Invites.ChildAdded += InviteReceived");
			ourInvites.ChildAdded += InviteReceived;

			break;
		case GameState.TEARING_DOWN_GAME:
		case GameState.AUTHENTICATING:
			break;
		case GameState.SELECTING_VIEW_MODE:
			lobby.Child (uniqueId).Child ("GameState").SetValue (gameState.ToString ());
			break;
		case GameState.PLAYING:
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
