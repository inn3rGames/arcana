using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Image _backgroundImage;
    private Image _characterImage;
    private TMP_Text _dialogueText;
    private TMP_Text _characterNameText;
    private GameObject _choiceGroup;
    private GameObject _choiceArea;
    public GameObject ChoiceButtonPrefab;

    private Dictionary<string, Sprite> _spriteDictionary = new Dictionary<string, Sprite>();
    private Sprite _forest;
    private Sprite _player;
    private Sprite _asra;
    private Sprite _snowman;

    private int _blockIndex;
    private int _commandIndex;
    private bool _disableClick = false;
    private Dictionary<string, string> _variables;
    private List<ProcessedBlock> _processedBlocks;
    private Dictionary<string, ProcessedBlock> _blockLinks;

    [SerializeField]
    [Range(0.01F, 0.5F)]
    private float _typeWriterDelay = 0.05F;
    private bool _typeWriterActive = false;

    void Awake()
    {
        _backgroundImage = GameObject
            .FindGameObjectWithTag("BackgroundImage")
            .GetComponent<Image>();
        _characterImage = GameObject.FindGameObjectWithTag("CharacterImage").GetComponent<Image>();
        _dialogueText = GameObject
            .FindGameObjectWithTag("DialogueText")
            .GetComponent<TextMeshProUGUI>();
        _characterNameText = GameObject
            .FindGameObjectWithTag("CharacterNameText")
            .GetComponent<TextMeshProUGUI>();

        _choiceGroup = GameObject.FindGameObjectWithTag("ChoiceGroup");
        _choiceArea = GameObject.FindGameObjectWithTag("ChoiceArea");

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
        // Hide character image
        _characterImage.color = new Color32(0, 0, 0, 0);
        _characterImage.enabled = false;

        _blockIndex = 0;
        _commandIndex = 0;

        _variables = DialogueParser.Variables;
        _processedBlocks = DialogueParser.ProcessedBlocks;
        _blockLinks = DialogueParser.BlockLinks;

        // Initialize scene
        ExecuteCommands();
    }

    public void Click()
    {
        if (_typeWriterActive == true)
        {
            _typeWriterActive = false;
            StopAllCoroutines();

            Command curentCommand = _processedBlocks[_blockIndex].Commands[_commandIndex];
            curentCommand.Content = ReplaceVariable(curentCommand.Content);
            _dialogueText.text = curentCommand.Content;
        }
        else
        {
            if (_disableClick == false)
            {
                Next();
                ExecuteCommands();
            }
        }
    }

    void Next()
    {
        _commandIndex += 1;

        if (_commandIndex > _processedBlocks[_blockIndex].Commands.Count - 1)
        {
            _commandIndex = 0;
            _blockIndex += 1;

            //Prevent infinite navigation. Disable for endless play/testing.
            StoryNavigator.EndStory();

            if (_blockIndex > _processedBlocks.Count - 1)
            {
                _blockIndex = 0;
                _commandIndex = 0;

                StoryNavigator.EndStory();
            }
        }
    }

    void LogText()
    {
        string currentContent = _processedBlocks[_blockIndex].Commands[_commandIndex].Content;
        Debug.Log(currentContent + " " + _blockIndex.ToString() + " " + _commandIndex.ToString());
    }

    void ExecuteCommands()
    {
        LogText();

        Command curentCommand = _processedBlocks[_blockIndex].Commands[_commandIndex];

        _dialogueText.text = "";
        _characterNameText.text = "";
        _choiceGroup.SetActive(false);

        if (curentCommand.Type.Equals("show"))
        {
            _characterImage.color = new Color32(255, 255, 255, 255);
            _characterImage.enabled = true;
            _characterImage.sprite = _spriteDictionary[curentCommand.Content];
        }

        if (curentCommand.Type.Equals("hide"))
        {
            _characterImage.color = new Color32(0, 0, 0, 0);
            _characterImage.enabled = false;
        }

        if (curentCommand.Type.Equals("showBG"))
        {
            _backgroundImage.enabled = true;
            _backgroundImage.sprite = _spriteDictionary[curentCommand.Content];
        }

        if (curentCommand.Type.Equals("call"))
        {
            string blockKey = curentCommand.Content;
            ProcessedBlock destinationBlock = _blockLinks[blockKey];

            _blockIndex = destinationBlock.Index;
            _commandIndex = 0;

            ExecuteCommands();
        }

        if (curentCommand.Type.Equals("choice"))
        {
            _choiceGroup.SetActive(true);

            // Clean old choice buttons
            foreach (Transform child in _choiceArea.transform)
            {
                Destroy(child.gameObject);
            }

            // Create new choice buttons
            while (_commandIndex <= _processedBlocks[_blockIndex].Commands.Count - 1)
            {
                GameObject choiceContainer = Instantiate(ChoiceButtonPrefab, _choiceArea.transform);
                Button choiceButton = choiceContainer.GetComponentInChildren<Button>();
                TMP_Text choiceText = choiceButton.GetComponentInChildren<TMP_Text>();

                choiceText.text = _processedBlocks[_blockIndex].Commands[_commandIndex].Content;

                int currentBlockIndex = _blockIndex;
                int currentCommandIndex = _commandIndex;

                choiceButton.onClick.AddListener(
                    () => HandleChoiceClick(currentBlockIndex, currentCommandIndex)
                );

                _commandIndex += 1;
            }

            _disableClick = true;
        }

        if (curentCommand.Type.Equals("dialogue"))
        {
            curentCommand.Content = ReplaceVariable(curentCommand.Content);
            //_dialogueText.text = curentCommand.Content;
            StartCoroutine(TypeWriterEffect(curentCommand.Content, _dialogueText));
            _characterNameText.text = curentCommand.Character;
        }
    }

    IEnumerator TypeWriterEffect(string content, TMP_Text parent)
    {
        _typeWriterActive = true;

        foreach (char letter in content)
        {
            parent.text += letter;
            yield return new WaitForSeconds(_typeWriterDelay);
        }
    }

    private void HandleChoiceClick(int blockIndex, int commandIndex)
    {
        string blockKey = _processedBlocks[blockIndex].Commands[commandIndex].ChoiceBlock;
        ProcessedBlock destinationBlock = _blockLinks[blockKey];

        _blockIndex = destinationBlock.Index;
        _commandIndex = 0;

        _disableClick = false;

        ExecuteCommands();
    }

    private string ReplaceVariable(string contentLine)
    {
        if (contentLine.Contains("$"))
        {
            string phaseOne = contentLine.Remove(0, contentLine.IndexOf("$") + 1);
            string key = phaseOne.Substring(0, phaseOne.IndexOf("}"));

            string newContent = contentLine.Replace("{$" + key + "}", _variables[key]);

            return ReplaceVariable(newContent);
        }
        else
        {
            return contentLine;
        }
    }
}
