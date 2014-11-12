using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AoEDmg : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        List<Enemy> dmgRecipients = new List<Enemy>();
        foreach (Enemy enemy in Enemy.enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 5)
                dmgRecipients.Add(enemy);
        }
        foreach (Enemy enemy in dmgRecipients)
        {
            enemy.takeDmg(50 * Time.deltaTime);
        }
	}
}
