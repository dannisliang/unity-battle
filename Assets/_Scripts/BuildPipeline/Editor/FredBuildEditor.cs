using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Threading;
using System.IO;

public class FredBuildEditor
{
	static bool executing;

	[MenuItem ("FRED/Build %&b")]
	public static void BuildGame ()
	{
		if (executing) {
			UnityEngine.Debug.LogError ("ALREADY EXECUTING !!");
			return;
		}
		executing = true;

		if (PlayerSettings.keystorePass.Length == 0 || PlayerSettings.keyaliasPass.Length == 0) {
			UnityEngine.Debug.LogError ("Need keystore and key alias passwords !!");
			return;
		}

		ClearLog ();
		UnityEngine.Debug.Log ("===================== CUSTOM BUILD =====================\n");

		string apk = PlayerSettings.bundleIdentifier + ".apk";

		if (File.Exists (apk)) {
			File.Delete (apk);
		}
		BuildPipeline.BuildPlayer (SceneMaster.buildLevels, apk, BuildTarget.Android, BuildOptions.None);

		if (File.Exists (apk)) {
			PostBuild ();
//			new Thread (new ThreadStart (PostBuild)).Start ();
		}
	}

	static void PostBuild ()
	{
		UnityEngine.Debug.Log ("Installing on devices …");
		Execute ("/bin/bash", "-lc", "./reinstall.sh");
		UnityEngine.Debug.Log ("===================== CUSTOM BUILD DONE =====================");
	}

	static int Execute (string cmd, params string[] args)
	{
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

		// Show two output lines at a time in Editor
		string output = "";
		proc.OutputDataReceived += new DataReceivedEventHandler (
			(sender, evt) => {
				if (evt.Data != null && evt.Data.Length > 0) {
					if (output != null) {
						UnityEngine.Debug.Log (output + "\n" + evt.Data);
						output = null;
					} else {
						output = evt.Data;
					}
				}
			}
		);
		proc.Start ();
		proc.BeginOutputReadLine ();
		var error = proc.StandardError.ReadToEnd ();

		proc.WaitForExit ();

		var exitCode = proc.ExitCode;

		if (output != null) {
			UnityEngine.Debug.Log (output);
		}
		if (error.Length > 0) {
			UnityEngine.Debug.LogError (error);
		}

		if (exitCode == 0) {
			UnityEngine.Debug.Log ("$ " + cmd + " " + joinedArgs + "\n==> OK");
		} else {
			UnityEngine.Debug.LogError ("$ " + cmd + " " + joinedArgs + "\n==> " + exitCode);
		}

		executing = false;
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
