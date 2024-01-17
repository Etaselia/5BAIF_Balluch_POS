namespace RichDomainModelHealthcare.Models.ValueObjects {
    public class Address {
        public Guid AddressId { get; set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }

        public Address(string street, string city, string state, string zipCode) {
            AddressId = new Guid();
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        // Override ToString, Equals, and GetHashCode as needed
    }
}
