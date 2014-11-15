using UnityEngine;
using System.Collections;

public class StickToMarian : MonoBehaviour {

    GameObject marian;
	// Use this for initialization
	void Start () {
        marian = GameObject.Find("Marian");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = marian.transform.position;
	}
}
