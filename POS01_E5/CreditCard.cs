namespace E5;

public class CreditCard : PaymentProvider
{
    public string Number { get; }

    public CreditCard(decimal limit, string number) : base(limit)
    {
        Number = number;
    }

    public override bool Pay(decimal amount)
    {
        if (amount > Limit)
        {
            return false;
        }
        return true;
    }
}