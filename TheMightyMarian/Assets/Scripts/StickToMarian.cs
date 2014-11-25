using UnityEngine;
using System.Collections;

public class StickToMarian : MonoBehaviour {
    Vector3 offset = new Vector3();
    GameObject marian;
	// Use this for initialization
	void Start () {
        marian = GameObject.Find("Marian");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = marian.transform.position + offset;
	}
    public void setOffset(Vector3 ofst){
        offset = ofst;
    }
}
