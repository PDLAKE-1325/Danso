using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] int hover_text_size;
    [SerializeField] ColorChanger colorChanger;
    Image cur_img;
    Color prev_color;
    Text cur_text;
    int prev_text_size;
    bool hovering;
    public void MainButtonMouseEnter(Image image)
    {
        cur_img = image;
        prev_color = image.color;
        cur_text = image.transform.GetChild(0).GetComponent<Text>();
        prev_text_size = cur_text.fontSize;
        cur_text.fontSize = hover_text_size;
        hovering = true;
    }
    void Update()
    {
        if (hovering && cur_img != null) cur_img.color = colorChanger.color;
    }
    public void MainButtonMouseExit(Image image)
    {
        hovering = false;
        if (prev_color != null) image.color = prev_color;
        if (prev_text_size != 0) cur_text.fontSize = prev_text_size;
    }

    public void GameStartSolo()
    {
        LoginManager.Instance.RequestRandomSentence();
    }
    public void FindStage()
    {
        GameStreamST.Instance.SetCurrentScene("find");
    }
    public void AddWordSet()
    {
        Application.OpenURL($"https://danso-api.thnos.app/dashboard/?login_code={LoginManager.Instance.loginCode}");
    }
    public void ChangeName()
    {
        Application.OpenURL($"https://danso-api.thnos.app/dashboard/profile/edit?login={LoginManager.Instance.loginCode}");
    }
}
