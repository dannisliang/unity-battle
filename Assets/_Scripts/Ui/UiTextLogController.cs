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
		Application.logMessageReceivedThreaded += HandleLog;
	}

	void OnDisable ()
	{
		Application.logMessageReceivedThreaded -= HandleLog;
	}

	void HandleLog (string msg, string stackTrace, LogType type)
	{
		string t = "\n" + (type == LogType.Log ? "" : type + " ");
		t += msg; 
		t += stackTrace;
		ThreadSafeAppend (t);
	}

	void ThreadSafeAppend (string msg)
	{
		text.text += msg;
	}
}