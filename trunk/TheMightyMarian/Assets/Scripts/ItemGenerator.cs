using UnityEngine;
using System.Collections;

public class ItemGenerator : MonoBehaviour {

	// Use this for initialization\
    private static int CommonChance = 69;
    private static int RareChance = 25;
    private static int EpicChance = 5;
    private static int LegendaryChance = 1;

	void Start () 
    {
        int rarity = Random.Range(1, 101);
        Weapon weapon = new Weapon();
        if(rarity <= CommonChance)
        {
            weapon.ItemRarity = Item.Rarity.Common;
        }
        else if (rarity <= CommonChance + RareChance)
        {
            weapon.ItemRarity = Item.Rarity.Rare;
        }
        else if (rarity <= CommonChance + RareChance + EpicChance)
        {
            weapon.ItemRarity = Item.Rarity.Epic;
        }
        else
        {
            weapon.ItemRarity = Item.Rarity.Legendary;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void GenerateName()
    {

    }

}
