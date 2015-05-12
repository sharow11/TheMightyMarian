using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ArrowCollide : MonoBehaviour {
    public Vector3 origin;
    Vector3 offset;
    bool gotEnemyToStick = false;
    bool isDropping = false;
    bool vibrate = false;
    float magnitude = 5f;
    Vector3 initVel;
    Enemy enemyToStick;
    public AudioClip s_arrow;
	
	// Update is called once per frame
	void Update () {
        if (gotEnemyToStick)
        {
            if (enemyToStick == null)
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                if (!isDropping)
                {
                    GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    GetComponent<Rigidbody>().velocity = new Vector3(Random.value, Random.value, Random.value) * 10;
                    isDropping = true;
                }
            }
            else
            {
                transform.GetComponent<Rigidbody>().position = enemyToStick.transform.position + offset;
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
            enemy.takeDmg(Marian.Damage, enemy.transform.position - transform.position);
            enemy.alertEnemy(origin);
            enemyToStick = enemy;
            offset = transform.position - enemy.transform.position;
            gotEnemyToStick = true;
            GetComponent<AudioSource>().PlayOneShot(s_arrow);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        initVel = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        GetComponent<Rigidbody>().angularVelocity = new Vector3();
        transform.GetComponent<Rigidbody>().velocity = new Vector3();
        ((Collider)this.GetComponentInChildren<Collider>()).enabled = false;
        vibrate = true;
        GetComponent<AudioSource>().PlayOneShot(s_arrow);
    }
}
