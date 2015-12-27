using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UiTextLogHandler : ILogHandler
{
	Text statusLogText;

	public UiTextLogHandler (Text statusLogText)
	{
		this.statusLogText = statusLogText;
		statusLogText.text = "";
	}

	public void LogFormat (LogType logType, UnityEngine.Object context, string format, params object[] args)
	{
		Debug.logger.logHandler.LogFormat (logType, context, format, args);
		statusLogText.text += "\nLogFormat() " + Time.frameCount + " logType=" + logType + ", context=" + context + ", format=" + format + ", args=" + args;
	}

	public void LogException (Exception exception, UnityEngine.Object context)
	{
		Debug.logger.LogException (exception, context);
		statusLogText.text += "\nLogException() " + Time.frameCount + " exception=" + exception + ", context=" + context;
	}
}
