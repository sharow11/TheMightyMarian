using UnityEngine;
using System.Collections;

public class Marian : MonoBehaviour {

    public static int HP = 1000;
    public static bool IsMarianDead = false;
    public static int Mana = 1000000;
    public static int Exp = 0;
    public static int Level = 1;

    public static int currHp = HP;
    public static int currMana = Mana;

    private static float manaRegen = 1.0f;

    public static Attack.Spell currSpell = Attack.Spell.BlueBolt;

    public enum AttackType
    {
        Melee,
        Ranged,
        Spell
    }

    public static AttackType currAttackType = AttackType.Melee;

	// Use this for initialization
	void Start () {
        if (IsMarianDead)
        {
            HP = 1000;
            currHp = HP;
            IsMarianDead = false;
            Mana = 100000;
            Exp = 0;
            Level = 1;
            currMana = Mana;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if(currHp <= 0)
        {
            IsMarianDead = true;
            Application.LoadLevel(2);
        }
        if(Exp == 100 * Level)
        {
            Exp = 0;
            Level++;
            HP += 20;
            currHp += 20;
            Mana += 20;
            currMana += 20;
        }
        if(currMana < Mana)
        {
            manaRegen -= Time.deltaTime;
            if(manaRegen < 0)
            {
                currMana++;
                manaRegen = 1.0f;
            }
        }
	}
}
