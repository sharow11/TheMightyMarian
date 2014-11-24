using UnityEngine;
using System.Collections;

public class VoidMapStripe : MapCell{

    int width = 1;
    public IntVector2 coordinatesLeft;
    public IntVector2 coordinatesRight;
    public int Width
    { get { return width; }
      set { this.width = value; }
    }

    private float x = 0.0f;
    public float X
    { get { return x; }}
    private float y = 0.0f;
    public float Y
    { get { return y; } }
    public void setCoordinates(IntVector2 left, IntVector2 right)
    {
        coordinatesLeft = left;
        coordinatesRight = right;
        this.coordinates = new IntVector2((coordinatesLeft.x + coordinatesRight.x)/2,(coordinatesLeft.y+coordinatesRight.y)/2);
        this.x = ((float)coordinatesLeft.x + (float)coordinatesRight.x) / 2.0f;
        this.y = ((float)coordinatesLeft.y + (float)coordinatesRight.y) / 2.0f;

        GameObject myQuad = transform.FindChild("Quad").gameObject;
        if (!myQuad)
        {
            Debug.Log("Stripe with no quad, hlep me");
            return;
        }
        myQuad.transform.localScale = new Vector3(1, width, 1);
        myQuad.renderer.material.mainTextureScale = new Vector2(1, width);
    }

}
