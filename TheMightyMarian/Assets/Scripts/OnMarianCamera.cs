using UnityEngine;
using System.Collections;

public class OnMarianCamera : MonoBehaviour {
    GameObject marian;
	// Use this for initialization
	void Start () {
	    marian = GameObject.Find("Marian");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(marian.transform.position.x, marian.transform.position.y - 10, marian.transform.position.z - 10);
	}
}
