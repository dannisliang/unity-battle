[System.Serializable]
public struct BoatConfiguration
{
	public int size;
	public string designation;

	public BoatConfiguration (int size, string designation)
	{
		this.size = size;
		this.designation = designation;
	}
}

