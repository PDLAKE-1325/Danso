using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class WordsData
{
    public int id;
    public string name;
    public string author;
    public string origin_author;
}

[System.Serializable]
public class FoundWordsList
{
    public WordsData[] items;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string player;
    public int score;
}

[System.Serializable]
public class RankedUser
{
    public string player;
    public int score;
    public int rank;
}

[System.Serializable]
public class SoloSentenceData
{
    public int id;
    public string name;
    public string author;
    public string original_author;
    public LeaderboardEntry[] leaderboard;
    public int my_score;
    public int my_rank;
    public RankedUser my_nearest_rank_user_1;
    public RankedUser my_nearest_rank_user_2;
}
public class MainData : MonoBehaviour
{
    public GameObject[] allUIs;
    public GameObject HomeUIs;
    public GameObject SoloUIs;
    public GameObject FindUIs;


    [Header("Solo")]
    [SerializeField] Text title_;
    [SerializeField] Text origin_author;
    [SerializeField] Transform rankingListParent;
    [SerializeField] GameObject rank_prefab;
    [SerializeField] GameObject rank_dot;

    [Header("Find")]
    [SerializeField] Text finder_input;
    [SerializeField] Text found_title;
    [SerializeField] Text found_author;
    [SerializeField] Text found_origin_author;
    [SerializeField] Transform found_words_parent;
    [SerializeField] GameObject found_words_prefab;
    [SerializeField] GameObject rightBox_obj;


    public void SetSoloInfos()
    {
        RequestSoloInfo();
    }
    public void RealStart()
    {
        GameStreamST.Instance.GameStart();
    }
    void RequestSoloInfo()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(GetSoloInfo());
    }
    IEnumerator GetSoloInfo()
    {
        string url = $"https://danso-api.thnos.app/sentences/{LoginManager.Instance.cur_words_id}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Login-Code", LoginManager.Instance.loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("요청 실패: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("응답: " + json);

            try
            {
                SoloSentenceData data = JsonUtility.FromJson<SoloSentenceData>(json);

                title_.text = data.name;
                origin_author.text = data.original_author;

                foreach (Transform item in rankingListParent)
                {
                    Destroy(item.gameObject);
                }

                for (int i = 0; i < data.leaderboard.Length; i++)
                {
                    CreateRankBlock(i + 1, data.leaderboard[i].player, data.leaderboard[i].score);
                }
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(rank_dot, rankingListParent);
                }

                if (data.my_nearest_rank_user_1.player != "없음")
                {
                    print("내위 있음");
                    CreateRankBlock(data.my_nearest_rank_user_1.rank, data.my_nearest_rank_user_1.player, data.my_nearest_rank_user_1.score);
                }

                CreateRankBlock(data.my_rank, LoginManager.Instance.user_name, data.my_score);

                if (data.my_nearest_rank_user_2.player != "없음")
                {
                    print("내아래 있음");
                    CreateRankBlock(data.my_nearest_rank_user_2.rank, data.my_nearest_rank_user_2.player, data.my_nearest_rank_user_2.score);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("파싱 오류: " + e.Message);
            }
        }
    }
    [SerializeField] Color myRankBlockColor = new();
    void CreateRankBlock(int _rank, string _name, int _score)
    {
        GameObject block = Instantiate(rank_prefab, rankingListParent);
        block.TryGetComponent(out RankingPrefab texts);
        if (_name == LoginManager.Instance.user_name)
        {
            block.TryGetComponent(out Image image);
            image.color = myRankBlockColor;
        }
        string rank;
        if (_rank == 1) rank = "1st.";
        else if (_rank == 2) rank = "2nd.";
        else if (_rank == 3) rank = "3rd.";
        else rank = $"{_rank}th.";
        texts.rank.text = rank;
        texts.user.text = _name;
        texts.score.text = $"{_score}점";
    }

    void ClearFoundTransform()
    {
        foreach (Transform item in found_words_parent)
        {
            Destroy(item.gameObject);
        }
    }

    string[] filter_s = { "", "a", "b", "c", "d", "e" };

    public void FindWordsByTitle()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        ClearFoundTransform();
    }

    public void FindWordsByAuthor()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        ClearFoundTransform();
    }

    public void FindByFilter()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        ClearFoundTransform();
    }

    IEnumerator GetWordsByTitle()
    {
        string url = $"https://danso-api.thnos.app/sentences/search?keyword={finder_input.text}";

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
            string wrappedJson = "{\"items\":" + json + "}"; // 배열 감싸기

            try
            {
                FoundWordsList data = JsonUtility.FromJson<FoundWordsList>(wrappedJson);

                if (data.items.Length > 0)
                {
                    Debug.Log("첫 문장 ID: " + data.items[0].id);
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


    #region Extra
    public void HomeBack()
    {
        GameStreamST.Instance.SetCurrentScene("home");
    }
    #endregion
}
