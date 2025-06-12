using UnityEngine;
using UnityEngine.UI;

public class FolllowParentText : MonoBehaviour
{
    public Text target_text;
    void Start()
    {
        GetComponent<Text>().text = target_text.text;
    }
    void FixedUpdate()
    {
        transform.position = target_text.transform.position;
    }
}
