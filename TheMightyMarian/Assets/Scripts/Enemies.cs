using UnityEngine;
using System.Collections;

public class Enemies : MonoBehaviour {

    public object[] enemies;
	// Use this for initialization
	void Start () {
        enemies = Resources.FindObjectsOfTypeAll(typeof(Enemy));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
