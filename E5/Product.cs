namespace E5;

public class Product
{
    public string Ean { get;}
    public string Name { get;}
    public decimal Price { get;}

    public Product(string ean, string name, decimal price)
    {
        Ean = ean;
        Name = name;
        Price = price;
    }
}