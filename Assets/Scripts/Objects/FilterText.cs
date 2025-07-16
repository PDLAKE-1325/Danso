using UnityEngine;
using UnityEngine.UI;

public class FilterText : MonoBehaviour
{
    Text my_text;
    [SerializeField] MainData mainData;
    public void UpdateFilter(int a)
    {
        FindST.Instance.UpadteFilter(a);
        mainData.SetWordSets();
    }
    void Start()
    {
        my_text = GetComponent<Text>();
    }
    void Update()
    {
        if (GameStreamST.Instance.cur_scene == "find")
        {
            my_text.text = FindST.Instance.GetFilterString();
        }
    }
}
