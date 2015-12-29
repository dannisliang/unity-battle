using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiTextLogController : MonoBehaviour
{

	LinkedList<string> buf;
	Text text;

	void Awake ()
	{
		//Application.stackTraceLogType = StackTraceLogType.ScriptOnly;
		text = GetComponent<Text> ();
		text.text = "";
		buf = new LinkedList<string> ();
		buf.AddLast ("…");
#if UNITY_EDITOR
		Destroy (gameObject);
#endif
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
		string t = type == LogType.Log ? "" : type + " ";
		t += msg; 
		//t += stackTrace;
		ThreadSafeAppend (t);
	}

	void ThreadSafeAppend (string msg)
	{
		buf.AddLast (msg);
		if (buf.Count > 10) {
			buf.RemoveFirst ();
		}
		string t = "";
		foreach (string m in buf) {
			t += m + "\n";
		}
		text.text = t;
	}
}