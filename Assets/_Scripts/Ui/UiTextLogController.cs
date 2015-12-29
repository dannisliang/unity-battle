using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UiTextLogController : MonoBehaviour
{
	#if UNITY_EDITOR
	static int MAX_ENTRIES = 3;
	#else
	static int MAX_ENTRIES = 10;
	#endif

	LinkedList<string> buf = new LinkedList<string> ();
	Text text;

	void Awake ()
	{
		//Application.stackTraceLogType = StackTraceLogType.ScriptOnly;
		text = GetComponent<Text> ();
		buf.Clear ();
	}

	void OnEnable ()
	{
		Application.logMessageReceivedThreaded += HandleLog;
	}

	void OnDisable ()
	{
		Application.logMessageReceivedThreaded -= HandleLog;
	}

	public void ClearLog ()
	{
		buf.Clear ();
		text.text = "";
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