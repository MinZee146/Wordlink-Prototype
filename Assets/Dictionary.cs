using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Dictionary : Singleton<Dictionary>
{
    private HashSet<string> _words = new();
    private TextAsset _dictText;

    public void LoadDictionary()
    {
        _dictText = Resources.Load<TextAsset>("dictionary");

        using var reader = new StringReader(_dictText.text);
        while (reader.ReadLine() is { } line)
        {
            _words.Add(line);
        }
    }

    public bool CheckWord(string word)
    {
        if (word == null)
        {
            return false;
        }

        return _words.Contains(word);
    }

    public string GetRandomWord()
    {
        if (_words.Count == 0) return null;

        return _words.ElementAt(UnityEngine.Random.Range(0, _words.Count));
    }

    public void GetWordWithPrefix(string prefix)
    {
        var filteredWords = _words.Where(word => word.StartsWith(prefix)).ToList();

        if (filteredWords.Count == 0)
        {
            Debug.Log("No words found.");
            return;
        }

        Debug.Log(filteredWords[UnityEngine.Random.Range(0, filteredWords.Count)]);
    }

    public string GetRandomWordWithPrefix(string prefix)
    {
        var filteredWords = _words.Where(word => word.StartsWith(prefix)).ToList();

        if (filteredWords.Count == 0)
        {
            return null;
        }

        return filteredWords[UnityEngine.Random.Range(0, filteredWords.Count)];
    }

    public int CalculateScore(string word)
    {
        if (string.IsNullOrEmpty(word)) return 0;

        Dictionary<char, int> scrabbleScores = new()
        {
            {'a', 1}, {'e', 1}, {'i', 1}, {'o', 1}, {'u', 1}, {'l', 1}, {'n', 1}, {'s', 1}, {'t', 1}, {'r', 1},
            {'d', 2}, {'g', 2},
            {'b', 3}, {'c', 3}, {'m', 3}, {'p', 3},
            {'f', 4}, {'h', 4}, {'v', 4}, {'w', 4}, {'y', 4},
            {'k', 5},
            {'j', 8}, {'x', 8},
            {'q', 10}, {'z', 10}
        };

        var score = 0;
        foreach (var c in word)
        {
            if (scrabbleScores.TryGetValue(c, out var letterScore))
            {
                score += letterScore;
            }
        }

        return score*word.Length;
    }
}
