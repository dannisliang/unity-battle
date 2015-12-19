using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardboardVrModeToggleController : MonoBehaviour,IPointerClickHandler//, IPointerEnterHandler,IPointerExitHandler
{

	//	public void OnPointerEnter (PointerEventData eventData)
	//	{
	//		Debug.Log ("OnPointerEnter");
	//	}
	//
	//	public void OnPointerExit (PointerEventData eventData)
	//	{
	//		Debug.Log ("OnPointerExit");
	//	}

	public void OnPointerClick (PointerEventData eventData)
	{
		Utils.IgnoreCurrentFire1 ();
		Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
	}
}
