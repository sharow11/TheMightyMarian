using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour 
{
    Enemy enemy;
	// Use this for initialization
	void Start () 
    {
        enemy = (Enemy)this.GetComponent("Enemy");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyProjectile"), LayerMask.NameToLayer("Enemy"));
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("wall");
        }
        if (other.CompareTag("MarianProjectile"))
        {
            Debug.Log("Marian proj");
            enemy.takeDmg(((Projectile)other.gameObject.GetComponent("Projectile")).dmg, (transform.position - ((Projectile)other.gameObject.GetComponent("Projectile")).origin).normalized);
            ((Projectile)other.gameObject.GetComponent("Projectile")).Blast();
            enemy.alertEnemy(((Projectile)other.gameObject.GetComponent("Projectile")).origin);
            Destroy(other.gameObject, 0);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            //Debug.Log("wall");
            Vector3 push = enemy.pushAwayFromWalls;
            enemy.pushAwayFromWalls = new Vector3(enemy.pushAwayFromWalls.x + transform.position.x - other.transform.position.x, enemy.pushAwayFromWalls.y + transform.position.y - other.transform.position.y, 0).normalized * 3;
        }
    }
}
