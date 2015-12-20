using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardboardVrModeToggleController : MonoBehaviour,IPointerClickHandler
{

	void Awake ()
	{
		Cardboard.SDK.TapIsTrigger = false;
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		Utils.IgnoreCurrentFire1 ();
		Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
	}
}
