using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    private TextAsset _dialogueFile;
    private List<string[]> _rawBlocks = new List<string[]>();
    public static Dictionary<string, string> Variables = new Dictionary<string, string>();
    public static List<ProcessedBlock> ProcessedBlocks = new List<ProcessedBlock>();

    void Awake()
    {
        //Reset static properties when reloading scene
        Variables = new Dictionary<string, string>();
        ProcessedBlocks = new List<ProcessedBlock>();

        // Load and process script
        _dialogueFile = Resources.Load<TextAsset>("Data/script");
        FindBlocksAndVariables();
        ProcessBlocks();
        //LogProcessedBlocks(ProcessedBlocks);
    }

    private void FindBlocksAndVariables()
    {
        // Load text file
        string content = _dialogueFile.text;
        string[] contentLines = content.Split(System.Environment.NewLine.ToCharArray());

        bool foundBlock = false;
        List<string> rawBlock = new List<string>();

        for (int i = 0; i < contentLines.Length; i++)
        {
            string contentLine = contentLines[i];

            // Find variables
            if (contentLine.StartsWith("$"))
            {
                string key = contentLine.Substring(1, contentLine.IndexOf(" ")).Trim();
                string value = contentLine.Replace("$" + key, "").Trim();
                Variables.Add(key, value);
            }

            // Process the founded raw block
            if (foundBlock == true)
            {
                // Look for a new block if the founded block no longer has commands
                if (contentLine.StartsWith("block") || i == contentLines.Length - 1)
                {
                    foundBlock = false;

                    // Use an array to deep copy our content
                    _rawBlocks.Add(rawBlock.ToArray());
                    rawBlock.Clear();
                }
                // Add commands to the founded block
                else
                {
                    rawBlock.Add(contentLine);
                }
            }

            // Find a raw block
            if (contentLine.StartsWith("block"))
            {
                foundBlock = true;

                // Add block start from text line
                string id = (i + 1).ToString();
                rawBlock.Add(id);

                // Add block title
                string title = contentLine.Remove(contentLine.IndexOf("block "), ("block ").Length);
                rawBlock.Add(title);
            }
        }
    }

    private void ProcessBlocks()
    {
        for (int i = 0; i < _rawBlocks.Count; i++)
        {
            List<Command> processedCommands = ExtractCommandsFromRawBlock(_rawBlocks[i]);
            ProcessedBlocks.Add(
                new ProcessedBlock(
                    i,
                    Int32.Parse(_rawBlocks[i][0]),
                    _rawBlocks[i][1],
                    processedCommands
                )
            );
        }
    }

    private List<Command> ExtractCommandsFromRawBlock(string[] rawBlock)
    {
        List<Command> processedCommands = new List<Command>();

        foreach (string contentLine in rawBlock)
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
            Debug.Log($"{pair.Key}{pair.Value}");
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
                $"|||{processedCommands[i].Type} {processedCommands[i].Character} {processedCommands[i].Content} {processedCommands[i].ChoiceBlock}|||"
            );
        }
    }

    // Log processed blocks
    private void LogProcessedBlocks(List<ProcessedBlock> processedBlock)
    {
        for (var i = 0; i < processedBlock.Count; i++)
        {
            Debug.Log(
                $"~~~BLOCK START index={processedBlock[i].Index}; textLineStart={processedBlock[i].TextLineStart}; name={processedBlock[i].Name}; totalCommands={processedBlock[i].Commands.Count};~~~"
            );
            LogProcessedCommands(processedBlock[i].Commands);
            Debug.Log(
                $"~~~BLOCK END index={processedBlock[i].Index}; textLineStart={processedBlock[i].TextLineStart}; name={processedBlock[i].Name}; totalCommands={processedBlock[i].Commands.Count};~~~"
            );
        }
    }
}
