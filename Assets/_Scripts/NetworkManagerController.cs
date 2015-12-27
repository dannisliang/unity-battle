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
//		Debug.logger.Log ("USING NETWORK: " + networkAddress + " " + networkPort);

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
//			Debug.logger.Log ("Calling StartServer");
			StartServer ();
		}
		if (Input.GetKeyUp (KeyCode.C)) {
//			Debug.logger.Log ("Calling StartClient");
			StartClient ();
		}
		if (Input.GetKeyUp (KeyCode.H)) {
//			Debug.logger.Log ("Calling StartHost");
			StartHost ();
		}
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		Debug.logger.Log ("OnClientConnect " + conn);
		base.OnClientConnect (conn);
	}

	public override void OnClientError (NetworkConnection conn, int errorCode)
	{
		Debug.logger.Log ("OnClientError " + conn + " / " + errorCode);
		base.OnClientError (conn, errorCode);
	}

	public override void OnClientNotReady (NetworkConnection conn)
	{
		Debug.logger.Log ("OnClientNotReady " + conn);
		base.OnClientNotReady (conn);
	}

	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		Debug.logger.Log ("OnClientSceneChanged " + conn);
		base.OnClientSceneChanged (conn);
	}

	public override void OnStopHost ()
	{
		Debug.logger.Log ("OnStopHost");
		base.OnStopHost ();
	}

	public override void OnStopClient ()
	{
		Debug.logger.Log ("OnStopClient");
		base.OnStopClient ();
	}
}
