using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("hi i'm ladder");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("don tacz mi");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Marian"))
        {
            GameManager gm = (GameManager)FindObjectOfType(typeof(GameManager));
            gm.levelUp();
        }
     }
}
