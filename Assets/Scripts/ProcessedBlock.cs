using System.Collections.Generic;

public class ProcessedBlock
{
    public int Index;
    public string Name;
    public List<Command> Commands;

    public ProcessedBlock(int index, string name, List<Command> commands)
    {
        Index = index;
        Name = name;
        Commands = commands;
    }
}
