// a simple struct that represents a product and a count.
public struct PotionAndCount
{
    public PotionAndCount(PotionType type, int count)
    {
        this.type = type;
        this.count = count;
    }
    public PotionType type;
    public int count;
}
