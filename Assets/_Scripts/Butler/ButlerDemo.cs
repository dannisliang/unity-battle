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

	public void NewGame ()
	{
	}

	public GameState GetConnectionStatus ()
	{
		if (!_signedIn) {
			return GameState.AUTHENTICATING;
		}
		if (_gameConnected) {
			Assert.IsTrue (gameSetupPercent == 100);
			return GameState.PLAYING;
		} else {
			return GameState.SETTING_UP_GAME;
		}
	}

	//	void SignIn (bool silent = false)
	//	{
	//		SceneMaster.instance.Async (delegate {
	//			signedIn = true;
	//		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	//	}
	//
	//	void SignOut ()
	//	{
	//		gameConnected = false;
	//		signedIn = false;
	//	}
	//
	//	void SetupGame (bool withInvitation)
	//	{
	//		Assert.IsTrue (gameSetupPercent == 0);
	//		gameSetupPercent = 1;
	//		SceneMaster.instance.Async (delegate {
	//			gameSetupPercent = 100;
	//			gameConnected = true;
	//		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	//	}

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

	public override string ToString ()
	{
		return string.Format ("[ButlerDemo: signedIn={0}, gameConnected={1}, gameSetupPercent={2}]", signedIn, gameConnected, gameSetupPercent);
	}
}
