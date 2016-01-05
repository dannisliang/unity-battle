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

	public override string ToString ()
	{
		return designation + " (" + size + " " + (size == 1 ? "unit" : "units") + ")";
	}
}

