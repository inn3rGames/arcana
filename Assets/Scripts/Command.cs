public class Command
{
    public string Type;
    public string Content;
    public string Character;

    public Command(string type, string contet, string character = "none")
    {
        Type = type;
        Content = contet;
        Character = character;
    }
}
