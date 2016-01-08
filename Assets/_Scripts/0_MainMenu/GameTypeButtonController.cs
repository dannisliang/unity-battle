using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTypeButtonController : MonoBehaviour
{
	public GameType gameType;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.gameType = gameType;
		});
	}

	void Start ()
	{
		Game.instance.OnGameTypeChanged += UpdateActive;
		Game.instance.InvokeGameTypeChanged (UpdateActive);
	}

	void OnDestroy ()
	{
		if (!SceneMaster.quitting) {
			Game.instance.OnGameTypeChanged -= UpdateActive;
		}
	}

	void UpdateActive (GameType gameType)
	{
		gameObject.SetActive (gameType == GameType.NONE_SELECTED);
	}

}
