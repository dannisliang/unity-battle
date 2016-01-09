using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class LayerInfo
{
	public string layerName;
	public int layer;
	public LayerMask layerMask;

	public LayerInfo (string layerName)
	{
		this.layerName = layerName;
		layer = LayerMask.NameToLayer (layerName);
		Assert.AreEqual (layerName, LayerMask.LayerToName (layer));
		layerMask = 1 << layer;
		Assert.IsTrue (layerMask == LayerMask.GetMask (layerName));
	}

	public override string ToString ()
	{
		return "LayerInfo(" + layerName + ", layer " + layer + ", mask " + layerMask + ")";
	}
}
