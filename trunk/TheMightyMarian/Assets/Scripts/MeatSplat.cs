using UnityEngine;
using System.Collections;
using System;

public class MeatSplat : MonoBehaviour {

    public GameObject meatSplash;
    float time;
	// Use this for initialization
	void Start () {
        time = Time.time;
        //collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	    /*if(Time.time - time > 0.2f)
            collider.enabled = true;*/
	}

    void OnCollisionEnter(Collision collision)
    {
        //float temp = collision.relativeVelocity.x * collision.relativeVelocity.x + collision.relativeVelocity.y * collision.relativeVelocity.y + collision.relativeVelocity.z * collision.relativeVelocity.z;
        //if (temp > 9 || true)
        //{
        GameObject bloodObj = (GameObject)Instantiate(meatSplash, transform.position, new Quaternion());
            //bloodObj.transform.LookAt(transform.position + new Vector3(0, 0, -9999.9f));
            /*bloodObj.particleSystem.startSize = 1;
            bloodObj.particleSystem.startSpeed = 5;
            bloodObj.particleSystem.startLifetime = 0.15f;
            bloodObj.particleSystem.Emit(10);*/
            //bloodObj.particleSystem.emissionRate = 50;
        Destroy(bloodObj, 0.5f);
        //}
    }
    /*void OnCollisionStay(Collision collision)
    {
        Debug.Log("stay");
        if (UnityEngine.Random.value < 0.1f && Math.Abs(collision.relativeVelocity.x) + Math.Abs(collision.relativeVelocity.y) + Math.Abs(collision.relativeVelocity.z) > 1)
        {
            GameObject bloodObj = (GameObject)Instantiate(blood, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), new Quaternion());
            bloodObj.transform.LookAt(transform.position + new Vector3(0, 0, -9999.9f));
            bloodObj.particleSystem.startSize = 2;
            bloodObj.particleSystem.startSpeed = 10;
            bloodObj.particleSystem.startLifetime = 0.2f;
            bloodObj.particleSystem.Emit(20);
            //bloodObj.particleSystem.emissionRate = 50;
            Destroy(bloodObj, 1);
        }
    }*/
}
