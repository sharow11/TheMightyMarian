﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public Vector3 origin;
    public Attack.Spell type = Attack.Spell.None;
    public GameObject blast;
    public float dmg = 10;
    // Use this for initialization
    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("MarianProjectile"), LayerMask.NameToLayer("EnemyProjectile"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("MarianProjectile"), LayerMask.NameToLayer("MarianProjectile"));
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
        GameObject blastObj = (GameObject)Instantiate(blast, transform.position, new Quaternion());
        Destroy(blastObj, 1);
    }
}
