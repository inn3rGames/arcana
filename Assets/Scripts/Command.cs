public class Command
{
    public string Type;
    public string Content;
    public string Character;
    public string ChoiceBlock;

    public Command(
        string type,
        string contet,
        string character = "none",
        string choiceBlock = "none"
    )
    {
        Type = type;
        Content = contet;
        Character = character;
        ChoiceBlock = choiceBlock;
    }
}
