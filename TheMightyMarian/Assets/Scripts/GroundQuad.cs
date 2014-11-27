using UnityEngine;
using System.Collections;

public class GroundQuad : MonoBehaviour {
    public Material ground;
    public Material water;

    public void beWater()
    {
        renderer.material = water;
        transform.position = new Vector3(0, 0, 2);
    }

    public void beFloor()
    {
        renderer.material = ground;
        transform.position = new Vector3(0, 0, 0);
    }
}
