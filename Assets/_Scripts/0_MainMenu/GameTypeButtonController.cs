using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTypeButtonController : MonoBehaviour
{
	public GameType gameType;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			Game.instance.NewGame (gameType);
		});
	}

	void Start ()
	{
		Game.instance.OnGameStateChange += UpdateActive;
	}

	void OnDestroy ()
	{
		if (SceneMaster.quitting) {
			return;
		}
		Game.instance.OnGameStateChange -= UpdateActive;
	}

	void UpdateActive (GameState gameState)
	{
		gameObject.SetActive (gameState == GameState.NEED_TO_SELECT_GAME_TYPE);
	}

}
