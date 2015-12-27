using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class NetworkManagerController : NetworkManager
{

	void Start ()
	{
#if UNITY_STANDALONE
		string[] args = Environment.GetCommandLineArgs ();
		for (int i = 0; i < args.Length; i++) {
			if (args [i] == "ip") {
				networkAddress = args [i + 1];
			}
			if (args [i] == "port") {
				networkPort = Int32.Parse (args [i + 1]);
			}
		}
//		Utils.logger.Log ("USING NETWORK: " + networkAddress + " " + networkPort);

		if (Application.isEditor) {
			StartHost ();
		} else {
//			StartClient ();
		}
#elif UNITY_ANDROID
		StartHost();
#endif
	}

	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.S)) {
//			Utils.logger.Log ("Calling StartServer");
			StartServer ();
		}
		if (Input.GetKeyUp (KeyCode.C)) {
//			Utils.logger.Log ("Calling StartClient");
			StartClient ();
		}
		if (Input.GetKeyUp (KeyCode.H)) {
//			Utils.logger.Log ("Calling StartHost");
			StartHost ();
		}
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		Utils.logger.Log ("OnClientConnect " + conn);
		base.OnClientConnect (conn);
	}

	public override void OnClientError (NetworkConnection conn, int errorCode)
	{
		Utils.logger.Log ("OnClientError " + conn + " / " + errorCode);
		base.OnClientError (conn, errorCode);
	}

	public override void OnClientNotReady (NetworkConnection conn)
	{
		Utils.logger.Log ("OnClientNotReady " + conn);
		base.OnClientNotReady (conn);
	}

	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		Utils.logger.Log ("OnClientSceneChanged " + conn);
		base.OnClientSceneChanged (conn);
	}

	public override void OnStopHost ()
	{
		Utils.logger.Log ("OnStopHost");
		base.OnStopHost ();
	}

	public override void OnStopClient ()
	{
		Utils.logger.Log ("OnStopClient");
		base.OnStopClient ();
	}
}
