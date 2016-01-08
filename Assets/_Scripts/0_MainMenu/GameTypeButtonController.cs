using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTypeButtonController : MonoBehaviour
{
	public GameType gameType;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			GameController.instance.gameType = gameType;
		});
	}

	void Start ()
	{
		GameController.instance.OnGameTypeChanged += UpdateActive;
		GameController.instance.InvokeGameTypeChanged (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			GameController.instance.OnGameTypeChanged -= UpdateActive;
		}
	}

	void UpdateActive (GameType gameType)
	{
		gameObject.SetActive (gameType == GameType.NONE_SELECTED);
	}

}
