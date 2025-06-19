using UnityEngine;

public class FindST : MonoBehaviour
{
    public static FindST Instance { get; private set; }
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

    #region Filter
    public int filter { get; private set; } = 0;

    public void UpadteFilter(int a)
    {
        filter = (filter == a) ? 0 : a;
    }
    public string GetFilterString()
    {
        string filter_str = "필터 : ";
        string add_str = "없음";

        if (filter == 1) add_str = "난이도 하";
        else if (filter == 2) add_str = "난이도 중하";
        else if (filter == 3) add_str = "난이도 중";
        else if (filter == 4) add_str = "난이도 중상";
        else if (filter == 5) add_str = "난이도 상";

        filter_str += add_str;

        return filter_str;
    }
    #endregion
}
