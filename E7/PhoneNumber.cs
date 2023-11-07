namespace E7;

class PhoneNr : IEquatable<PhoneNr>, IComparable<PhoneNr>, IComparable
{
    public long Vorwahl { get; }
    public long Telefonnummer { get; }

    public PhoneNr(long vorwahl, long telefonnummer)
    {
        Vorwahl = vorwahl;
        Telefonnummer = telefonnummer;
    }

    public override string ToString() => $"0{Vorwahl}/{Telefonnummer}";
    
    // This is a pretty ridiculous copy paste job, I mean this type of usecase for something like this seems of
    // I would understand doing it for a library but come on, here it just screams taking care of people that don't know the Equals operator exists
    public static bool operator >(PhoneNr left, PhoneNr right)
    {
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        return left.CompareTo(right) > 0;
    }

    public static bool operator <(PhoneNr left, PhoneNr right)
    {
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        return left.CompareTo(right) < 0;
    }

    public static bool operator >=(PhoneNr left, PhoneNr right)
    {
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <=(PhoneNr left, PhoneNr right)
    {
        if (ReferenceEquals(left, null))
        {
            return false;
        }
        return left.CompareTo(right) <= 0;
    }


    public bool Equals(PhoneNr other)
    {
    
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Vorwahl == other.Vorwahl && Telefonnummer == other.Telefonnummer;
    }
    
    public bool Equals(object other) {
        try {
            PhoneNr otherCast = (PhoneNr) other;
            if (ReferenceEquals(null, otherCast)) return false;
            if (ReferenceEquals(this, otherCast)) return true;
            return Vorwahl == otherCast.Vorwahl && Telefonnummer == otherCast.Telefonnummer;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
        
    }

    public int CompareTo(PhoneNr other)
    {
        if (ReferenceEquals(other, null)) return 1;
        int vorwahlComparison = Vorwahl.CompareTo(other.Vorwahl);
        if (vorwahlComparison != 0) return vorwahlComparison;
        return Telefonnummer.CompareTo(other.Telefonnummer);
    }

    public int CompareTo(object obj)
    {
        if (obj is null) return 1;
        if (obj is PhoneNr other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PhoneNr)}");
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Vorwahl, Telefonnummer);
    }
}