using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    private int _notesQuantity;

    private AudioSource _audioSource;
    private AudioClip _musicResource;
    private Sprite _speakerOnResource;
    private Sprite _speakerOffResource;
    private Text _typewriterTextComponent;
    private Text _notesTextComponent;
    private Text _scoreTextComponent;
    private Text _mugTextComponent;
    private Image _speakerComponent;
    private List<string> _validKeyCodes;
    private List<string> _activeNotes;
    private List<string> _allNotes;
    private int _score;
    private int _targetScore;
    private int _level;
    private float _normalVolume;
    private bool _isUsingTestNotes;

    // Use this for initialization
    IEnumerator Start () {
        _notesQuantity = 1;

        _audioSource = GetComponent<AudioSource>();
        _musicResource = Resources.Load<AudioClip>("Audio/MapleLeafRag");
        _speakerOnResource = Resources.Load<Sprite>("Icons/speaker-on");
        _speakerOffResource = Resources.Load<Sprite>("Icons/speaker-off");

        _typewriterTextComponent = transform
            .Find("TypewriterPanel")
            .Find("TypewriterText")
            .GetComponent<Text>();

        _notesTextComponent = transform
            .Find("NotesPanel")
            .Find("NotesText")
            .GetComponent<Text>();

        _scoreTextComponent = transform
            .Find("NotesPanel")
            .Find("ScoreText")
            .GetComponent<Text>();

        _mugTextComponent = transform
            .Find("MugPanel")
            .Find("MugText")
            .GetComponent<Text>();

        _speakerComponent = transform
            .Find("MugPanel")
            .Find("SpeakerPanel")
            .GetComponent<Image>();

        _validKeyCodes = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.,'"
            .ToCharArray()
            .Select(x => x.ToString())
            .ToList();

        _isUsingTestNotes = false;

        if (_isUsingTestNotes)
        {
            _allNotes = Resources.LoadAll<TextAsset>("Test Notes")
                .Select(x => x.text)
                .ToList();
        }
        else
        {
            _allNotes = Resources.Load<TextAsset>("notes")
                .text
                .Split('\n')
                .Select(x => x.Trim())
                .ToList();
        }

        Debug.Log("ALL OF THE NOTES: " + _allNotes.Count());

        AddNewActiveNode();

        _score = 0;
        _level = 1;
        UpdateMug();
        ResetTextValues();
        UpdateScore();

        yield return StartCoroutine(WaitUntilFrameLoad());
        _normalVolume = 0.7f;
        PlayMusic();
    }

    // Update is called once per frame
    void Update () {
        EnterTextOnTypewriter();

        if (Input.GetKeyUp(KeyCode.Return))
        {
            ConfirmInput();
            UpdateScore();
            CheckForNextLevel();
            ResetTextValues();
        }

    }

    private void CheckForNextLevel()
    {
        if (_score < _targetScore)
            return;

        _level++;
        UpdateMug();

        _score = 0;
        _notesQuantity++;
        _notesTextComponent.text = string.Empty;

        AddNewActiveNode();
        UpdateScore();
    }

    private void AddNewActiveNode()
    {
        if(_activeNotes == null)
            _activeNotes = new List<string>();

        var newNote = _allNotes.Where(x => !_activeNotes.Contains(x))
            .FirstOrDefault();

        if (string.IsNullOrEmpty(newNote))
        {
            Debug.Log("Exceeded limit of notes");
            return;
        }

        _activeNotes.Add(newNote);
        Debug.Log("Note added:" + newNote);

        _targetScore = _activeNotes.Count * 10;
    }

    private void UpdateScore()
    {
        _scoreTextComponent.text = "Copies made - "
            + _score +
            " / " +
            _targetScore;
    }

    private void UpdateMug()
    {
        _mugTextComponent.text = "Day " + _level;
    }

    private void ConfirmInput()
    {
        var notes = _notesTextComponent.text;
        var typewriter = _typewriterTextComponent.text;

        if (string.IsNullOrEmpty(notes))
            return;

        if (string.IsNullOrEmpty(typewriter))
            return;

        Debug.Log("Notes: " + notes);
        Debug.Log("Typewriter: " + typewriter);

        if (notes.Equals(typewriter))
            _score++;
    }

    private void ResetTextValues()
    {
        if (_activeNotes.Count() < 1)
            throw new Exception("Notes not found in Resources file");

        var newNote = _activeNotes.Contains(_notesTextComponent.text)
            ? _notesTextComponent.text
            : _activeNotes.ElementAt(0);

        if (_activeNotes.Count() > 1)
        {
            if (_activeNotes.Last().Equals(_notesTextComponent.text))
            {
                newNote = _activeNotes.First();
            }
            else
            {
                var ind = _activeNotes.IndexOf(_notesTextComponent.text);
                newNote = _activeNotes.ElementAt(ind + 1);
            }
        }

        Debug.Log("Note changed to " + newNote);
        _notesTextComponent.text = newNote;
        _typewriterTextComponent.text = string.Empty;
    }

    private void EnterTextOnTypewriter()
    {
        var key = Input.inputString;
        if (string.IsNullOrEmpty(key))
            return;

        if (_validKeyCodes.Contains(key))
        {
            Debug.Log(key);
            var input = key;
            var currentText = _typewriterTextComponent.text;
            _typewriterTextComponent.text = currentText + input;
        }
    }

    private IEnumerator WaitUntilFrameLoad()
    {
        yield return new WaitForEndOfFrame();
    }

    private void PlayMusic()
    {
        _audioSource.clip = _musicResource;
        Debug.Log("Playing " + _audioSource.clip.name);
        _audioSource.Play();
        //_audioSource.PlayOneShot(_musicResource, 0.7f);
    }

    public void ToggleMusic()
    {
        if (_audioSource.volume > 0)
        {
            _audioSource.volume = 0;
            _speakerComponent.sprite = _speakerOffResource;
        }
        else
        {
            _audioSource.volume = _normalVolume;
            _speakerComponent.sprite = _speakerOnResource;
        }
    }

    public void LeaveGame()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }
}
