using UnityEngine;

public class FollowTexts : MonoBehaviour
{
    [SerializeField] Transform main_texts_transform;
    void Update()
    {
        transform.localPosition = main_texts_transform.localPosition + Vector3.down * 100;
    }
}
