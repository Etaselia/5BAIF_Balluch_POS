using RichDomainModelHealthcare.Models.ValueObjects;
namespace RichDomainModelHealthcare.Models;

public abstract class Person
{
    public Guid Id { get; set; }
    public Name Name { get; set; } // Assuming Name is a value object
    public Address Address { get; set; } // Shared property
    public DateTime DateOfBirth { get; set; } // Shared property
    public string PhoneNumber { get; set; } // Shared property

    // Common constructor for shared properties
    protected Person(Name name, Address address, DateTime dateOfBirth, string phoneNumber)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
    }
}

