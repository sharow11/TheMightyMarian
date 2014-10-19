using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("block"))
        {
            Debug.Log("wall");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("block"))
        {
            //Debug.Log("wall");
            Enemy enemy = (Enemy)this.GetComponent("Enemy");
            Vector3 push = enemy.pushAwayFromWalls;
            //if(transform.position.x - other.transform.position.x
            //jeśli w lewo to w lewo itd, normalize n shit
            enemy.pushAwayFromWalls = new Vector3(enemy.pushAwayFromWalls.x + transform.position.x - other.transform.position.x, enemy.pushAwayFromWalls.y + transform.position.y - other.transform.position.y, 0).normalized * 5;
        }
    }
}
