using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public Vector3 origin;
    public Attack.Spell type = Attack.Spell.None;
    public GameObject blast;
    public float dmg;
    // Use this for initialization
    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("MarianProjectile"), LayerMask.NameToLayer("EnemyProjectile"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("MarianProjectile"), LayerMask.NameToLayer("MarianProjectile"));
        switch (type)
        {
            case Attack.Spell.BlueBolt:
                dmg = 20;
                break;
            case Attack.Spell.FireBolt:
                dmg = 90;
                break;
            default:
                dmg = 10;
                break;
        }
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
        if (type == Attack.Spell.FireBolt)
        {
            List<Enemy> dmgRecipients = new List<Enemy>();
            foreach (Enemy enemy in Enemy.enemies)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < 10 && !Physics.Raycast(new Ray(transform.position,
                    origin - new Vector3(transform.position.x, transform.position.y, -1)), 10, LayerMask.GetMask("Wall")))
                {
                    dmgRecipients.Add(enemy);
                }
            }
            foreach (Enemy enemy in dmgRecipients) //Enemy.enemies zmienia się podczas umierania wrogów, dlatego nie można połączyć tych 2 foreachów.
            {
                enemy.alertEnemy(origin);
                enemy.takeDmg(dmg * 1.3f / (Vector3.Distance(enemy.transform.position, transform.position) / 10 + 1) - dmg / 2, enemy.transform.position - transform.position);
            }
        }
        Destroy(blastObj, 1);
    }
}
