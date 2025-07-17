using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class WordsData
{
    public int id;
    public string name;
    public string author;
    public string origin_author;
    public string level;
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
    public int total_likes;
    public string is_liked;
    public string image_url;
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
    [SerializeField] Transform found_words_parent;
    [SerializeField] GameObject found_words_prefab;
    [SerializeField] FoundWordsInfo foundWordsInfo_script;


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
                    CreateRankBlock(i + 1, data.leaderboard[i].player, data.leaderboard[i].score, rankingListParent);
                }
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(rank_dot, rankingListParent);
                }

                if (data.my_nearest_rank_user_1.player != "없음")
                {
                    print("내위 있음");
                    CreateRankBlock(data.my_nearest_rank_user_1.rank, data.my_nearest_rank_user_1.player, data.my_nearest_rank_user_1.score, rankingListParent);
                }

                CreateRankBlock(data.my_rank, LoginManager.Instance.user_name, data.my_score, rankingListParent);

                if (data.my_nearest_rank_user_2.player != "없음")
                {
                    print("내아래 있음");
                    CreateRankBlock(data.my_nearest_rank_user_2.rank, data.my_nearest_rank_user_2.player, data.my_nearest_rank_user_2.score, rankingListParent);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("파싱 오류: " + e.Message);
            }
        }
    }
    [SerializeField] Color myRankBlockColor = new();
    void CreateRankBlock(int _rank, string _name, int _score, Transform parent)
    {
        GameObject block = Instantiate(rank_prefab, parent);
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

    string[] filter_s = { "", "E", "D", "C", "B", "A" };

    FoundWordsList curFounds;
    public void FindWordsByTitle()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        curFounds = new();
        ClearFoundTransform();
        StartCoroutine(GetWordsByTitleOrAuther(true));
    }

    public void FindWordsByAuthor()
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        curFounds = new();
        ClearFoundTransform();
        StartCoroutine(GetWordsByTitleOrAuther(false));
    }


    IEnumerator GetWordsByTitleOrAuther(bool isTitle)
    {
        string url = isTitle ? $"https://danso-api.thnos.app/sentences/search?keyword={finder_input.text}" : $"https://danso-api.thnos.app/sentences/search?author={finder_input.text}";

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
                    curFounds = data;
                    SetWordSets();
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
    string[] diff_s = { "", "하", "중하", "중", "중상", "상" };
    public void SetWordSets()
    {
        if (curFounds == null) return;
        if (curFounds.items.Length == 0) return;
        foreach (Transform item in found_words_parent.transform)
        {
            Destroy(item.gameObject);
        }
        int cur_filter = FindST.Instance.filter;
        if (cur_filter != 0)
        {
            foreach (WordsData item in curFounds.items)
            {
                if (item.level == filter_s[cur_filter])
                {
                    FoundUnits fu;
                    if (Instantiate(found_words_prefab, found_words_parent).TryGetComponent(out fu))
                    {
                        fu.title.text = item.name;
                        fu.diff.text = diff_s[Array.IndexOf(filter_s, item.level)];
                        fu.id = item.id;
                    }

                }
            }
        }
        else
        {
            foreach (WordsData item in curFounds.items)
            {
                FoundUnits fu;
                if (Instantiate(found_words_prefab, found_words_parent).TryGetComponent(out fu))
                {
                    fu.title.text = item.name;
                    fu.diff.text = diff_s[Array.IndexOf(filter_s, item.level)];
                    fu.id = item.id;
                }
            }
        }
    }
    int cur_found_id;
    public void SetStage(int id)
    {
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }
        StartCoroutine(SetStageInfo(id));
    }
    public void Like()
    {
        // /sentences/{id}/interact-like
        if (string.IsNullOrEmpty(LoginManager.Instance.loginCode))
        {
            print("로그인 코드가 없습니다.");
            return;
        }

        StartCoroutine(Like_());
    }
    IEnumerator Like_()
    {
        string url = $"https://danso-api.thnos.app/sentences/{cur_found_id}/interact-like";

        WWWForm form = new WWWForm();
        form.AddField("", "");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("X-Login-Code", LoginManager.Instance.loginCode);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print("에러: " + request.error);
        }
        else
        {
            Debug.Log("좋아요 함");
            SetStage(cur_found_id);
        }
    }

    IEnumerator SetStageInfo(int id)
    {
        string url = $"https://danso-api.thnos.app/sentences/{id}";
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

                // foundWordsInfo_script
                StartCoroutine(LoadImageFromUrl(data.image_url));
                cur_found_id = id;
                foundWordsInfo_script.title.text = data.name;
                foundWordsInfo_script.author.text = $"제작자 : {data.author}";
                foundWordsInfo_script.origin_author.text = $"원작자 : {data.original_author}";
                foundWordsInfo_script.heart.color = data.is_liked == "true" ? Color.red : Color.black;
                foundWordsInfo_script.liked.text = data.total_likes.ToString();

                foreach (Transform item in foundWordsInfo_script.ranking_parent)
                {
                    Destroy(item.gameObject);
                }

                for (int i = 0; i < data.leaderboard.Length; i++)
                {
                    CreateRankBlock(i + 1, data.leaderboard[i].player, data.leaderboard[i].score, foundWordsInfo_script.ranking_parent);
                }
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(rank_dot, foundWordsInfo_script.ranking_parent);
                }

                if (data.my_nearest_rank_user_1.player != "없음")
                {
                    print("내위 있음");
                    CreateRankBlock(data.my_nearest_rank_user_1.rank, data.my_nearest_rank_user_1.player, data.my_nearest_rank_user_1.score, foundWordsInfo_script.ranking_parent);
                }

                CreateRankBlock(data.my_rank, LoginManager.Instance.user_name, data.my_score, foundWordsInfo_script.ranking_parent);

                if (data.my_nearest_rank_user_2.player != "없음")
                {
                    print("내아래 있음");
                    CreateRankBlock(data.my_nearest_rank_user_2.rank, data.my_nearest_rank_user_2.player, data.my_nearest_rank_user_2.score, foundWordsInfo_script.ranking_parent);
                }
                foundWordsInfo_script.transform.gameObject.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError("파싱 오류: " + e.Message);
            }
        }
    }
    IEnumerator LoadImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://danso-image-cdn.serltretu24.workers.dev/?url=" + url);
        request.SetRequestHeader("Accept-Encoding", "gzip");
        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("이미지 로딩 실패: " + request.error);
            foundWordsInfo_script.img.sprite = null;
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            // Texture2D -> Sprite 변환
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            foundWordsInfo_script.img.sprite = sprite;
        }
    }

    #region MYGGGG
    [SerializeField] GameObject FoundInfoRankObj;
    [SerializeField] GameObject FoundInfoObj;
    public void showFoundInfoRank()
    {
        FoundInfoRankObj.SetActive(true);
        FoundInfoObj.SetActive(false);
    }
    public void hideFoundInfoRank()
    {
        FoundInfoRankObj.SetActive(false);
        FoundInfoObj.SetActive(true);
    }

    public void startFoundWords()
    {
        LoginManager.Instance.cur_words_id = cur_found_id;
        RealStart();
    }
    #endregion

    #region Extra
    public void HomeBack()
    {
        GameStreamST.Instance.SetCurrentScene("home");
    }
    #endregion
}
