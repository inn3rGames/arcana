using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public TextAsset DialogueFile;
    private Dictionary<string, string> _variables = new Dictionary<string, string>();
    private List<Queue<string>> _blocks = new List<Queue<string>>();

    void Start()
    {
        DialogueFile = Resources.Load<TextAsset>("Data/script");
        FindBlocksAndVariables();
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
                    _blocks.Add(currentBlockContent);
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

    // Log dictionaries
    private void LogDictionaries(Dictionary<string, string> dictionary)
    {
        foreach (var pair in dictionary)
        {
            Debug.Log($"{pair.Key} {pair.Value}");
        }
    }

    // Log queues
    private void LogQueues(Queue<string> queue)
    {
        Debug.Log("START");
        string[] arrayFromQueue = queue.ToArray();
        for (int i = 0; i < arrayFromQueue.Length; i++)
        {
            Debug.Log(arrayFromQueue[i].ToString());
        }
        Debug.Log("END");
    }
}
