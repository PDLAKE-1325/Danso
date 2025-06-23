using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TypeManager : MonoBehaviour
{
    public static TypeManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    [SerializeField] private Text displayText; // UI Text 컴포넌트
    [SerializeField] TextGenerater textGenerater;
    [SerializeField] GameObject follow_text;
    [SerializeField] Transform follow_text_parent;
    [SerializeField] InputField inputField;
    public string inputText = "";
    private string composition = "";
    public float cur_y;
    public bool gameStarted = false;

    int cur_pos;

    public int score;
    public int end_score;
    public int incorrect;
    bool end;

    void CheckIncorrect(string str)
    {
        string ans = textGenerater.texts[cur_pos];
        int error = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != ans[i])
                error++;
        }
        incorrect += error;
    }
    void EndLine()
    {
        Instantiate(follow_text, follow_text_parent).TryGetComponent(out Text text);
        CheckIncorrect(inputText);
        text.text = inputText;
        inputText = "";
        displayText.text = inputText;
        cur_pos++;
    }

    [SerializeField] Text scoreText;

    void SetInfos()
    {
        if (end) return;
        float time = InGameManager.Instance.GameTime;
        score = (int)MathF.Max(0, 10000 - (10 * time) - (500 * incorrect));
        scoreText.text = $"{score}점";
    }
    void GameEnd()
    {
        if (incorrect >= 5)
            end_score = 0;
        else
            end_score = score;
        GameStreamST.Instance.SetCurrentScene("home");
        SendScore();
    }
    void SendScore()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(_SendScore());
    }
    IEnumerator _SendScore()
    {
        string url = $"https://danso-api.thnos.app/sentences/{LoginManager.Instance.cur_words_id}/set-score";

        WWWForm form = new WWWForm();
        form.AddField("score", end_score);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("X-Login-Code", LoginManager.Instance.loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("에러: " + request.error);
        }
        else
        {
            Debug.Log("점수 보냄:\n" + end_score);
        }

        SceneManager.LoadScene("Main");
    }

    void Update()
    {
        SetInfos();

        if (!gameStarted) return;

        if ((incorrect >= 5 || cur_pos >= textGenerater.texts.Count) && !end)
        {
            end = true;
            GameEnd();
            return;
        }

        string prevText = inputText;
        inputText = inputField.text;

        if (inputText.Length < prevText.Length)
        {
            inputText = prevText;
            inputField.text = prevText;
        }

        composition = Input.compositionString;

        if (inputText.Length == textGenerater.texts[cur_pos].Length)
        {
            composition = "";
        }

        if (!string.IsNullOrEmpty(composition))
        {
            displayText.text = inputText + composition;
        }
        else
        {
            displayText.text = inputText;
        }

        if (!inputField.isFocused)
            inputField.ActivateInputField();

        cur_y = 250 * cur_pos;

        if (cur_pos >= textGenerater.texts.Count || end) return;

        int answerLength = textGenerater.texts[cur_pos].Length;
        if (inputText.Length > answerLength)
        {
            inputText = inputText.Substring(0, answerLength);
            inputField.text = inputText;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputText.Length == answerLength)
            {
                EndLine();
                inputField.text = "";
                inputText = "";
            }
        }
    }

}



