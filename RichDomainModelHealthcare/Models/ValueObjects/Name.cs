namespace RichDomainModelHealthcare.Models.ValueObjects {
    public class Name {
        public Guid NameId { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public Name(string firstName, string lastName) {
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString() {
            return $"{FirstName} {LastName}";
        }

        // Override ToString, Equals, and GetHashCode as needed
    }
}
