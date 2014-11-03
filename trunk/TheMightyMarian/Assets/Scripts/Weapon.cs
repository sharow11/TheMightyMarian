using UnityEngine;
using System.Collections;

public class Weapon : Item {

    public int Damage { get; set; }

    public enum Type
    {
        Sword,
        Axe,
        Bow,
        Wand
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
