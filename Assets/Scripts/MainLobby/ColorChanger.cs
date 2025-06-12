using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    private Text image;
    [SerializeField] private float speed, alpha = 1;
    float x;
    public Color color;
    void Awake()
    {
        image = GetComponent<Text>();
    }

    void Update()
    {
        ChangeColor();
    }

    void ChangeColor()
    {
        color = new Color(Mathf.Abs(Mathf.Cos(0.5f * x)), Mathf.Max(0, Mathf.Sin(0.7f * x)), Mathf.Sin(0.7f * x - 1.1f), alpha);
        image.color = color;

        x += Time.deltaTime * speed;
        x %= 2 * Mathf.PI;
    }
}
