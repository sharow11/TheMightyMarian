using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public Attack.Spell type = Attack.Spell.None;
    public GameObject blast;
    public float dmg = 10;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        Blast();
        Enemy e = new Enemy();
        Destroy(this.gameObject);
        //GameObject shot = (GameObject)Instantiate(effect, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z - 1), new Quaternion());
    }
    public void Blast()
    {
        GameObject blastObj = (GameObject)Instantiate(blast, transform.position, new Quaternion());
        Destroy(blastObj, 1);
    }
}
