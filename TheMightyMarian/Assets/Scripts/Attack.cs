﻿using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    public enum Spell : byte { None, BlueBolt, LightningBolt, Rail, FireBolt, Light };
    public GameObject blueBolt, lightningBolt, rail, rail_around, fireBolt, light, AOE;
    public GameObject blueBlast, fireBlast, lightningBoltBlast;
    public GameObject arrow;
    public GameObject sword;
    LayerMask mask;
    public float lightningBoltRange = 20;
    public float railRange = 50;
    RaycastHit hitInfo;
    OnMarianCamera camera;
    public float angle;
    GameObject marian;
    public float projectileSpeed = 10;
    GameObject shot;
    Projectile shotProj;
	// Use this for initialization
	void Start () {
        camera = (OnMarianCamera)GameObject.Find("MainCamera").GetComponent("OnMarianCamera");
        marian = GameObject.Find("Marian");
        mask = LayerMask.GetMask("Marian");
        mask += LayerMask.GetMask("MarianProjectile");
        mask += LayerMask.GetMask("EnemyProjectile");
        mask = ~mask;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            switch (Marian.currAttackType)
            {
                case Marian.AttackType.Spell:
                    switch (Marian.currSpell)
                    {
                        case Spell.BlueBolt:
                            if (Marian.currMana > 5)
                            {
                                shot = (GameObject)Instantiate(blueBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                                Destroy(shot, 3);
                                shotProj = (Projectile)shot.GetComponent("Projectile");
                                shotProj.blast = blueBlast;
                                shotProj.type = Spell.BlueBolt;
                                shotProj.origin = new Vector3(transform.position.x, transform.position.y + 0.5f, -2);
                                /*float angle = Vector3.Angle(new Vector3(camera.hit.point.x, camera.hit.point.y, -1) - marian.transform.position, Vector3.up);
                                shot.transform.Rotate(Vector3.forward, angle, Space.World);*/
                                //shot.transform.rotation = Quaternion.LookRotation(new Vector3(camera.hit.point.x, camera.hit.point.y, -1) - transform.position, -Vector3.forward);
                                shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                                shot.rigidbody.velocity = (new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - shot.transform.position).normalized * projectileSpeed;
                                Marian.currMana -= 5;
                            }
                            break;
                        case Spell.LightningBolt:
                            if (Marian.currAttackType == Marian.AttackType.Spell)
                            {
                                if (Marian.currMana > 2)
                                {
                                    Marian.currMana -= 2;
                                    if (shot != null && shot.name == "LightningBolt(Clone)")
                                        Destroy(shot);
                                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                                        new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0), out hitInfo, lightningBoltRange, mask))
                                    {
                                        GameObject lb = lightningBolt;
                                        lb.particleSystem.startSpeed = Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 0.5f, -2), hitInfo.point) + 1;
                                        Vector3 blastPos = hitInfo.point - (hitInfo.point - new Vector3(transform.position.x, transform.position.y + 0.5f, -2)).normalized / 4;
                                        GameObject blastObj = (GameObject)Instantiate(lightningBoltBlast, new Vector3(blastPos.x, blastPos.y, -2), new Quaternion());
                                        //ToDo: LIGHT i GFX osobno!
                                        Destroy(blastObj, 0.05f);
                                        Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                                        if (enemy != null)
                                        {
                                            enemy.takeDmg(200 * Time.deltaTime, new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0).normalized);
                                            enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                                        }
                                    }
                                    else
                                    {
                                        lightningBolt.particleSystem.startSpeed = lightningBoltRange;
                                    }
                                    shot = (GameObject)Instantiate(lightningBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                                }
                            }
                            break;
                        case Spell.Rail:
                            break;
                        case Spell.FireBolt:
                            break;
                        case Spell.Light:
                            break;
                    }
                    break;
                case Marian.AttackType.Melee:
                    Vector3 v = new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0).normalized;
                    Vector3 from = new Vector3(transform.position.x, transform.position.y, -2);
                    Vector3 to = new Vector3(camera.hit.point.x, camera.hit.point.y, -2);

                    shot = (GameObject)Instantiate(sword, new Vector3(transform.position.x + v.x * 2, transform.position.y + v.y * 2, -2), Quaternion.identity);
                    shot.transform.LookAt(to, -Vector3.forward);
                    shot.transform.RotateAround(transform.position, new Vector3(Vector3.back.x + 0.1f, Vector3.back.y + 0.2f, Vector3.back.z + 0.1f), -45);
                    Destroy(shot, 0.2f);
                    //shot.transform.Rotate();
                    break;
                case Marian.AttackType.Ranged:
                    shot = (GameObject)Instantiate(arrow, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), Quaternion.identity);
                    Destroy(shot, 3);
                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2),  -Vector3.forward);
                    shot.transform.Rotate(Vector3.back, 90);
                    shot.transform.Rotate(Vector3.up, 90);
                    shot.rigidbody.velocity = (new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - shot.transform.position).normalized * projectileSpeed;
                    break;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
            switch (Marian.currSpell)
            {
                case Spell.LightningBolt:
                    if (Marian.currAttackType == Marian.AttackType.Spell)
                    {
                        if (Marian.currMana > 2)
                        {
                            Marian.currMana -= 2;
                            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                                 new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0), out hitInfo, lightningBoltRange, mask))
                            {
                                GameObject lb = lightningBolt;
                                lb.particleSystem.startSpeed = Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 0.5f, -2), hitInfo.point) + 1;
                                Vector3 blastPos = hitInfo.point - (hitInfo.point - new Vector3(transform.position.x, transform.position.y + 0.5f, -2)).normalized / 4;
                                GameObject blastObj = (GameObject)Instantiate(lightningBoltBlast, new Vector3(blastPos.x, blastPos.y, -2), new Quaternion());
                                //ToDo: LIGHT i GFX osobno!
                                Destroy(blastObj, 0.05f);
                                Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                                if (enemy != null)
                                {
                                    enemy.takeDmg(100 * Time.deltaTime, new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0).normalized);
                                    enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                                }
                            }
                            else
                            {
                                lightningBolt.particleSystem.startSpeed = lightningBoltRange;
                            }
                            shot = (GameObject)Instantiate(lightningBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                            shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                        }
                    }
                    break;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
        }
        ///NEW SPELLS///
        if (Input.GetKeyDown("2") && !Gui.cd2Start)
        {
            if (Marian.currMana > 20)
            {
                Marian.currMana -= 20;
                if (shot != null && shot.name == "LightningBolt(Clone)")
                    Destroy(shot);
                LayerMask wallMask = LayerMask.GetMask("Wall");
                float range = railRange;
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                    new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0), out hitInfo, lightningBoltRange, wallMask))
                {
                    range = Vector3.Distance(hitInfo.point, new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                    rail.particleSystem.startSpeed = range;
                    rail_around.particleSystem.startLifetime = range * 3 / 50;
                    rail_around.transform.Find("Rail_around2").particleSystem.startLifetime = range * 3 / 50;
                }
                else
                {
                    rail.particleSystem.startSpeed = railRange;
                    rail_around.particleSystem.startLifetime = railRange * 3 / 50;
                    rail_around.transform.Find("Rail_around2").particleSystem.startLifetime = railRange * 3 / 50;
                }
                RaycastHit[] raycastHits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                            new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0), range, mask);
                foreach (RaycastHit hit in raycastHits)
                {
                    Enemy enemy = (Enemy)hit.collider.GetComponent("Enemy");
                    if (enemy != null && !enemy.gotRaild)
                    {
                        enemy.takeDmg(60, new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0).normalized);
                        enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                        enemy.gotRaild = true;
                    }
                }
                shot = (GameObject)Instantiate(rail, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                Destroy(shot, 0.15f);
                shot = (GameObject)Instantiate(rail_around, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                Destroy(shot, 0.4f);
            }
        }
        if (Input.GetKeyDown("1") && !Gui.cd1Start)
        {
            if (Marian.currMana > 30)
            {
                Marian.currMana -= 30;
                if (shot != null && shot.name == "LightningBolt(Clone)")
                    Destroy(shot);
                shot = (GameObject)Instantiate(fireBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                Destroy(shot, 3);
                shotProj = (Projectile)shot.GetComponent("Projectile");
                shotProj.origin = new Vector3(transform.position.x, transform.position.y + 0.5f, -2);
                shotProj.blast = fireBlast;
                shotProj.type = Spell.FireBolt;
                shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -0.75f));
                shot.rigidbody.velocity = (new Vector3(camera.hit.point.x, camera.hit.point.y, -0.75f) - shot.transform.position).normalized * projectileSpeed;
            }
        }
        if (Input.GetKeyDown("4") && !Gui.cd4Start)
        {
            if (Marian.currMana > 5)
            {
                Marian.currMana -= 5;
                if (shot != null && shot.name == "LightningBolt(Clone)")
                    Destroy(shot);
                shot = (GameObject)Instantiate(light, new Vector3(camera.hit.point.x, camera.hit.point.y, -3), new Quaternion());
                Destroy(shot, 10.6f);
            }
        }
        if (Input.GetKeyDown("5") && !Gui.cd4Start)
        {
            if (Marian.currMana > 100)
            {
                Marian.currMana -= 100;
                if (shot != null && shot.name == "LightningBolt(Clone)")
                    Destroy(shot);
                shot = (GameObject)Instantiate(AOE, new Vector3(camera.hit.point.x, camera.hit.point.y, -0.1f), new Quaternion());
                Destroy(shot, 13.5f);
            }
        }
        if (Input.GetKeyDown("q"))
        {
            if(Marian.currAttackType == Marian.AttackType.Melee)
            {
                Marian.currAttackType = Marian.AttackType.Ranged;
            }
            else if(Marian.currAttackType == Marian.AttackType.Ranged)
            {
                Marian.currAttackType = Marian.AttackType.Spell;
            }
            else if(Marian.currAttackType == Marian.AttackType.Spell)
            {
                Marian.currAttackType = Marian.AttackType.Melee;
            }
        }
	}
}
