using UnityEngine;
using UnityEngine.UI;

public class FoundUnits : MonoBehaviour
{
    public Text title;
    public Text diff;
    public int id;
    public void SetStage()
    {
        GameObject.FindWithTag("master").GetComponent<MainData>().SetStage(id);
    }
}
