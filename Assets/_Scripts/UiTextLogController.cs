using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiTextLogController : MonoBehaviour
{

	Text text;

	void Awake ()
	{
		//Application.stackTraceLogType = StackTraceLogType.ScriptOnly;
		text = GetComponent<Text> ();
		text.text = "";
	}

	void OnEnable ()
	{
		// TODO: Make thread safe and use logMessageReceivedThreaded
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable ()
	{
		// TODO: Make thread safe and use logMessageReceivedThreaded
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog (string msg, string stackTrace, LogType type)
	{
		text.text += "\n" + (type == LogType.Log ? "" : type + " ");
		text.text += msg;
		//text.text += stackTrace;
	}

}