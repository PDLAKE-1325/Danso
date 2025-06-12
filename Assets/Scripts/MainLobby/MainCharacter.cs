using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 startPoint;
    float wavePosition;
    void Start()
    {
        startPoint = transform.position;
    }
    public void MouseEnter()
    {
        transform.position = new Vector2(startPoint.x, startPoint.y + MathF.Sin(wavePosition));
    }
    public void MouseExit()
    {

    }
    void WaveMove()
    {
        wavePosition += Time.deltaTime * speed;
        wavePosition %= 2 * Mathf.PI;
    }
}
