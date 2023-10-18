namespace E5;

public abstract class PaymentProvider
{
    public decimal Limit
    {
        get { return InternalLimit; }
    }
    
    private decimal InternalLimit { get; set; }

    public PaymentProvider(decimal limit)
    {
        InternalLimit = limit;
    }

    public abstract bool Pay(decimal amount);
}
