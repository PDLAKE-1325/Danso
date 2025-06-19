using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStreamST : MonoBehaviour
{
    public static GameStreamST Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    MainData menuDatas;

    #region Scene
    public string cur_scene { get; private set; } = "home";
    public void SetCurrentScene(string scene)
    {
        if (scene == "solo")
        {
            menuDatas.SetSoloInfos();
        }
        cur_scene = scene;
    }
    #endregion

    #region Methods

    public void GameStart()
    {
        SetCurrentScene("game");
        menuDatas = null;
        SceneManager.LoadScene("Game");
    }
    #endregion

    #region ChangeUIs

    void HomeUI()
    {
        foreach (GameObject item in menuDatas.allUIs)
        {
            if (item != menuDatas.HomeUIs) item.SetActive(false);
            else item.SetActive(true);
        }
    }
    void SoloUI()
    {
        foreach (GameObject item in menuDatas.allUIs)
        {
            if (item != menuDatas.SoloUIs) item.SetActive(false);
            else item.SetActive(true);
        }
    }
    void ChangeUIs()
    {
        try
        {
            if (cur_scene == "home") HomeUI();
            else if (cur_scene == "solo") SoloUI();
        }
        catch
        {
            print("메인화면 UI 접근실패");
        }
    }

    #endregion

    void Update()
    {
        if (cur_scene != "game" && menuDatas == null)
        {
            menuDatas = FindFirstObjectByType<MainData>();
        }
        ChangeUIs();
    }
}
