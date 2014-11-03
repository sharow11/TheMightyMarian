using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    public enum Spell : byte { None, BlueBolt, LightningBolt, Rail, FireBolt, Light };
    public GameObject blueBolt, lightningBolt, rail, fireBolt, light;
    public GameObject blueBlast, fireBlast, lightningBoltBlast;
    public static Spell currSpell = Spell.BlueBolt;
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
                    if (Physics.Raycast(transform.position, new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - transform.position, out hitInfo, lightningBoltRange, mask))
                    {
                        GameObject lb = lightningBolt;
                        lb.particleSystem.startSpeed = Vector3.Distance(transform.position, hitInfo.point) + 1;
                        GameObject blastObj = (GameObject)Instantiate(lightningBoltBlast, marian.transform.position + (hitInfo.point - marian.transform.position) * 0.95f, new Quaternion());
                        Destroy(blastObj, 0.2f);
                        Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                        if (enemy != null)
                        {
                            enemy.takeDmg(100 * Time.deltaTime);
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
                    if (Physics.Raycast(transform.position, new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - transform.position, out hitInfo, railRange, mask))
                    {
                        rail.particleSystem.startSpeed = Vector3.Distance(transform.position, hitInfo.point) + 0.5f;
                        Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                        if (enemy != null)
                        {
                            enemy.takeDmg(60);
                            enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                        }
                    }
                    else
                    {
                        rail.particleSystem.startSpeed = railRange;
                    }
                    shot = (GameObject)Instantiate(rail, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                    Destroy(shot, 0.15f);
                    break;
                case Spell.FireBolt:
                    shot = (GameObject)Instantiate(fireBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, - 2), new Quaternion());
                    Destroy(shot, 3);
                    shotProj = (Projectile)shot.GetComponent("Projectile");
                    shotProj.origin = new Vector3(transform.position.x, transform.position.y + 0.5f, - 2);
                    shotProj.blast = fireBlast;
                    shotProj.type = Spell.FireBolt;
                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                    shot.rigidbody.velocity = (new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - shot.transform.position).normalized * projectileSpeed;
                    break;
                case Spell.Light:
                    shot = (GameObject)Instantiate(light, new Vector3(camera.hit.point.x, camera.hit.point.y, -3), new Quaternion());
                    Destroy(shot, 10.6f);
                    break;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            switch (currSpell)
            {
                case Spell.LightningBolt:
                    Destroy(shot);
                    if (Physics.Raycast(transform.position, new Vector3(camera.hit.point.x, camera.hit.point.y, -2) - transform.position, out hitInfo, lightningBoltRange, mask))
                    {
                        GameObject lb = lightningBolt;
                        lb.particleSystem.startSpeed = Vector3.Distance(transform.position, hitInfo.point) + 1;
                        GameObject blastObj = (GameObject)Instantiate(lightningBoltBlast, hitInfo.point - (hitInfo.point - marian.transform.position).normalized / 2, new Quaternion());
                        //ToDo: LIGHT i GFX osobno!
                        Destroy(blastObj, 0.2f);
                        Enemy enemy = (Enemy)hitInfo.collider.GetComponent("Enemy");
                        if (enemy != null)
                        {
                            enemy.takeDmg(100 * Time.deltaTime);
                            enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                        }
                    }
                    else
                    {
                        lightningBolt.particleSystem.startSpeed = lightningBoltRange;
                    }
                    shot = (GameObject)Instantiate(lightningBolt, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    shot.transform.LookAt(new Vector3(camera.hit.point.x, camera.hit.point.y, -2));
                    break;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            switch (currSpell)
            {
                case Spell.LightningBolt:
                    if(shot!=null)
                        Destroy(shot);
                    break;
            }
        }
	}
}
