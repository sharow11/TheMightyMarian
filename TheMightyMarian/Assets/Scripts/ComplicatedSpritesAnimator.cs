using UnityEngine;
using System.Collections;

public class ComplicatedSpritesAnimator : MonoBehaviour {

    //IDLE
    public int idleColCount;
    public int idleRowCount;
    public Material idleMaterial;
    public int idleTotalCells;
    public int idleFps;

    //ATTACK
    public int attackColCount;
    public int attackRowCount;
    public Material attackMaterial;
    public int attackTotalCells;
    public int attackFps;

    //WALK
    public int walkColCount;
    public int walkRowCount;
    public Material walkMaterial;
    public int walkTotalCells;
    public int walkFps;

    enum AnimState : byte { idle, attack, walk };
    private AnimState currentState = AnimState.idle;
    public int rowNumber = 0; //Zero Indexed
    public int colNumber = 0; //Zero Indexed

    public int fps = 2;
    //Maybe this should be a private var
    private Vector2 offset;
    //Update
    void FixedUpdate() { 
        //SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps); 
        updateState();
        switch (currentState)
        {
            case AnimState.idle:
                IdleSpriteAnimation();
                break;
            case AnimState.attack:
                AttackSpriteAnimation();
                break;
            case AnimState.walk:
                WalkSpriteAnimation();
                break;
            default:
                IdleSpriteAnimation();
                break;
        }
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

    void IdleSpriteAnimation()
    {
        // Calculate index
        int index = (int)(Time.time * idleFps);
        // Repeat when exhausting all cells
        index = index % idleTotalCells;

        // Size of every cell
        float sizeX = 1.0f / idleColCount;
        float sizeY = 1.0f / idleRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);

        // split into horizontal and vertical index
        var uIndex = index % idleColCount;
        var vIndex = index / idleColCount;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);

        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

    void WalkSpriteAnimation()
    {
        // Calculate index
        int index = (int)(Time.time * walkFps);
        // Repeat when exhausting all cells
        index = index % walkTotalCells;

        // Size of every cell
        float sizeX = 1.0f / walkColCount;
        float sizeY = 1.0f / walkRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);

        // split into horizontal and vertical index
        var uIndex = index % walkColCount;
        var vIndex = index / walkColCount;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);

        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

    void AttackSpriteAnimation()
    {
        // Calculate index
        int index = (int)(Time.time * attackFps);
        // Repeat when exhausting all cells
        index = index % attackTotalCells;

        // Size of every cell
        float sizeX = 1.0f / attackColCount;
        float sizeY = 1.0f / attackRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);

        // split into horizontal and vertical index
        var uIndex = index % attackColCount;
        var vIndex = index / attackColCount;

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
        switch (currentState)
        {
            case AnimState.idle:
                this.renderer.material = idleMaterial;
                break;
            case AnimState.attack:
                this.renderer.material = attackMaterial;
                break;
            case AnimState.walk:
                this.renderer.material = walkMaterial;
                break;
            default:
                break;
        }
    }

    void updateState()
    {
        AnimState currentEnemyState = enemyStateToAnimState(this.transform.parent.GetComponent<Enemy>().state);
        if (currentEnemyState != currentState)
        {
            currentState = currentEnemyState;
            rowNumber = 0;
            colNumber = 0;
            setMaterial();
        }
        
    }

    AnimState enemyStateToAnimState(Enemy.State es)
    {
        if(es == Enemy.State.idle)
        { return AnimState.idle;}
        else if(es == Enemy.State.attacking)
        { return AnimState.attack;}
        else
        { return AnimState.walk; }

    }

}
