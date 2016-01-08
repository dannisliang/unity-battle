using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameModePanelController : MonoBehaviour
{
	public GameType gameType;

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
		gameObject.SetActive (gameType == this.gameType);
	}
}
