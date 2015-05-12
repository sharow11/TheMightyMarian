using UnityEngine;
using System.Collections;

public class GroundQuad : MonoBehaviour {
    public Material ground;
    public Material water;

    public void beWater()
    {
        GetComponent<Renderer>().material = water;
        transform.position = new Vector3(0, 0, 0.1f);
    }

    public void beFloor()
    {
        GetComponent<Renderer>().material = ground;
        transform.position = new Vector3(0, 0, 0);
    }
}
