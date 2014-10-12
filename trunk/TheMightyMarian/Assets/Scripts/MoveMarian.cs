using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveMarian : MonoBehaviour {
    public int speed = 10;
    public List<Vector3> positions;
    List<float> times;

	// Use this for initialization
    void Start()
    {
        positions = new List<Vector3>();
        times = new List<float>();
	}
	
	// Update is called once per frame
	void Update () {
        rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * speed;
        positions.Add(transform.position);
        times.Add(Time.time);
        while (times.Count > 0 && Time.time - times[0] > 0.5f)
        {
            times.RemoveAt(0);
            positions.RemoveAt(0);
        }
	}
}
