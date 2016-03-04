using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Button))]
public class GameTypeButtonController : MonoBehaviour
{
	public GameType gameType;
	public KeyCode keyCode;

	void Awake ()
	{
		GetComponent<Button> ().onClick.AddListener (delegate {
			StartNewGame ();
		});
	}

	void Start ()
	{
		Game.instance.OnGameStateChange += UpdateActive;
	}

	void Update ()
	{
		if (Input.GetKeyDown (keyCode)) {
			StartNewGame ();
		}
	}

	void StartNewGame ()
	{
		Game.instance.NewGame (gameType);
	}

	void OnDestroy ()
	{
		if (Game.instance != null) {
			Game.instance.OnGameStateChange -= UpdateActive;
		}
	}

	void UpdateActive (GameState gameState)
	{
		gameObject.SetActive (gameState == GameState.SELECTING_GAME_TYPE);
	}

}
