﻿using UnityEngine;
using System.Collections;

public class ComplicatedSpritesAnimator : MonoBehaviour {

    //IDLE
    public int idleColCount;
    public int idleRowCount;
    public Material idleMaterial;
    public int idleTotalCells;

    //ATTACK
    public int attackColCount;
    public int attackRowCount;
    public Material attackMaterial;
    public int attackTotalCells;

    //WALK
    public int walkColCount;
    public int walkRowCount;
    public Material walkMaterial;
    public int walkTotalCells;


    public int rowNumber = 0; //Zero Indexed
    public int colNumber = 0; //Zero Indexed

    public int fps = 2;
    //Maybe this should be a private var
    private Vector2 offset;
    //Update
    void FixedUpdate() { 
        //SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps); 
    }

    //SetSpriteAnimation
    void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps)
    {

        // Calculate index
        int index = (int)(Time.time * fps);
        // Repeat when exhausting all cells
        index = index % totalCells;

        // Size of every cell
        float sizeX = 1.0f / colCount;
        float sizeY = 1.0f / rowCount;
        Vector2 size = new Vector2(sizeX, sizeY);

        // split into horizontal and vertical index
        var uIndex = index % colCount;
        var vIndex = index / colCount;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);

        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

    void setMaterial()
    { 
        
    }

    void updateState()
    { 
        
    }

}