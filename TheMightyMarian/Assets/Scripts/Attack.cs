using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    public enum Spell : byte { None, BlueBolt, LightningBolt, Rail, FireBolt, Light };
    public GameObject blueBolt, lightningBolt, rail, rail_around, fireBolt, light;
    public GameObject blueBlast, fireBlast, lightningBoltBlast;
    public static Spell currSpell = Spell.BlueBolt;
    public static bool fireBlueBolt = false, fireLightningBolt = false, fireRail = false, fireFireBolt = false, fireLight = false;
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
            switch (currSpell)
            {
                case Spell.BlueBolt:
                    shot = (GameObject)Instantiate(blueBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, - 2), new Quaternion());
                    Destroy(shot, 3);
                    shotProj = (Projectile)shot.GetComponent("Projectile");
                    shotProj.blast = blueBlast;
                    shotProj.type = Spell.BlueBolt;
                    shotProj.origin = new Vector3(transform.position.x, transform.position.y + 0.5f, - 2);
                    /*float angle = Vector3.Angle(new Vector3(camera.hit.point.x, camera.hit.point.y, -1) - marian.transform.position, Vector3.up);
                    shot.transform.Rotate(Vector3.forward, angle, Space.World);*/
                    //shot.transform.rotation = Quaternion.LookRotation(new Vector3(camera.hit.point.x, camera.hit.point.y, -1) - transform.position, -Vector3.forward);
                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                    shot.rigidbody.velocity = (new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - shot.transform.position).normalized * projectileSpeed;
                    break;
                case Spell.LightningBolt:
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
                    break;
                case Spell.Rail:
                    break;
                case Spell.FireBolt:
                    break;
                case Spell.Light:
                    break;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
            switch (currSpell)
            {
                case Spell.LightningBolt:
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
                    break;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
        }
        ///NEW SPELLS///
        if (fireRail == true)
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                        new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0), out hitInfo, railRange, mask))
            {
                rail.particleSystem.startSpeed = Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 0.5f, -2), hitInfo.point) + 0.5f;
                rail_around.particleSystem.startLifetime = (Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 0.5f, -2), hitInfo.point) + 0.5f) * 3 / 50;
                rail_around.transform.Find("Rail_around2").particleSystem.startLifetime = (Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 0.5f, -2), hitInfo.point) + 0.5f) * 3 / 50;
                Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                if (enemy != null)
                {
                    enemy.takeDmg(60, new Vector3(camera.hit.point.x - transform.position.x, camera.hit.point.y - transform.position.y - 0.5f, 0).normalized);
                    enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                }
            }
            else
            {
                rail.particleSystem.startSpeed = railRange;
                rail_around.particleSystem.startLifetime = railRange * 3 / 50;
                rail_around.transform.Find("Rail_around2").particleSystem.startLifetime = railRange * 3 / 50;
            }
            shot = (GameObject)Instantiate(rail, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
            shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
            Destroy(shot, 0.15f);
            shot = (GameObject)Instantiate(rail_around, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
            shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
            Destroy(shot, 0.4f);
            fireRail = false;
        }
        else if (fireFireBolt == true)
        {
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
            fireFireBolt = false;
        }
        else if (fireLight == true)
        {
            if (shot != null && shot.name == "LightningBolt(Clone)")
                Destroy(shot);
            shot = (GameObject)Instantiate(light, new Vector3(camera.hit.point.x, camera.hit.point.y, -3), new Quaternion());
            Destroy(shot, 10.6f);
            fireLight = false;
        }
	}
}
