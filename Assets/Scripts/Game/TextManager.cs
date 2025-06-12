using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public List<string> texts = new();
    [SerializeField] GameObject curTextPrefab;
    [SerializeField] GameObject mainTextPrefab;

    void Start()
    {
        PlayerPrefs.SetString("STRING_SET", "마른 하늘에 날벼락이|내리쳤다.|유감");
        SplitTexts();
    }
    void SplitTexts()
    {
        string cur_string_set = PlayerPrefs.GetString("STRING_SET");
        int len = cur_string_set.Length;
        string line = "";
        for (int i = 0; i < len + 1; i++)
        {
            if (i == len) texts.Add(line);
            else if (cur_string_set[i] != '|') line += cur_string_set[i];
            else { texts.Add(line); line = ""; }
        }
    }
}
