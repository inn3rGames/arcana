using System.Collections.Generic;
using UnityEngine;

public class MagicParser : MonoBehaviour
{
    public TextAsset DialogueFile;
    private Dictionary<string, string> _variables = new Dictionary<string, string>();
    private Dictionary<string, Queue<string>> _blocks = new Dictionary<string, Queue<string>>();

    void Start()
    {
        DialogueFile = Resources.Load<TextAsset>("Data/script2");
        GenerateCommands();
    }

    private void GenerateCommands()
    {
        string content = DialogueFile.text;
        string[] contentLines = content.Split(System.Environment.NewLine.ToCharArray());

        bool foundBlock = false;
        Queue<string> currentBlockContent = new Queue<string>();

        for (int i = 0; i < contentLines.Length; i++)
        {
            string contentLine = contentLines[i];

            if (contentLine.StartsWith("$"))
            {
                string key = contentLine.Substring(1, contentLine.IndexOf(" "));
                string value = contentLine.Replace("$" + key, "");
                _variables.Add(key, value);
            }

            if (foundBlock == true)
            {
                if (contentLine.StartsWith("block"))
                {
                    foundBlock = false;
                    currentBlockContent.Clear();
                }else{
                    currentBlockContent.Enqueue(contentLine);
                }
            }

            if (contentLine.StartsWith("block"))
            {
                foundBlock = true;

                string value = contentLine.Replace("block ", "");
                currentBlockContent.Enqueue(value);

                _blocks.Add("block" + (i + 1).ToString(), currentBlockContent);
            }

        }

        LogDictionaries(_variables);
    }

    private void LogDictionaries(Dictionary<string, string> dictionary)
    {
        foreach (var pair in dictionary)
        {
            Debug.Log($"{pair.Key} {pair.Value}");
        }
    }
}
