namespace DocuSign.MyHR.Domain
{
    public class User
    {
        public User()
        {
        }

        public User(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public User(string id, string name, LoginType loginType) : this(id, name)
        {
            LoginType = loginType;
        }
        
        public string Id { get; set; }

        public string Name { get; set; } 

        public LoginType LoginType { get; set; }
    }
}