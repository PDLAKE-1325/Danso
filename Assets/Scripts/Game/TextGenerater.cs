using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class SentenceWords
{
    public int id;
    public string name;
    public string author;
    public string[] sentences;
}

public class TextGenerater : MonoBehaviour
{
    public List<string> texts = new();
    [SerializeField] GameObject mainTextPrefab;
    [SerializeField] Transform text_parent;
    [SerializeField] float text_move_speed;

    void Start()
    {
        GetTexts();
    }
    void Update()
    {
        SetTextsPosition();
    }
    void SetTextsPosition()
    {
        Vector2 target_vector = new(0, TypeManager.Instance.cur_y);
        text_parent.localPosition = Vector2.Lerp(text_parent.localPosition, target_vector, Time.deltaTime * text_move_speed);
    }
    void GetTexts()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(GetWords());
    }
    IEnumerator GetWords()
    {
        string url = $"https://danso-api.thnos.app/sentences/{LoginManager.Instance.cur_words_id}/game";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Login-Code", LoginManager.Instance.loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("에러: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;

            // JSON 파싱
            SentenceWords data = JsonUtility.FromJson<SentenceWords>(json);

            if (data != null)
            {
                texts = data.sentences.ToList();
                CreateTexts();
                TypeManager.Instance.gameStarted = true;
            }
            else
            {
                print("파싱 실패");
            }
            Debug.Log("랜덤 문장 데이터:\n" + json);
        }
    }

    void CreateTexts()
    {
        foreach (string item in texts)
        {
            Text cur_text;

            Instantiate(mainTextPrefab, text_parent).TryGetComponent(out cur_text);
            cur_text.text = item;
        }
    }
}
