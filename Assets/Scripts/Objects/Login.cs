using UnityEngine;

public class Login : MonoBehaviour
{
    void Update()
    {
        gameObject.SetActive(LoginManager.Instance.loginCode == "");
    }
}
