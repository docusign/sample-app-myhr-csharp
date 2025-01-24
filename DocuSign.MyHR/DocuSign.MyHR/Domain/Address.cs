namespace DocuSign.MyHR.Domain
{
    public class Address
    {
        public Address()
        {
        }

        public Address(
            string address1,
            string address2,
            string city,
            string country,
            string fax,
            string phone,
            string postalCode,
            string stateOrProvince)
        {
            Address1 = address1;
            Address2 = address2;
            City = city;
            Country = country;
            Fax = fax;
            Phone = phone;
            PostalCode = postalCode;
            StateOrProvince = stateOrProvince;
        }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Fax { get; set; }

        public string Phone { get; set; }

        public string PostalCode { get; set; }

        public string StateOrProvince { get; set; }
    }
}
