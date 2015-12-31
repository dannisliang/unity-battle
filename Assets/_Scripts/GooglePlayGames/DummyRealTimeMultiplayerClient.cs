using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Multiplayer;

public class DummyRealTimeMultiplayerClient : IRealTimeMultiplayerClient
{

	bool roomConnecting;
	bool roomConnected;
	List<Participant> participants;

	#region IRealTimeMultiplayerClient implementation

	public void CreateQuickGame (uint minOpponents, uint maxOpponents, uint variant, RealTimeMultiplayerListener listener)
	{
		CreateQuickGame (minOpponents, maxOpponents, variant, 0L, listener);
	}

	public void CreateQuickGame (uint minOpponents, uint maxOpponents, uint variant, ulong exclusiveBitMask, RealTimeMultiplayerListener listener)
	{
		Assert.IsFalse (roomConnecting);
		Assert.IsFalse (roomConnected);
		GameController.instance.ExecuteDelayed (() => {
			roomConnecting = true;
			listener.OnRoomSetupProgress (20);
		}, 1f);
		GameController.instance.ExecuteDelayed (() => {
			if (!roomConnecting || roomConnected) {
				return;
			}
			roomConnected = true;
			participants = new List<Participant> ();
			participants.Add (new Participant ("me", "me42", Participant.ParticipantStatus.Joined, new Player ("me player", "player42", null), true));
			participants.Add (new Participant ("other", "other43", Participant.ParticipantStatus.Joined, new Player ("other player", "player43", null), true));
			listener.OnRoomConnected (GameController.instance.RoomSetupPercent () > 0);
		}, 2f);
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
		Debug.Log ("***PRETENDING SendMessageToAll(" + reliable + "," + (char)data [0] + "-" + data.Length + ")");
		// mirror back messages
		GameController.instance.OnRealTimeMessageReceived (reliable, "senderid", data);
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
		roomConnecting = false;
		roomConnected = false;
		participants = null;
		GameController.instance.OnLeftRoom ();
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
