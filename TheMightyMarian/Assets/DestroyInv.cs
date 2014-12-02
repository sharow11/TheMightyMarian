using UnityEngine;
using System.Collections;

public class DestroyInv : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        GameObject inventory = GameObject.Find("Inventory");

        Destroy(GameObject.Find("Inventory"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
