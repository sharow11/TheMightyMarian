using UnityEngine;
using System.Collections;

public class MarianAnimator : MonoBehaviour {
    MoveMarian.State state = MoveMarian.State.Idle;

    //WALK
    public int walkColCount;
    public int walkRowCount;
    public Material walkMaterial;
    public int walkTotalCells;
    public int walkFps;

    //IDLE
    public int idleColCount;
    public int idleRowCount;
    public Material idleMaterial;
    public int idleTotalCells;
    public int idleFps;

    public int rowNumber = 0;
    public int colNumber = 0;

    private Vector2 offset;

    void FixedUpdate() {
        updateState();
        switch (state)
        {
            case MoveMarian.State.Idle:
                IdleSpriteAnimation();
                break;
            case MoveMarian.State.Moving:
                WalkSpriteAnimation();
                break;
            default:
                break;
        }
    }

    void updateState()
    {
        MoveMarian.State currentMarianState = this.transform.parent.GetComponent<MoveMarian>().state;
        if (state != currentMarianState)
        {
            state = currentMarianState;
            rowNumber = 0;
            colNumber = 0;
            setMaterial();
        }
    }

    void setMaterial()
    {
        switch (state)
        {
            case MoveMarian.State.Idle:
                this.renderer.material = idleMaterial;
                break;
            case MoveMarian.State.Moving:
                this.renderer.material = walkMaterial;
                break;
            default:
                break;
        }
    }

    void IdleSpriteAnimation()
    {
        int index = (int)(Time.time * idleFps);
        index = index % idleTotalCells;
        float sizeX = 1.0f / idleColCount;
        float sizeY = 1.0f / idleRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        float uIndex = index % idleColCount;
        float vIndex = index / idleColCount;
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);
        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

    void WalkSpriteAnimation()
    {
        int index = (int)(Time.time * walkFps);
        index = index % walkTotalCells;
        float sizeX = 1.0f / walkColCount;
        float sizeY = 1.0f / walkRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        float uIndex = index % walkColCount;
        float vIndex = index / walkColCount;
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);
        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

}
