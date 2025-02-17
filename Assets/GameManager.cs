using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI _playerScore, _opponentScore, _lastWord, _turn;
    [SerializeField] private TMP_InputField _wordInput;

    private HashSet<string> _usedWords = new();

    private void Start()
    {
        Dictionary.Instance.LoadDictionary();
        StartGame();
    }

    public void StartGame()
    {
        Reset();
    }

    private void Reset()
    {
        _playerScore.text = "0";
        _opponentScore.text = "0";
        _turn.text = "your turn";
        _usedWords.Clear();
        _lastWord.text = Dictionary.Instance.GetRandomWord();
    }

    public void SubmitWord()
    {
        var inputWord = _wordInput.text.ToLower();
        var lastWord = _lastWord.text;

        if (!Dictionary.Instance.CheckWord(inputWord))
        {
            Debug.Log("Not a valid word in dictionary.");
            _wordInput.text = "";
            return;
        }

        if (!MatchesLastPart(inputWord, lastWord))
        {
            Debug.Log($"Word must start with the last part of '{lastWord}'.");
            _wordInput.text = "";
            return;
        }

        if (_usedWords.Contains(inputWord))
        {
            Debug.Log($"The word '{inputWord}' has already been used.");
            _wordInput.text = "";
            return;
        }

        _usedWords.Add(inputWord);
        _lastWord.text = inputWord;
        _wordInput.text = "";
        _playerScore.text = (int.Parse(_playerScore.text) + Dictionary.Instance.CalculateScore(inputWord)).ToString();

        ComputerTurn();
    }

    private bool MatchesLastPart(string input, string lastWord)
    {
        var maxCheckLength = Mathf.Min(3, lastWord.Length);
        for (var i = maxCheckLength; i > 0; i--)
        {
            var lastPart = lastWord[^i..];
            if (input.StartsWith(lastPart)) return true;
        }
        return false;
    }

    public void ComputerTurn()
    {
        _turn.text = "opponent turn";
        var lastWord = _lastWord.text;

        string computerWord = null;
        var maxAttempts = lastWord.Length;

        for (var i = 0; i < maxAttempts; i++)
        {
            var prefix = GetRandomPrefix(lastWord);
            computerWord = Dictionary.Instance.GetRandomWordWithPrefix(prefix);

            if (computerWord != null && !_usedWords.Contains(computerWord))
            {
                _lastWord.text = computerWord;
                _usedWords.Add(computerWord);
                _opponentScore.text = (int.Parse(_opponentScore.text) + Dictionary.Instance.CalculateScore(computerWord)).ToString();
                _turn.text = "your turn";
                return;
            }
        }

        Debug.LogWarning("No valid word found for the opponent after multiple attempts.");
    }

    private string GetRandomPrefix(string word)
    {
        if (word.Length == 1) return word;

        var randomLength = Random.Range(1, word.Length);
        return word[^randomLength..];
    }
}
