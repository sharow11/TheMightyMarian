using UnityEngine;
using System.Collections;
using System;

public class MeatSplat : MonoBehaviour {
    public GameObject meatSplash;
    float time;

	// Use this for initialization
	void Start () {
        time = Time.time;
	}

    void OnCollisionEnter(Collision collision)
    {
        GameObject bloodObj = (GameObject)Instantiate(meatSplash, transform.position, new Quaternion());
        Destroy(bloodObj, 0.5f);
    }
}
