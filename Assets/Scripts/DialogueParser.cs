using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public TextAsset DialogueFile;
    private Dictionary<string, string> _variables = new Dictionary<string, string>();
    private List<string[]> _blocks = new List<string[]>();

    void Start()
    {
        DialogueFile = Resources.Load<TextAsset>("Data/script");
        FindBlocksAndVariables();

        for (int i = 0; i < _blocks.Count; i++)
        {
            ExtractCommandsFromBlock(_blocks[i]);
        }
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

    private void ExtractCommandsFromBlock(string[] block)
    {
        List<Command> processedBlock = new List<Command>();

        foreach (string contentLine in block)
        {
            if (contentLine.Contains("show "))
            {
                ExtractStandardCommand("show", "show ", contentLine, processedBlock);
            }

            if (contentLine.Contains("hide "))
            {
                ExtractStandardCommand("hide", "hide ", contentLine, processedBlock);
            }

            if (contentLine.Contains("showBG "))
            {
                ExtractStandardCommand("showBG", "showBG ", contentLine, processedBlock);
            }

            if (contentLine.Contains("call "))
            {
                ExtractStandardCommand("call", "call ", contentLine, processedBlock);
            }

            if (contentLine.Contains("choice "))
            {
                ExtractStandardCommand("choice", "choice ", contentLine, processedBlock);
            }

            if (contentLine.Contains(": "))
            {
                ExtractDialogueCommand("dialogue", ": ", contentLine, processedBlock);
            }
        }

        LogList(processedBlock);
    }

    private void ExtractStandardCommand(
        string commandType,
        string commandSpacing,
        string contentLine,
        List<Command> processedBlock
    )
    {
        string removeCommand = contentLine.Remove(
            contentLine.IndexOf(commandSpacing),
            commandSpacing.Length
        );
        string extractContent = removeCommand.Trim();

        Command processedCommand = new Command(commandType, extractContent);
        processedBlock.Add(processedCommand);
    }

    private void ExtractDialogueCommand(
        string commandType,
        string commandSpacing,
        string contentLine,
        List<Command> processedBlock
    )
    {
        string character = contentLine.Substring(0, contentLine.IndexOf(commandSpacing)).Trim();
        string removeCommand = contentLine.Remove(
            contentLine.IndexOf(character + commandSpacing),
            (character + commandSpacing).Length
        );
        string extractContent = removeCommand.Trim();

        Command processedCommand = new Command(commandType, extractContent, character);
        processedBlock.Add(processedCommand);
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

    // Log lists
    private void LogList(List<Command> processedBlock)
    {
        Debug.Log("START");
        for (var i = 0; i < processedBlock.Count; i++)
        {
            Debug.Log(
                $"{processedBlock[i].Type} {processedBlock[i].Character} {processedBlock[i].Content}"
            );
        }
        Debug.Log("END");
    }
}
