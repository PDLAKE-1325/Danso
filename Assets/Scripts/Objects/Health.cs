using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int show_hp;
    void Update()
    {
        gameObject.SetActive(5 - TypeManager.Instance.incorrect >= show_hp);
    }
}
