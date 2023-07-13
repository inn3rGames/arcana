using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Image _backgroundImage;
    private Image _characterImage;

    private Dictionary<string, Sprite> _spriteDictionary = new Dictionary<string, Sprite>();

    private Sprite _forest;
    private Sprite _player;
    private Sprite _asra;
    private Sprite _snowman;

    private TMP_Text dialogueText;
    private int _blockIndex;
    private int _commandIndex;
    private List<ProcessedBlock> _processedBlocks;

    void Awake()
    {
        _backgroundImage = GameObject
            .FindGameObjectWithTag("BackgroundImage")
            .GetComponent<Image>();
        _characterImage = GameObject.FindGameObjectWithTag("CharacterImage").GetComponent<Image>();

        // Load sprites
        _forest = Resources.Load<Sprite>("Art/Forest");
        _spriteDictionary.Add("Forest", _forest);

        _player = Resources.Load<Sprite>("Art/Player");
        _spriteDictionary.Add("Player", _player);

        _asra = Resources.Load<Sprite>("Art/Asra");
        _spriteDictionary.Add("Asra", _asra);

        _snowman = Resources.Load<Sprite>("Art/Snowman");
        _spriteDictionary.Add("Snowman", _snowman);
    }

    void Start()
    {
        dialogueText = GameObject
            .FindGameObjectWithTag("DialogueText")
            .GetComponent<TextMeshProUGUI>();

        _blockIndex = 0;
        _commandIndex = 0;

        _processedBlocks = DialogueParser.ProcessedBlocks;

        // Initialize scene
        ExecuteCommands();
        SetText();
    }

    public void Click()
    {
        Next();
        ExecuteCommands();
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

                StoryNavigator.EndStory();
            }
        }
    }

    void SetText()
    {
        string currentContent = _processedBlocks[_blockIndex].Commands[_commandIndex].Content;
        dialogueText.text =
            currentContent + " " + _blockIndex.ToString() + " " + _commandIndex.ToString();
    }

    void ExecuteCommands()
    {
        Command curentCommand = _processedBlocks[_blockIndex].Commands[_commandIndex];

        if (curentCommand.Type.Equals("show"))
        {
            _characterImage.enabled = true;
            _characterImage.sprite = _spriteDictionary[curentCommand.Content];
        }

        if (curentCommand.Type.Equals("hide"))
        {
            _characterImage.enabled = false;
        }

        if (curentCommand.Type.Equals("showBG"))
        {
            _backgroundImage.enabled = true;
            _backgroundImage.sprite = _spriteDictionary[curentCommand.Content];
        }

        if (curentCommand.Type.Equals("call")) { }

        if (curentCommand.Type.Equals("choice")) { }

        if (curentCommand.Type.Equals("dialogue")) { }
    }
}
