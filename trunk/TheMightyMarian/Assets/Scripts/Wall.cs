using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    private IntVector2 coordinates;
    //public int r;

    private Quaternion[] oldrotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

    private Quaternion[] rotations = {
		Quaternion.Euler(90f,180f,0f), //a ok
		Quaternion.Euler(0f, 270f, 90f), //b ok
		Quaternion.Euler(270f, 0f, 0f), //c ok
		Quaternion.Euler(0f, 90f, 270f) //d ok
	};
	
    public IntVector2 Coordinates
    {
        get { return coordinates; }
        set { coordinates = value; }
    }

    public void setRotation(int r)
    {
        if (r < 4)
        {
            this.transform.localRotation = rotations[r];
        }
    }

    public void adjustPosition(int r)
    {
        Vector3 adjust;
        switch (r)
        {
            case 0: //a
                adjust = new Vector3(0f, -0.5f, 0f);
                break; 
            case 1: //b
                adjust = new Vector3(-0.5f, 0f, 0f);
                break; 
            case 2: //c
                adjust = new Vector3(0f, 0.5f, 0f);
                break; 
            case 3: //d
                adjust = new Vector3(0.5f, 0f, 0f);
                break; 
            default:
                adjust = new Vector3(0f, 0f, 0f);
                break; 
        }
        this.transform.localPosition += adjust;
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
