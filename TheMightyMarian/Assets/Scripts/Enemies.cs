using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour {

	void Start () {
        if (Enemy.enemies == null)
        {
            Enemy.enemies = new List<Enemy>();
        }
        object[] enemiesArray = FindObjectsOfTypeAll(typeof(Enemy));
        foreach (object thisEnemy in enemiesArray)
            Enemy.enemies.Add((Enemy)thisEnemy);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
