﻿using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class FredBuildEditor : EditorWindow
{
	static string TWO_LINES = ".*\n.*\n";

	static bool executing = false;

	static FredBuildEditor ()
	{
		CheckPasswords ();
	}

	[MenuItem ("FRED/Build %&b")]
	static void BuildGame ()
	{
		DoBuildGame ();
//		ClearLog ();
//		UnityEngine.Debug.Log ("Build window ready\n");
//		EditorWindow.GetWindow (typeof(FredBuildEditor));
	}

	//	void OnGUI ()
	//	{
	//		GUILayout.Label ("Building is fun!");
	//
	//		GUI.SetNextControlName ("build_button");
	//		if (GUILayout.Button ("Build it!")) {
	//			GUILayout.Label ("Executing!");
	//			DoBuildGame ();
	//			Close ();
	//		}
	//		GUI.FocusControl ("build_button");
	//	}

	static void DoBuildGame ()
	{
		if (executing) {
			UnityEngine.Debug.LogError ("Already executing !!");
			return;
		}

		ClearLog ();
		CheckPasswords ();

		string apk = PlayerSettings.bundleIdentifier + ".apk";
		DateTime creationTime = File.GetCreationTime (apk);

		executing = true;
		BuildPipeline.BuildPlayer (SceneMaster.buildLevels, apk, BuildTarget.Android, BuildOptions.None);

		if (File.GetCreationTime (apk).Equals (creationTime)) {
			UnityEngine.Debug.LogError ("Failed to build " + apk);
			executing = false;
			return;
		}

		// InstallApk responsible for setting executing = false
		new Thread (new ThreadStart (InstallApk)).Start ();
	}

	static void InstallApk ()
	{
		UnityEngine.Debug.Log ("$ ./reinstall.sh");
		Execute ("/bin/bash", "-lc", "./reinstall.sh");
	}

	static int Execute (string cmd, params string[] args)
	{
		string joinedArgs = string.Join (" ", args);
		Process proc = new Process ();
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.CreateNoWindow = true;
		proc.StartInfo.ErrorDialog = false;
		proc.StartInfo.RedirectStandardError = true;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
		proc.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
		proc.StartInfo.FileName = cmd;
		proc.StartInfo.Arguments = joinedArgs;

		// Show two output lines at a time in Editor
		string output = "";
		proc.OutputDataReceived += new DataReceivedEventHandler (
			(sender, evt) => {
				if (evt.Data != null) {
					output += evt.Data + "\n";
					output = StripEmptyLines (output);
					// log two lines at a time
					foreach (Match match in Regex.Matches (output, TWO_LINES, RegexOptions.Multiline)) {
						UnityEngine.Debug.Log (PrefixOutput (match.Value));
						output = output.Substring (match.Value.Length);
					}
				}
			}
		);

		// Show two output lines at a time in Editor
		string error = "";
		proc.ErrorDataReceived += new DataReceivedEventHandler (
			(sender, evt) => {
				if (evt.Data != null) {
					error += evt.Data + "\n";
					error = StripEmptyLines (error);
					// log two lines at a time
					foreach (Match match in Regex.Matches (error, TWO_LINES, RegexOptions.Multiline)) {
						UnityEngine.Debug.LogError (PrefixOutput (match.Value));
						error = error.Substring (match.Value.Length);
					}
				}
			}
		);

		proc.Start ();
		proc.BeginOutputReadLine ();
		proc.BeginErrorReadLine ();
		proc.WaitForExit ();

		var exitCode = proc.ExitCode;

		// log any remaining output
		output = StripEmptyLines (output);
		if (output.Length > 0) {
			UnityEngine.Debug.Log (PrefixOutput (output));
		}

		// log any remaining error
		error = StripEmptyLines (error);
		if (error.Length > 0) {
			UnityEngine.Debug.LogError (PrefixOutput (error));
		}

		if (exitCode == 0) {
			UnityEngine.Debug.Log ("$ " + cmd + " " + joinedArgs + "\n==> OK");
		} else {
			UnityEngine.Debug.LogError ("$ " + cmd + " " + joinedArgs + "\n==> " + exitCode);
		}

		executing = false;
		return exitCode;
	}

	static string StripEmptyLines (string output)
	{
		output = Regex.Replace (output, "^\n+", "", RegexOptions.Multiline);
		output = Regex.Replace (output, "\n+", "\n", RegexOptions.Multiline);
		return output;
	}

	static object PrefixOutput (string output)
	{
		return ">  " + Regex.Replace (output, "\n", "\n>  ", RegexOptions.Multiline);
	}
		
	// Since UnityEngine.Debug.ClearDeveloperConsole() doesn't work
	static void ClearLog ()
	{
		Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
		Type type = assembly.GetType ("UnityEditorInternal.LogEntries");
		MethodInfo method = type.GetMethod ("Clear");
		method.Invoke (new object (), null);
	}

	static void CheckPasswords ()
	{
		if (PlayerSettings.keystorePass.Length == 0 || PlayerSettings.keyaliasPass.Length == 0) {
			string path = System.Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments) + "/.fred-build-info";
			string password = File.ReadAllText (path);
			PlayerSettings.keystorePass = password;
			PlayerSettings.keyaliasPass = password;
		}
	}

}
