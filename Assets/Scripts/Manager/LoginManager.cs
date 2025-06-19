using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable]
public class SentenceData
{
    public int id;
    public string name;
    public string author;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string nickname;
    public string username;
    public string email;
}

[System.Serializable]
public class SentenceDataList
{
    public SentenceData[] items;
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }

    [Header("UI")]
    public InputField codeInput;   // 사용자가 로그인 코드를 입력할 InputField

    public string loginCode = "";
    public string user_name = "";
    public int cur_words_id;
    public string cur_words_name;
    public string cur_words_auther;


    bool name_request_sent;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void OpenLoginPage()
    {
        Application.OpenURL("https://danso-api.thnos.app/login/oauth");
    }

    public void OnSubmitLoginCode()
    {
        loginCode = codeInput.text.Trim();

        if (string.IsNullOrEmpty(loginCode))
        {
            print("로그인 코드가 비어 있습니다.");
            return;
        }
    }

    public void QuickCodeLogin(string code)
    {
        loginCode = code;
        if (string.IsNullOrEmpty(loginCode))
        {
            print("로그인 코드가 비어 있습니다.");
            return;
        }
    }

    public void RequestUserInfo()
    {
        if (string.IsNullOrEmpty(loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(GetUserInfo());
    }

    public void RequestRandomSentence()
    {
        if (user_name == "")
        {
            print("유저네임 없음");
            return;
        }
        if (string.IsNullOrEmpty(loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(GetRandomSentence());
    }

    IEnumerator GetRandomSentence()
    {
        string url = "https://danso-api.thnos.app/sentences/random";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Login-Code", loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("에러: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            string wrappedJson = "{\"items\":" + json + "}"; // 배열 감싸기

            try
            {
                SentenceDataList data = JsonUtility.FromJson<SentenceDataList>(wrappedJson);

                if (data.items.Length > 0)
                {
                    cur_words_id = data.items[0].id;

                    GameStreamST.Instance.SetCurrentScene("solo");

                    Debug.Log("랜덤 문장 ID: " + data.items[0].id);
                }
                else
                {
                    print("응답 배열이 비어 있음.");
                }
            }
            catch (System.Exception e)
            {
                print("파싱 실패: " + e.Message);
            }

            Debug.Log("받은 원본 JSON: " + json);
        }
    }


    IEnumerator GetUserInfo()
    {
        string url = "https://danso-api.thnos.app/user/me";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Login-Code", loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("요청 실패: " + request.error);
            name_request_sent = false;
        }
        else
        {
            string result = request.downloadHandler.text;

            try
            {
                UserData data = JsonUtility.FromJson<UserData>(result);

                if (data != null)
                {
                    user_name = data.nickname;
                }
                else
                {
                    user_name = "데이터를 불러오지 못함";
                    print("파싱 실패 데이터 널");
                }
            }
            catch (System.Exception e)
            {
                print("파싱 실패: " + e.Message);
            }

            print("요청 성공: " + result);
            Debug.Log("User Info: " + result);

            name_request_sent = false;
        }
    }
    public void RequestFindStages()
    {
        if (string.IsNullOrEmpty(loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(GetFoundStages());
    }
    IEnumerator GetFoundStages()
    {
        string url = "https://danso-api.thnos.app/user/me";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Login-Code", loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("요청 실패: " + request.error);
        }
        else
        {
            string result = request.downloadHandler.text;
            print("요청 성공: " + result);
            Debug.Log("User Info: " + result);
        }
    }
    void Update()
    {
        if (user_name == "" && loginCode != "" && !name_request_sent)
        {
            name_request_sent = true;
            RequestUserInfo();
        }
    }
}
