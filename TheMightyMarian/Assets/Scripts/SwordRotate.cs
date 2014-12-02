using UnityEngine;
using System.Collections;

public class SwordRotate : MonoBehaviour {

    float orbitSpeed;
    Transform target;
    public bool IsAttack = false;
	// Use this for initialization
	void Start () 
    {
        if (IsAttack)
        {
            orbitSpeed = 500.0f;
            target = GameObject.Find("Marian").transform;
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Marian"), LayerMask.NameToLayer("Sword"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Sword"));
        }
	}
	// Update is called once per frame
	void Update () 
    {
        if (IsAttack)
        {
            transform.RotateAround(target.position, new Vector3(Vector3.back.x + 0.1f, Vector3.back.y + 0.2f, Vector3.back.z + 0.1f), orbitSpeed * Time.deltaTime);
        }
	}
    void OnTriggerEnter(Collider collision)
    {
        if (IsAttack)
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (Marian.Empower)
            {
                enemy.takeDmg(Marian.Damage * 2, enemy.transform.position - transform.position);
                if(Marian.LifeSteal)
                {
                    if(Marian.currHp + Marian.Damage / 2 <= Marian.HP)
                    {
                        Marian.currHp += Marian.Damage / 2;
                    }
                    else
                    {
                        Marian.currHp = Marian.HP;
                    }
                }
            }
            else
            {
                enemy.takeDmg(Marian.Damage, enemy.transform.position - transform.position);
                if (Marian.LifeSteal)
                {
                    if (Marian.currHp + Marian.Damage / 2 <= Marian.HP)
                    {
                        Marian.currHp += Marian.Damage / 2;
                    }
                    else
                    {
                        Marian.currHp = Marian.HP;
                    }
                }
            }
        }
    }
}
