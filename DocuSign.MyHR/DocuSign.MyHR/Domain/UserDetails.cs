using System;

namespace DocuSign.MyHR.Domain
{
    public class UserDetails : User
    {
        public UserDetails()
        {
        }

        public UserDetails(string id, string name, string email, string firstName, string lastName, DateTime hireDate, string profileId, Address address) : base(id, name)
        {
            Address = address;
            ProfileId = profileId;
            FirstName = firstName;
            LastName = lastName;
            HireDate = hireDate;
            Email = email;
        }
        public string Email { get; set; }

        public Address Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime HireDate { get; set; }

        public string ProfileImage { get; set; }

        public string ProfileId { get; set; }
    }
}