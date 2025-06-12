using UnityEngine;

public class ScrollViewContents : MonoBehaviour
{
    int child_count => transform.childCount;
    RectTransform myRectTransform;

    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        myRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, child_count * 105);
    }
}
