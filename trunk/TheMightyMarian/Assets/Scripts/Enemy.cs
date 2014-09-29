using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public bool isAttacking;
    public GameObject Marian;
	// Use this for initialization
	void Start () {
        isAttacking = false;
        Marian = GameObject.Find("Marian");
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(Marian.transform.position, transform.position) <= 10)
            isAttacking = true;
        else
            isAttacking = false;
        if (isAttacking)
        {
            if (Physics.Raycast(transform.position, transform.position - Marian.transform.position, 10))
                print("There is something in front of the object!");
        }
	}
}
