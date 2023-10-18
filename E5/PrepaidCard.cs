namespace E5;

public class PrepaidCard : PaymentProvider
{
    public decimal Credit
    {
        get { return InternalCredit; }
    }

    public PrepaidCard(decimal limit, decimal credit) : base(limit)
    {
        InternalCredit = credit;
    }
    
    private decimal InternalCredit { get; set; }

    public void Charge(decimal amount)
    {
        if (amount > Limit)
        {
            throw new Exception("Insufficient credit.");
        }
        InternalCredit += amount;
    }

    public override bool Pay(decimal amount)
    {
        if (amount > Credit)
        {
            return false;
        }

        InternalCredit -= amount;
        return true;
    }
}