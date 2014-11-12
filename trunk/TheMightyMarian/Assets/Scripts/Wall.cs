using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : MonoBehaviour {

    private IntVector2 coordinates;
    //public List<Material> MaterialsList;
    private int MyRotation;

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
            MyRotation = r;
            this.transform.localRotation = rotations[r];
        }
    }

    public void adjustPosition()
    {
        //MyRotation = r;
        Vector3 adjust;
        switch (MyRotation)
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

    /*
    public void setRightMaterial()
    {
        int materialNo = 0;
        switch (MyRotation)
        {
            case 0: //a
                materialNo = coordinates.x % 6;
                break;
            case 1: //b
                materialNo = coordinates.y % 6;
                break;
            case 2: //c
                materialNo = coordinates.x % 6;
                break;
            case 3: //d
                materialNo = coordinates.y % 6;
                break;
            default:
                materialNo = 0;
                break; 
        }
        /*
        foreach (Transform child in transform)
        {
            if (child.tag == "block")
            {
                child.renderer.material = MaterialsList[materialNo];
            }
        }
        
            
    }*/

}
