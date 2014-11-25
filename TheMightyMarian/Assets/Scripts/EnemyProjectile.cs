using UnityEngine;
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
        if (collision.collider.gameObject.name == "Marian")
        {
            collision.collider.gameObject.GetComponent<Attack>().takeDmg((int)dmg);
        }
        Blast();
        Destroy(this.gameObject);
    }
    public void Blast()
    {
        GameObject blastObj = (GameObject)Instantiate(blast, transform.position, new Quaternion());
        Destroy(blastObj, 1);
    }
}
