using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public TextAsset DialogueFile;
    private Dictionary<string, string> _variables = new Dictionary<string, string>();
    private List<string[]> _blocks = new List<string[]>();
    public List<ProcessedBlock> ProcessedBlocks = new List<ProcessedBlock>();

    void Start()
    {
        DialogueFile = Resources.Load<TextAsset>("Data/script");
        FindBlocksAndVariables();
        ProcessBlocks();
        LogProcessedBlocks(ProcessedBlocks);
    }

    private void FindBlocksAndVariables()
    {
        // Load text file
        string content = DialogueFile.text;
        string[] contentLines = content.Split(System.Environment.NewLine.ToCharArray());

        bool foundBlock = false;
        Queue<string> currentBlockContent = new Queue<string>();

        for (int i = 0; i < contentLines.Length; i++)
        {
            string contentLine = contentLines[i];

            // Find variables
            if (contentLine.StartsWith("$"))
            {
                string key = contentLine.Substring(1, contentLine.IndexOf(" "));
                string value = contentLine.Replace("$" + key, "");
                _variables.Add(key, value);
            }

            // Process the founded block
            if (foundBlock == true)
            {
                // Look for a new block if the founded block no longer has commands
                if (contentLine.StartsWith("block") || i == contentLines.Length - 1)
                {
                    foundBlock = false;

                    // Use an array to deep copy our content
                    _blocks.Add(currentBlockContent.ToArray());
                    currentBlockContent.Clear();
                }
                // Add commands to the founded block
                else
                {
                    currentBlockContent.Enqueue(contentLine);
                }
            }

            // Find a block
            if (contentLine.StartsWith("block"))
            {
                foundBlock = true;

                // Add block index from text line
                string id = (i + 1).ToString();
                currentBlockContent.Enqueue(id);

                // Add block title
                string title = contentLine;
                currentBlockContent.Enqueue(title);
            }
        }
    }

    private void ProcessBlocks()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            List<Command> processedCommands = ExtractCommandsFromContent(_blocks[i]);
            ProcessedBlocks.Add(new ProcessedBlock(i, "demo", processedCommands));
        }
    }

    private List<Command> ExtractCommandsFromContent(string[] block)
    {
        List<Command> processedCommands = new List<Command>();

        foreach (string contentLine in block)
        {
            if (contentLine.Contains("show "))
            {
                ExtractStandardCommand("show", "show ", contentLine, processedCommands);
            }

            if (contentLine.Contains("hide "))
            {
                ExtractStandardCommand("hide", "hide ", contentLine, processedCommands);
            }

            if (contentLine.Contains("showBG "))
            {
                ExtractStandardCommand("showBG", "showBG ", contentLine, processedCommands);
            }

            if (contentLine.Contains("call "))
            {
                ExtractStandardCommand("call", "call ", contentLine, processedCommands);
            }

            if (contentLine.Contains("choice "))
            {
                ExtractChoiceCommand("choice", "choice ", contentLine, processedCommands);
            }

            if (contentLine.Contains(": "))
            {
                ExtractDialogueCommand("dialogue", ": ", contentLine, processedCommands);
            }
        }

        LogProcessedCommands(processedCommands);

        return processedCommands;
    }

    private void ExtractStandardCommand(
        string commandType,
        string commandSpacing,
        string contentLine,
        List<Command> processedCommands
    )
    {
        string removeCommand = contentLine.Remove(
            contentLine.IndexOf(commandSpacing),
            commandSpacing.Length
        );
        string extractContent = removeCommand.Trim();

        Command processedCommand = new Command(commandType, extractContent);
        processedCommands.Add(processedCommand);
    }

    private void ExtractDialogueCommand(
        string commandType,
        string commandSpacing,
        string contentLine,
        List<Command> processedCommands
    )
    {
        string character = contentLine.Substring(0, contentLine.IndexOf(commandSpacing)).Trim();
        string removeCommand = contentLine.Remove(
            contentLine.IndexOf(character + commandSpacing),
            (character + commandSpacing).Length
        );
        string extractContent = removeCommand.Trim();

        Command processedCommand = new Command(commandType, extractContent, character);
        processedCommands.Add(processedCommand);
    }

    private void ExtractChoiceCommand(
        string commandType,
        string commandSpacing,
        string contentLine,
        List<Command> processedCommands
    )
    {
        string removeCommand = contentLine.Remove(
            contentLine.IndexOf(commandSpacing),
            commandSpacing.Length
        );
        string choiceBlock = removeCommand.Substring(0, removeCommand.IndexOf(" ")).Trim();
        string extractContent = removeCommand
            .Remove(removeCommand.IndexOf(choiceBlock + " "), (choiceBlock + " ").Length)
            .Trim();

        Command processedCommand = new Command(commandType, extractContent, "none", choiceBlock);
        processedCommands.Add(processedCommand);
    }

    // Log dictionaries
    private void LogDictionaries(Dictionary<string, string> dictionary)
    {
        foreach (var pair in dictionary)
        {
            Debug.Log($"{pair.Key} {pair.Value}");
        }
    }

    // Log arrays
    private void LogArray(string[] array)
    {
        Debug.Log("START");
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i].ToString());
        }
        Debug.Log("END");
    }

    // Log processed commands
    private void LogProcessedCommands(List<Command> processedCommands)
    {
        for (var i = 0; i < processedCommands.Count; i++)
        {
            Debug.Log(
                $"COMMAND START {processedCommands[i].Type} {processedCommands[i].Character} {processedCommands[i].Content} {processedCommands[i].ChoiceBlock} COMMAND END"
            );
        }
    }

    // Log processed blocks
    private void LogProcessedBlocks(List<ProcessedBlock> processedBlock)
    {
        for (var i = 0; i < processedBlock.Count; i++)
        {
            Debug.Log(
                $"BLOCK START {processedBlock[i].Index} {processedBlock[i].Name} {processedBlock[i].Commands.Count} BLOCK END"
            );
            LogProcessedCommands(processedBlock[i].Commands);
        }
    }
}
