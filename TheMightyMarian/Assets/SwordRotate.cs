using UnityEngine;
using System.Collections;

public class SwordRotate : MonoBehaviour {

    float orbitSpeed;
    Transform target;
	// Use this for initialization
	void Start () 
    {
        orbitSpeed = 500.0f;
        target = GameObject.Find("Marian").transform;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Marian"), LayerMask.NameToLayer("Sword"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Sword"));
	}
	// Update is called once per frame
	void Update () 
    {
        transform.RotateAround( target.position, new Vector3(Vector3.back.x + 0.1f, Vector3.back.y +0.2f, Vector3.back.z + 0.1f), orbitSpeed * Time.deltaTime );
	}
    void OnTriggerEnter(Collider collision)
    {
        var enemy = collision.gameObject.GetComponent<Enemy>();
        enemy.takeDmg(20, enemy.transform.position - transform.position);
    }
}
