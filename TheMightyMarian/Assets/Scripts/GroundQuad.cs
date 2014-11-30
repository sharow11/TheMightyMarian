using UnityEngine;
using System.Collections;

public class GroundQuad : MonoBehaviour {
    public Material ground;
    public Material water;

    public void beWater()
    {
        renderer.material = water;
        transform.position = new Vector3(0, 0, 0.1f);
    }

    public void beFloor()
    {
        renderer.material = ground;
        transform.position = new Vector3(0, 0, 0);
    }
}
