using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class CurrentUserTextController : MonoBehaviour
{

	Text text;

	void Start ()
	{
		text = GetComponent<Text> ();
	}

	void Update ()
	{
		text.text = PlayGamesPlatform.Instance.localUser.userName + "(" + PlayGamesPlatform.Instance.localUser.state + ")";
	}
}
