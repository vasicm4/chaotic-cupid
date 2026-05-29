namespace DefaultNamespace
;

public class Person
{
    string _username { get; set; }
    string _city { get; set; }
    int _age { get; set; }
    string _phoneNumber { get; set; }
    
    public Person(string username, string city, int age, string phoneNumber)
    {
        _username = username;
        _city = city;
        _age = age;
        _phoneNumber = phoneNumber;
    }
}