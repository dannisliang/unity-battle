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
		//text.text = Social.localUser.userName + "(" + Social.localUser.state + ")";
		text.text = GameController.gamesPlatform.localUser.userName + "(" + GameController.gamesPlatform.localUser.state + ")";
	}
}
