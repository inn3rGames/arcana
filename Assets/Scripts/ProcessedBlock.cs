using System.Collections.Generic;

public class ProcessedBlock
{
    public int Index;
    public int TextLineStart;
    public string Name;
    public List<Command> Commands;

    public ProcessedBlock(int index, int textLineStart, string name, List<Command> commands)
    {
        Index = index;
        TextLineStart = textLineStart;
        Name = name;
        Commands = commands;
    }
}
