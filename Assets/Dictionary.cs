using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Meaning
{
    public string partOfSpeech;
    public Definition[] definitions;
}

[System.Serializable]
public class Definition
{
    public string definition;
    public string example;
    public string[] synonyms;
    public string[] antonyms;
}

[System.Serializable]
public class WordData
{
    public string word;
    public string phonetic;
    public Meaning[] meanings;
}

[System.Serializable]
public class WordDataList
{
    public WordData[] meanings;
}

public class Dictionary : Singleton<Dictionary>
{
    [SerializeField] private TextMeshProUGUI _wordToCheck, _definition;

    private HashSet<string> _words = new();
    private TextAsset _dictText;

    public void GetWordDefinition()
    {
        StartCoroutine(GetDefinition(_wordToCheck.text));
    }

    private IEnumerator GetDefinition(string word)
    {
        var request = UnityWebRequest.Get("https://api.dictionaryapi.dev/api/v2/entries/en/" + word);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var wordDataList = JsonUtility.FromJson<WordDataList>("{\"meanings\":" + request.downloadHandler.text + "}");

            if (wordDataList.meanings.Length > 0 && wordDataList.meanings[0].meanings.Length > 0)
            {
                var definition = wordDataList.meanings[0].meanings[0].definitions[0].definition;
                _definition.text = $"Definition: {definition}";
            }
            else
            {
                Debug.LogWarning("No definition found.");
            }
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
    }

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

        return score * word.Length;
    }


}

