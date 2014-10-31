using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveMarian : MonoBehaviour {
    public int speed = 10;
    public List<Vector3> positions;
    List<float> times;
    const float precision = 0.1f;

	// Use this for initialization
    void Start()
    {
        positions = new List<Vector3>();
        times = new List<float>();
        Physics.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("MarianProjectile"), true);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized * speed;
        positions.Add(transform.position);
        times.Add(Time.time);
        while (times.Count > 0 && Time.time - times[0] > 30)
        {
            times.RemoveAt(0);
            positions.RemoveAt(0);
        }
	}
    public int GetIndex(float time)
    {
        int begin = 0;
        int end = times.Count;
        int timeout = 0;
        //Debug.Log("end =" + end);
        while (begin != end && timeout < 10)
        {
            timeout++;
            float middle = times[begin + (end - begin) / 2];
            if (middle > time)
            {
                if (middle - time < precision)
                    return (end - begin) / 2;
				else
					end -= (end - begin) / 2;
            }
            else
            {
                if(time - middle < precision)
					return begin + (end - begin) / 2;
				else
					begin += (end - begin) / 2;
            }
        }
        return 0;
    }
    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Grass")
        if (collision.gameObject.tag == "MarianProjectile")
        {
            Physics.IgnoreCollision(collision.collider, collider);
        }
    }*/
}
