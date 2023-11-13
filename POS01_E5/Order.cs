using System.Reflection;

namespace E5;

public class Order
{
    // There is no way to modify a IReadOnlyList in any way if there is no setter
    // The way this has to be implemented here is kinda bad, you might as well just use a private set and not mess with these workarounds
    // This is literally teaching to make spaghetti code and I DO NOT LIKE IT
    public IReadOnlyList<Product> Products 
    {
        get{return InternalList.AsReadOnly();}
        
    }
    public decimal InvoiceAmount
    {
        get { return InternalInvoice; }
    }

    private PaymentProvider paymentProvider;
    
    private List<Product> InternalList { get; set; }
    private decimal InternalInvoice { get; set; }

    public Order(PaymentProvider paymentProvider)
    {
        this.paymentProvider = paymentProvider;
        InternalList = new List<Product>();
        InternalInvoice = 0;
    }

    public void AddProduct(Product p)
    {
        InternalList.Add(p);
        InternalInvoice += p.Price;
    }

    public bool Checkout()
    {
        
        if (paymentProvider.Pay(InvoiceAmount))
        {
            InternalList.Clear();
            return true;
        }
        return false;
    }
}