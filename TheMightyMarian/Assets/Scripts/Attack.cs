using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Attack : MonoBehaviour {
    public enum Spell : byte { None, BlueBolt, LightningBolt, Rail, FireBolt, Light, Heal, Rage, Fast, Empower, Shout, Eruption};
    public GameObject blueBolt, lightningBolt, rail, rail_around, fireBolt, light, AOE, heal;
    public GameObject blueBlast, fireBlast, lightningBoltBlast;
    public GameObject arrow;
    public GameObject sword;
    public GameObject rage;
    public GameObject shout;
    public GameObject goFast;
    public GameObject redSword;
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
    float lastLightningSoundTime = 0;
    float bloodyScreenAlpha = 0f;
    GUITexture bloodyScreen;
    public AudioClip s_lightning, s_hurt;

	// Use this for initialization
	void Start () {
        camera = (OnMarianCamera)GameObject.Find("MainCamera").GetComponent("OnMarianCamera");
        marian = GameObject.Find("Marian");
        mask = LayerMask.GetMask("Marian");
        mask += LayerMask.GetMask("MarianProjectile");
        mask += LayerMask.GetMask("EnemyProjectile");
        mask = ~mask;
        bloodyScreen = (GUITexture)GameObject.Find("BloodyScreen").GetComponent<GUITexture>();
	}
	
	// Update is called once per frame
	void Update () {
        //audio.PlayOneShot(s_lightning);
        bloodyScreen.color = new Color(bloodyScreen.color.r, bloodyScreen.color.g, bloodyScreen.color.b, bloodyScreenAlpha);
        bloodyScreenAlpha -= Time.deltaTime / 5;
        if (bloodyScreenAlpha < 0)
            bloodyScreenAlpha = 0;
        else if (bloodyScreenAlpha > 1)
            bloodyScreenAlpha = 1;

        if (Input.GetKeyDown("p"))
        {
            Debug.Break();
        }
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
                                shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2));
                                shot.rigidbody.velocity = (new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2) - shot.transform.position).normalized * projectileSpeed;
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
                                        new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0), out hitInfo, lightningBoltRange, mask))
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
                                            enemy.takeDmg(200 * Time.deltaTime, new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0).normalized);
                                            enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                                        }
                                    }
                                    else
                                    {
                                        lightningBolt.particleSystem.startSpeed = lightningBoltRange;
                                    }
                                    shot = (GameObject)Instantiate(lightningBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                                    shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2));
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
                    Vector3 v = new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0).normalized;
                    Vector3 from = new Vector3(transform.position.x, transform.position.y, -2);
                    Vector3 to = new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2);
                    if (Marian.Empower)
                    {
                        shot = (GameObject)Instantiate(redSword, new Vector3(transform.position.x + v.x * 2, transform.position.y + v.y * 2, -2), Quaternion.identity);
                    }
                    else
                    {
                        shot = (GameObject)Instantiate(sword, new Vector3(transform.position.x + v.x * 2, transform.position.y + v.y * 2, -2), Quaternion.identity);
                    }
                    shot.GetComponentInChildren<SwordRotate>().IsAttack = true;
                    shot.transform.LookAt(to, -Vector3.forward);
                    shot.transform.RotateAround(transform.position, new Vector3(Vector3.back.x + 0.1f, Vector3.back.y + 0.2f, Vector3.back.z + 0.1f), -45);
                    Destroy(shot, 0.2f);
                    //shot.transform.Rotate();
                    break;
                case Marian.AttackType.Ranged:
                    Vector3 offset = (camera.hitAbove - new Vector3(transform.position.x, transform.position.y, hitInfo.point.z)).normalized;
                    shot = (GameObject)Instantiate(arrow, new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, -2), Quaternion.identity);
                    Destroy(shot, 5);
                    shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2), -Vector3.forward);
                    shot.transform.Rotate(Vector3.back, 90);
                    shot.transform.Rotate(Vector3.up, 90);
                    shot.rigidbody.velocity = (new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2) - shot.transform.position).normalized * projectileSpeed * 2;
                    shot.GetComponent<ArrowCollide>().origin = marian.transform.position;
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
                            if (Time.time - lastLightningSoundTime > 0.02f)
                            //if (!audio.isPlaying)
                            {
                                lastLightningSoundTime = Time.time;
                                //audio.Play();
                                audio.PlayOneShot(s_lightning, 0.2f);
                            }
                            Marian.currMana -= 2;
                            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                                 new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0), out hitInfo, lightningBoltRange, mask))
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
                                    enemy.takeDmg(100 * Time.deltaTime, new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0).normalized);
                                    enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                                }
                            }
                            else
                            {
                                lightningBolt.particleSystem.startSpeed = lightningBoltRange;
                            }
                            shot = (GameObject)Instantiate(lightningBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                            shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2));
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
        if (Input.GetKeyDown("1") && !Gui.cd1Start)
        {
            castSpell(Marian.Spell1);
        }
        if (Input.GetKeyDown("2") && !Gui.cd2Start)
        {
            castSpell(Marian.spell2);
        }
        if (Input.GetKeyDown("3") && !Gui.cd3Start)
        {
            castSpell(Marian.spell3);
        }
        if (Input.GetKeyDown("4") && !Gui.cd4Start)
        {
            castSpell(Marian.spell4);
        }
        if (Input.GetKeyDown("5") && !Gui.cd5Start)
        {
            castSpell(Marian.spell5);
        }
        if (Input.GetKeyDown("q"))
        {
            castSpell(Spell.Light);
        }
	}
    public void takeDmg(int dmg)
    {
        Music.hype += (float)dmg / 10;
        bloodyScreenAlpha += (float)dmg / 100;
        Marian.currHp -= dmg;
        audio.PlayOneShot(s_hurt, 0.5f);
    }

    public void castSpell(Spell spell)
    {
        switch (spell)
        {
            case Spell.None:
                break;
            case Spell.BlueBolt:
                break;
            case Spell.LightningBolt:
                break;
            case Spell.Rail:
                if (Marian.currMana > 20)
                {
                    Marian.currMana -= 20;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    LayerMask wallMask = LayerMask.GetMask("Wall");
                    float range = railRange;
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, -2),
                        new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0), out hitInfo, railRange, wallMask))
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
                                new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0), range, mask);
                    foreach (RaycastHit hit in raycastHits)
                    {
                        Enemy enemy = (Enemy)hit.collider.GetComponent("Enemy");
                        if (enemy != null && !enemy.gotRaild)
                        {
                            enemy.takeDmg(60, new Vector3(camera.hitAbove.x - transform.position.x, camera.hitAbove.y - transform.position.y - 0.5f, 0).normalized);
                            enemy.alertEnemy(new Vector3(transform.position.x, transform.position.y + 0.5f, -2));
                            enemy.gotRaild = true;
                        }
                    }
                    shot = (GameObject)Instantiate(rail, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2));
                    Destroy(shot, 0.15f);
                    shot = (GameObject)Instantiate(rail_around, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    shot.transform.LookAt(new Vector3(camera.hitAbove.x, camera.hitAbove.y, -2));
                    Destroy(shot, 0.8f);
                }
                break;
            case Spell.FireBolt:
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
                break;
            case Spell.Light:
                if (Marian.currMana > 5)
                {
                    Marian.currMana -= 5;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(light, new Vector3(camera.hitAbove.x, camera.hitAbove.y, -3), new Quaternion());
                    Destroy(shot, 10.6f);
                }
                break;
            case Spell.Heal:
                if (Marian.currMana > 100)
                {
                    Marian.currMana -= 100;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(heal, new Vector3(marian.transform.position.x, marian.transform.position.y, -2f), new Quaternion());
                    shot.transform.LookAt(new Vector3(marian.transform.position.x, marian.transform.position.y, -99999f));
                    if (Marian.currHp + 100 <= Marian.HP)
                    {
                        Marian.currHp += 100;
                    }
                    else
                    {
                        Marian.currHp = Marian.HP;
                    }
                    Destroy(shot, 10f);
                }
                break;
            case Spell.Rage:
                if (Marian.currMana > 10)
                {
                    Marian.currMana -= 10;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(rage, new Vector3(marian.transform.position.x, marian.transform.position.y, -2f), new Quaternion());
                    shot.transform.LookAt(new Vector3(marian.transform.position.x, marian.transform.position.y, -99999f));
                    Marian.LifeSteal = true;
                    Destroy(shot, 2f);
                }
                break;
            case Spell.Fast:
                if (Marian.currMana > 10)
                {
                    Marian.currMana -= 10;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(goFast, new Vector3(marian.transform.position.x, marian.transform.position.y, -2f), new Quaternion());
                    shot.transform.LookAt(new Vector3(marian.transform.position.x, marian.transform.position.y, -99999f));
                    Marian.Speed += 8;
                    Marian.GoFast = true;
                    Destroy(shot, 2f);
                }
                break;
            case Spell.Empower:
                if (Marian.currMana > 10)
                {
                    Marian.Empower = true;
                }
                break;
            case Spell.Shout:
                if (Marian.currMana > 10)
                {
                    Marian.currMana -= 10;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(shout, new Vector3(marian.transform.position.x, marian.transform.position.y, -2f), new Quaternion());
                    shot.transform.LookAt(new Vector3(marian.transform.position.x, marian.transform.position.y, -99999f));
                    Destroy(shot, 10f);
                }
                break;
            case Spell.Eruption:
                if (Marian.currMana > 100)
                {
                    Marian.currMana -= 100;
                    if (shot != null && shot.name == "LightningBolt(Clone)")
                        Destroy(shot);
                    shot = (GameObject)Instantiate(AOE, new Vector3(camera.hit.point.x, camera.hit.point.y, -0.1f), new Quaternion());
                    shot.GetComponent<AoEDmg>().origin = marian.transform.position;
                    Destroy(shot, 13.5f);
                }
                break;
            default:
                break;
        }
    }
}
