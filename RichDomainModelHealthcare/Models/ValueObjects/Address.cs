namespace RichDomainModelHealthcare.Models.ValueObjects
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Parameterless constructor needed for model binding
        public Address() { }

        public Address(string street, string city, string state, string zipCode)
        {
            AddressId = Guid.NewGuid();
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }
    }
}