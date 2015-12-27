using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardboardAssistantController : MonoBehaviour
{
	public Toggle cardboardToggle;

	void Awake ()
	{
		Cardboard.SDK.TapIsTrigger = false;
	}

}
