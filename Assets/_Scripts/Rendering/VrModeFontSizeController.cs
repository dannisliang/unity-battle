using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
public class VrModeFontSizeController : MonoBehaviour
{
	public int vrModeFontSize = 6;
	public int magicWindowFontSize = 12;

	void UpdateFontSize (bool vrMode)
	{
		gameObject.GetComponent<Text> ().fontSize = Cardboard.SDK.VRModeEnabled ? vrModeFontSize : magicWindowFontSize;
	}

}
