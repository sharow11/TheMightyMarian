using UnityEngine;
using System.Collections;

public class Marian : MonoBehaviour {

    public GameObject LevelUp;
    public static int HP = 1000;
    public static bool IsMarianDead = false;
    public static int Mana = 200;
    public static int Exp = 0;
    public static int Level = 1;

    public static Attack.Spell Spell1 = Attack.Spell.None;
    public static Attack.Spell spell2 = Attack.Spell.None;
    public static Attack.Spell spell3 = Attack.Spell.None;
    public static Attack.Spell spell4 = Attack.Spell.None;
    public static Attack.Spell spell5 = Attack.Spell.None;

    public static bool GoFast = false;
    private static float goFastTime = 2.0f;

    public static int Strenght = 0;
    public static int Dexterity = 0;
    public static int Inteligence = 0;
    public static int SkillPoints = 0;
    public static int Speed = 8;

    public static bool LifeSteal = false;
    private static float lifeStealTime = 2.0f;

    public static bool Lightning = false;
    private static float lightningTime = 2.0f;

    public static int BaseDamage = 10;

    public static int Damage = 10;

    public static float currHp = HP;
    public static int currMana = Mana;

    public static bool Empower = false;
    private static float empowerTime = 2.0f;

    private static float manaRegen = 1.0f;
    private static float maxManaRegen = 1.0f;

    private GameObject inventory;

    public static Attack.Spell currSpell = Attack.Spell.BlueBolt;
    GameObject marianObj;
    public enum AttackType
    {
        Melee,
        Ranged,
        Spell
    }

    public static AttackType currAttackType = AttackType.Melee;

	// Use this for initialization
    void Start()
    {
        if (IsMarianDead)
        {
            HP = 1000;
            currHp = HP;
            IsMarianDead = false;
            Mana = 200;
            Exp = 0;
            Level = 1;
            GameManager.currLevel = 0;
            currMana = Mana;
            Strenght = 0;
            Dexterity = 0;
            Inteligence = 0;
            BaseDamage = 10;
            Damage = 10;
            manaRegen = 1.0f;
            maxManaRegen = 1.0f;
            SkillPoints = 0;
            Spell1 = Attack.Spell.None;
            spell2 = Attack.Spell.None;
            spell3 = Attack.Spell.None;
            spell4 = Attack.Spell.None;
            spell5 = Attack.Spell.None;

        }
    }
	
	// Update is called once per frame
	void Update () {

        inventory = GameObject.Find("Inventory");

        var script = inventory.GetComponent<Character>() as Character;

        int type = (int)script.Type;

        if ((int)script.Damage < 10)
        {
            BaseDamage = 10;
        }
        else
        {
            BaseDamage = (int)script.Damage;
        }

        currAttackType = (AttackType)type;

        if(currAttackType == AttackType.Melee)
        {
            Damage = BaseDamage + Strenght;
        }
        else if (currAttackType == AttackType.Ranged)
        {
            Damage = BaseDamage + Dexterity;
        }
        else
        {
            Damage = BaseDamage + Inteligence;
        }

        if (LifeSteal)
        {
            lifeStealTime -= Time.deltaTime;
            if (lifeStealTime < 0)
            {
                LifeSteal = false;
                lifeStealTime = 2.0f;
            }
        }

        if (GoFast)
        {
            goFastTime -= Time.deltaTime;
            if (goFastTime < 0)
            {
                GoFast = false;
                goFastTime = 2.0f;
                Speed -= 8;
            }
        }

        if (Empower)
        {
            empowerTime -= Time.deltaTime;
            if (empowerTime < 0)
            {
                Empower = false;
                empowerTime = 2.0f;
            }
        }

        if (Lightning)
        {
            lightningTime -= Time.deltaTime;
            if (lightningTime < 0)
            {
                Lightning = false;
                lightningTime = 2.0f;
            }
        }


	    if(currHp <= 0)
        {
            IsMarianDead = true;
            Application.LoadLevel(2);
        }
        if(Exp >= 100 * Level)
        {
            marianObj = GameObject.Find("Marian");
            Destroy((GameObject)Instantiate(LevelUp, new Vector3(marianObj.transform.position.x, marianObj.transform.position.y + 0.5f, -1.5f), new Quaternion()), 10f);
            Exp -= 100 * Level;
            Level++;
            HP += 20;
            currHp += 20;
            Mana += 20;
            currMana += 20;
            SkillPoints += 5;

        }
        if(currMana < Mana)
        {
            manaRegen -= Time.deltaTime;
            if(manaRegen < 0)
            {
                currMana++;
                manaRegen = maxManaRegen;
            }
        }
	}

    public static void AddSkillPoints(string type)
    {
        if(type == "str")
        {
            Strenght++;
            HP += 5;
            currHp += 5;
            if(Strenght == 10)
            {
                AddSpell(Attack.Spell.Empower);
            }
            if (Strenght == 20)
            {
                AddSpell(Attack.Spell.Rage);
            }
            if (Strenght == 30)
            {
                AddSpell(Attack.Spell.Shout);
            }
        }
        else if(type == "dex")
        {
            Dexterity++;
            if (Dexterity == 10)
            {
                AddSpell(Attack.Spell.Fast);
                Speed++;
            }
            if (Dexterity == 20)
            {
                AddSpell(Attack.Spell.Heal);
                Speed++;
            }
            if (Dexterity == 30)
            {
                AddSpell(Attack.Spell.Rail);
                Speed++;
            }
        }
        else
        {
            Inteligence++;
            Mana += 5;
            currMana += 5;
            maxManaRegen -= 0.01f;
            if (Inteligence == 10)
            {
                AddSpell(Attack.Spell.Eruption);
            }
            if (Inteligence == 20)
            {
                AddSpell(Attack.Spell.LightningBolt);
            }
            if (Inteligence == 30)
            {
                AddSpell(Attack.Spell.FireBolt);
            }
        }
        SkillPoints--;
    }
    
    private static void AddSpell(Attack.Spell spell)
    {
        if(Spell1 == Attack.Spell.None)
        {
            Spell1= spell;
        }
        else if (spell2 == Attack.Spell.None)
        {
            spell2 = spell;
        }
        else if (spell3 == Attack.Spell.None)
        {
            spell3 = spell;
        }
        else if (spell4 == Attack.Spell.None)
        {
            spell4 = spell;
        }
        else if (spell5 == Attack.Spell.None)
        {
            spell5 = spell;
        }
    }
}
