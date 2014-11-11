﻿using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {
    public GameObject blast;
    public float dmg = 10;
    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("EnemyProjectile"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("MarianProjectile"));
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        Blast();
        Destroy(this.gameObject);
    }
    public void Blast()
    {
        GameObject marian = GameObject.Find("Marian");
        if (Vector3.Distance(marian.transform.position, transform.position) < 10)
        {
            Marian.currHp -= 5;
            GameObject blastObj = (GameObject)Instantiate(blast, transform.position, new Quaternion());
            Destroy(blastObj, 1);
        }
    }
}
