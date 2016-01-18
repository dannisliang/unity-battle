using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
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
		if (Game.instance != null) {
			Game.instance.OnGameStateChange -= UpdateActive;
		}
	}

	void UpdateActive (GameState gameState)
	{
		gameObject.SetActive (gameState == GameState.NEED_TO_SELECT_GAME_TYPE);
	}

}
