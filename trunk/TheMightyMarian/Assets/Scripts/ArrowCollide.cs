using UnityEngine;
using System.Collections;

public class ArrowCollide : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider collision)
    {
        var enemy = collision.gameObject.GetComponent<Enemy>();
        enemy.takeDmg(20, enemy.transform.position - transform.position);
        Destroy(this);
    }
}
