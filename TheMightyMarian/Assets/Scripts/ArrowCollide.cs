using UnityEngine;
using System.Collections;

public class ArrowCollide : MonoBehaviour {
    public Vector3 origin;
    Vector3 offset;
    bool gotEnemyToStick = false;
    bool isDropping = false;
    bool vibrate = false;
    float magnitude = 5f;
    Vector3 initVel;
    Enemy enemyToStick;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (gotEnemyToStick)
        {
            if (enemyToStick == null)
            {
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.None;
                if (!isDropping)
                {
                    rigidbody.angularVelocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    rigidbody.velocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    isDropping = true;
                }
            }
            else
            {
                transform.rigidbody.position = enemyToStick.transform.position + offset;
            }
        }
        if (vibrate && magnitude > 0)
        {
            if(initVel.y*initVel.y < initVel.x*initVel.x)
                transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 100) * magnitude, 0, 0) * transform.localRotation;
            else
                transform.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 100) * magnitude, 0) * transform.localRotation;
            magnitude -= Time.deltaTime * 2;
        }
	}
    void OnTriggerEnter(Collider collision)
    {
        if (!gotEnemyToStick)
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.takeDmg(20, enemy.transform.position - transform.position);
            enemy.alertEnemy(origin);
            enemyToStick = enemy;
            offset = transform.position - enemy.transform.position;
            gotEnemyToStick = true;
        }
        //Destroy(this.gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        initVel = rigidbody.velocity;
        rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        rigidbody.angularVelocity = new Vector3();
        transform.rigidbody.velocity = new Vector3();
        ((Collider)this.GetComponentInChildren<Collider>()).enabled = false;
        vibrate = true;
    }
}
