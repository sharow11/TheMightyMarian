using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour {

    public List<Enemy> enemies = new List<Enemy>();
	// Use this for initialization
	void Start () {
        object[] enemiesArray = FindObjectsOfTypeAll(typeof(Enemy));
        foreach (object thisEnemy in enemiesArray)
            enemies.Add((Enemy)thisEnemy);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
