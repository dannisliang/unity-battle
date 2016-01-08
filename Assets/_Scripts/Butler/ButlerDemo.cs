using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using GooglePlayGames.BasicApi.Multiplayer;

public class ButlerDemo : MonoBehaviour,IButler
{

	//	public event Game.ConnectStatusAction OnConnectChanged;

	bool _signedIn;
	bool _gameConnected;
	int _gameSetupPercent;

	public bool IsSignedIn ()
	{
		return signedIn;
	}

	public bool IsGameConnected ()
	{
		return gameConnected;
	}

	public int GameSetupPercent ()
	{
		return gameSetupPercent;
	}

	public bool signedIn {
		get {
			return _signedIn;
		}
		set {
			if (_signedIn == value) {
				return;
			}
			_signedIn = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

	public bool gameConnected {
		get {
			return _gameConnected;
		}
		set {
			if (_gameConnected == value) {
				return;
			}
			_gameConnected = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

	public int gameSetupPercent {
		get {
			return _gameSetupPercent;
		}
		set {
			if (_gameSetupPercent == value) {
				return;
			}
			_gameSetupPercent = value;
			Game.instance.InvokeConnectStatusAction ();
		}
	}

	public int NumPlayers ()
	{
		return gameConnected ? 2 : 0;
	
	}

	public string GetLocalUsername ()
	{
		return gameConnected ? "Ford Prefect" : "";
	}

	public void Init ()
	{
	}

	public void SignIn (bool silent = false)
	{
		SceneMaster.instance.Async (delegate {
			signedIn = true;
		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	}

	public void SignOut ()
	{
		gameConnected = false;
		signedIn = false;
	}

	public void SetupGame (bool withInvitation)
	{
		Assert.IsTrue (gameSetupPercent == 0);
		gameSetupPercent = 1;
		SceneMaster.instance.Async (delegate {
			gameSetupPercent = 100;
			gameConnected = true;
		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	}

	public void QuitGame ()
	{
		SceneMaster.instance.Async (delegate {
			gameConnected = false;
			gameSetupPercent = 0;
		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	}

	public void SendMessageToAll (bool reliable, byte[] data)
	{
		SceneMaster.instance.Async (delegate {
			Game.instance.OnRealTimeMessageReceived (reliable, "dummySenderId", data);
		}, Utils.DUMMY_PLAY_GAMES_REPLAY_DELAY);
	}

}
