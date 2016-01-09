using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Reflection;
using System;

public class MyBuildEditor
{
	static bool executing;

	[MenuItem ("FRED/Build %&b")]
	public static void BuildGame ()
	{
		ClearLog ();
		UnityEngine.Debug.Log (typeof(MyBuildEditor).Name + " …");

		string path = PlayerSettings.bundleIdentifier + ".apk";
//		BuildPipeline.BuildPlayer (SceneMaster.buildLevels, path, BuildTarget.Android, BuildOptions.None);

		Execute ("/bin/ls", "-l", path);
		UnityEngine.Debug.Log ("Done");
	}

	static int Execute (string cmd, params string[] args)
	{
		if (executing) {
			UnityEngine.Debug.LogError ("ALREADY EXECUTING !!");
			return 1;
		}
		executing = true;
		string joinedArgs = string.Join (" ", args);
		Process proc = new Process ();
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.CreateNoWindow = false;
		proc.StartInfo.ErrorDialog = true;
		proc.StartInfo.RedirectStandardError = true;
		proc.StartInfo.RedirectStandardInput = true;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.FileName = cmd;
		proc.StartInfo.Arguments = joinedArgs;
		proc.Start ();

		proc.OutputDataReceived += new DataReceivedEventHandler (
			(sender, evt) => { 
				if (evt.Data != null) {
					UnityEngine.Debug.Log (evt.Data);
				}
			}
		);
		proc.BeginOutputReadLine ();
		var error = proc.StandardError.ReadToEnd ();
		proc.WaitForExit ();
		executing = false;

		var exitCode = proc.ExitCode;
		if (exitCode != 0 || error.Length > 0) {
			UnityEngine.Debug.LogError (cmd + " " + joinedArgs + "\n" +
			"==> Exit code " + exitCode);
			UnityEngine.Debug.LogError (error);
			throw new Exception ("Non-zero exit code\n");
		}
		return exitCode;
	}

	// Since UnityEngine.Debug.ClearDeveloperConsole() doesn't work
	static void ClearLog ()
	{
		Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
		Type type = assembly.GetType ("UnityEditorInternal.LogEntries");
		MethodInfo method = type.GetMethod ("Clear");
		method.Invoke (new object (), null);
	}

}
