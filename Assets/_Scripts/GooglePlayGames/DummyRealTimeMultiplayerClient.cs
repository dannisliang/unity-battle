﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Multiplayer;

public class DummyRealTimeMultiplayerClient : IRealTimeMultiplayerClient
{

	bool roomConnecting;
	bool roomConnected;
	List<Participant> participants;
	RealTimeMultiplayerListener listener;

	#region IRealTimeMultiplayerClient implementation

	public void CreateQuickGame (uint minOpponents, uint maxOpponents, uint variant, RealTimeMultiplayerListener listener)
	{
		CreateQuickGame (minOpponents, maxOpponents, variant, 0L, listener);
	}

	public void CreateQuickGame (uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitMask, RealTimeMultiplayerListener listener)
	{
		Assert.IsTrue (this.listener == null || this.listener == listener);
		this.listener = listener;
		Assert.IsFalse (roomConnecting);
		Assert.IsFalse (roomConnected);
		SceneMaster.instance.Async (() => {
			roomConnecting = !Input.GetKey (KeyCode.F);
			listener.OnRoomSetupProgress (roomConnecting ? 20 : 0);
		}, Utils.DUMMY_PLAY_GAMES_REAL_TIME_ASYNC_DELAY);
		SceneMaster.instance.Async (() => {
			if (!roomConnecting || roomConnected) {
				return;
			}
			roomConnected = !Input.GetKey (KeyCode.F);
			if (roomConnected) {
				participants = new List<Participant> ();
				participants.Add (new Participant ("me", "me42", Participant.ParticipantStatus.Joined, new Player ("me player", "player42", null), true));
				participants.Add (new Participant ("other", "other43", Participant.ParticipantStatus.Joined, new Player ("other player", "player43", null), true));
			}
			listener.OnRoomConnected (roomConnected);
		}, 2f * Utils.DUMMY_PLAY_GAMES_REAL_TIME_ASYNC_DELAY);
	}

	public void CreateWithInvitationScreen (uint minOpponents, uint maxOppponents, uint variant, RealTimeMultiplayerListener listener)
	{
		throw new System.NotImplementedException ();
	}

	public void ShowWaitingRoomUI ()
	{
		throw new System.NotImplementedException ();
	}

	public void GetAllInvitations (System.Action<Invitation[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void AcceptFromInbox (RealTimeMultiplayerListener listener)
	{
		throw new System.NotImplementedException ();
	}

	public void AcceptInvitation (string invitationId, RealTimeMultiplayerListener listener)
	{
		throw new System.NotImplementedException ();
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		if (Protocol.GetMessageType (data) != Protocol.MessageType.AIM_AT) {
			Debug.Log ("***PRETENDING SendMessageToAll(" + reliable + ",'" + (char)data [0] + "': " + data.Length + "bytes)");
		}
		if (Protocol.GetMessageType (data) == Protocol.MessageType.GRID_POSITIONS) {
			data = ButlerAi.MakeAiGridMessage ();
		} else {
			// simply mirror back messages with delay
		}
		SceneMaster.instance.Async (() => {
			listener.OnRealTimeMessageReceived (reliable, Utils.AI_PLAYER_ID, data);
		}, Utils.DUMMY_PLAY_GAMES_REAL_TIME_REPLAY_DELAY);
	}

	public void SendMessageToAll (bool reliable, byte[] data, int offset, int length)
	{
		throw new System.NotImplementedException ();
	}

	public void SendMessage (bool reliable, string participantId, byte[] data)
	{
		throw new System.NotImplementedException ();
	}

	public void SendMessage (bool reliable, string participantId, byte[] data, int offset, int length)
	{
		throw new System.NotImplementedException ();
	}

	public System.Collections.Generic.List<Participant> GetConnectedParticipants ()
	{
		return participants;
	}

	public Participant GetSelf ()
	{
		return participants.ToArray () [0];
	}

	public Participant GetParticipant (string participantId)
	{
		throw new System.NotImplementedException ();
	}

	public Invitation GetInvitation ()
	{
		throw new System.NotImplementedException ();
	}

	public void LeaveRoom ()
	{
		SceneMaster.instance.Async (() => {
			roomConnecting = false;
			roomConnected = false;
			participants = null;
			listener.OnLeftRoom ();
		}, Utils.DUMMY_PLAY_GAMES_REAL_TIME_ASYNC_DELAY);
	}

	public bool IsRoomConnected ()
	{
		return roomConnected;
	}

	public void DeclineInvitation (string invitationId)
	{
		throw new System.NotImplementedException ();
	}

	#endregion

}
