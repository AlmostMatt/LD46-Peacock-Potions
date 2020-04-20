// a simple struct that represents a product and a count.
public struct ProductAndCount
{
    public ProductAndCount(ProductType type, int count)
    {
        this.type = type;
        this.count = count;
    }
    public ProductType type;
    public int count;
}
