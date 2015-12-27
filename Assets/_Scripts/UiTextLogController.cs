using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiTextLogController : MonoBehaviour
{

	Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
		text.text = "";
	}

	void OnEnable ()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable ()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog (string msg, string stackTrace, LogType type)
	{
		text.text += "\n" + (type == LogType.Log ? "" : type + " ");
		text.text += msg;
		//text.text += stackTrace;
	}

}