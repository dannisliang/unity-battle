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
//		Debug.Log ("USING NETWORK: " + networkAddress + " " + networkPort);

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
//			Debug.Log ("Calling StartServer");
			StartServer ();
		}
		if (Input.GetKeyUp (KeyCode.C)) {
//			Debug.Log ("Calling StartClient");
			StartClient ();
		}
		if (Input.GetKeyUp (KeyCode.H)) {
//			Debug.Log ("Calling StartHost");
			StartHost ();
		}
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		Debug.Log ("OnClientConnect " + conn);
		base.OnClientConnect (conn);
	}

	public override void OnClientError (NetworkConnection conn, int errorCode)
	{
		Debug.Log ("OnClientError " + conn + " / " + errorCode);
		base.OnClientError (conn, errorCode);
	}

	public override void OnClientNotReady (NetworkConnection conn)
	{
		Debug.Log ("OnClientNotReady " + conn);
		base.OnClientNotReady (conn);
	}

	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		Debug.Log ("OnClientSceneChanged " + conn);
		base.OnClientSceneChanged (conn);
	}

	public override void OnStopHost ()
	{
		Debug.Log ("OnStopHost");
		base.OnStopHost ();
	}

	public override void OnStopClient ()
	{
		Debug.Log ("OnStopClient");
		base.OnStopClient ();
	}
}
