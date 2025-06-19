using UnityEngine;
using UnityEngine.UI;

public class FilterText : MonoBehaviour
{
    Text my_text;
    public void UpdateFilter(int a)
    {
        FindST.Instance.UpadteFilter(a);
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
