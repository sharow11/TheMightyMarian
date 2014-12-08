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
    private int rowNumber = 0; 
    private int colNumber = 0; 

    private Vector2 offset;

    void FixedUpdate() { 
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

    void IdleSpriteAnimation()
    {
        int index = (int)(Time.time * idleFps);
        index = index % idleTotalCells;
        float sizeX = 1.0f / idleColCount;
        float sizeY = 1.0f / idleRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        float uIndex = index % idleColCount; //horizontal indeks
        float vIndex = index / idleColCount; //vertival indeks
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
        float uIndex = index % walkColCount;  //horizontal indeks
        float vIndex = index / walkColCount; //vertival indeks
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2(offsetX, offsetY);
        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", size);
    }

    void AttackSpriteAnimation()
    {
        int index = (int)(Time.time * attackFps);
        index = index % attackTotalCells;
        float sizeX = 1.0f / attackColCount;
        float sizeY = 1.0f / attackRowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        float uIndex = index % attackColCount;
        float vIndex = index / attackColCount;
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
