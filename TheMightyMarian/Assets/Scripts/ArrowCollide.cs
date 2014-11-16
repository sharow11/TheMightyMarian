﻿using UnityEngine;
using System.Collections;

public class ArrowCollide : MonoBehaviour {
    public Vector3 origin;
    Vector3 offset;
    bool gotEnemyToStick = false;
    bool isDropping = false;
    Enemy enemyToStick;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (gotEnemyToStick)
        {
            if (enemyToStick == null)
            {
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.None;
                if (!isDropping)
                {
                    rigidbody.angularVelocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    rigidbody.velocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    isDropping = true;
                }
            }
            else
            {
                transform.rigidbody.position = enemyToStick.transform.position + offset;
            }
        }
	}
    void OnTriggerEnter(Collider collision)
    {
        if (!gotEnemyToStick)
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.takeDmg(20, enemy.transform.position - transform.position);
            enemy.alertEnemy(origin);
            enemyToStick = enemy;
            offset = transform.position - enemy.transform.position;
            gotEnemyToStick = true;
        }
        //Destroy(this.gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        transform.rigidbody.velocity = new Vector3();
    }
}
