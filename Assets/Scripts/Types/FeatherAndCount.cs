// a simple struct that represents a resource and a count.
[System.Serializable]
public struct FeatherAndCount
{
    public FeatherAndCount(FeatherType type, int count)
    {
        this.type = type;
        this.count = count;
    }
    public FeatherType type;
    public int count;
}
