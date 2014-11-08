using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public string Name { get; set; }

    public Rarity ItemRarity { get; set; }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
