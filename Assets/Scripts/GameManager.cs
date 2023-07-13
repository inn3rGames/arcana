using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private TMP_Text dialogueText;
    private int _blockIndex;
    private int _commandIndex;
    private List<ProcessedBlock> _processedBlocks;

    // Start is called before the first frame update
    void Start()
    {
        dialogueText = GetComponent<TextMeshProUGUI>();

        _blockIndex = 0;
        _commandIndex = 0;

        _processedBlocks = DialogueParser.ProcessedBlocks;

        SetText();
    }

    public void Click()
    {
        Next();
        SetText();
    }

    void Next()
    {
        _commandIndex += 1;

        if (_commandIndex > _processedBlocks[_blockIndex].Commands.Count - 1)
        {
            _commandIndex = 0;
            _blockIndex += 1;

            if (_blockIndex > _processedBlocks.Count - 1)
            {
                _blockIndex = 0;
                _commandIndex = 0;
            }
        }
    }

    void SetText()
    {
        string currentContent = _processedBlocks[_blockIndex].Commands[_commandIndex].Content;
        dialogueText.text =
            currentContent + " " + _blockIndex.ToString() + " " + _commandIndex.ToString();
    }
}
