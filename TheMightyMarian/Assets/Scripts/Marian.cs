using UnityEngine;
using System.Collections;

public class Marian : MonoBehaviour {

    public static int HP = 1000;
	// Use this for initialization
	void Start () {
	     HP = 1000;
	}
	
	// Update is called once per frame
	void Update () {
	    if(HP <= 0)
        {
            Application.LoadLevel(2);
        }
	}
}
