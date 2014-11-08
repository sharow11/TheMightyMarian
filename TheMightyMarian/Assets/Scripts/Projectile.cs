using UnityEngine;
using System.Collections;

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
        Destroy(blastObj, 1);
    }
}
