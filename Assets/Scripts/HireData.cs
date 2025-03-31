using System;

[Serializable]
public class HireData
{
    public string name;
    public int age;
    public string dateOfBirth;
    public string qualifications;
    public bool acceptOrDeny;

    public HireData(string name, int age, string dateOfBirth, string qualifications, bool acceptOrDeny)
    {
        this.name = name;
        this.age = age;
        this.dateOfBirth = dateOfBirth;
        this.qualifications = qualifications;
        this.acceptOrDeny = acceptOrDeny;
    }
}
