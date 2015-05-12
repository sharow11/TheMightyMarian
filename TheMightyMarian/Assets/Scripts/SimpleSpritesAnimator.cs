using UnityEngine;
using System.Collections;

public class SimpleSpritesAnimator : MonoBehaviour
{
    public int colCount = 5;
    public int rowCount = 2;
    //ilosc obrazkow/sprajtsheet
    private int rowNumber = 0;
    private int colNumber = 0;
    //to sie zmienia z animacja
    public int totalCells = 10;
    //niby moznaby to obliczac, ale jakby to obliczac co klatke..
    public int fps = 2;

    private Vector2 offset;

    void FixedUpdate() {
        int index = (int)(Time.time * fps);
        index = index % totalCells;
        float sizeX = 1.0f / colCount;
        float sizeY = 1.0f / rowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        float uIndex = index % colCount;
        float vIndex = index / colCount;
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);
    }
}