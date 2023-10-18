namespace E7;

record PhoneRecord(long Vorwahl, long Telefonnummer) : IComparable<PhoneRecord>
{
    public int CompareTo(PhoneRecord other)
    {
        if (ReferenceEquals(other, null)) return 1;
        int vorwahlComparison = Vorwahl.CompareTo(other.Vorwahl);
        if (vorwahlComparison != 0) return vorwahlComparison;
        return Telefonnummer.CompareTo(other.Telefonnummer);
    }
}