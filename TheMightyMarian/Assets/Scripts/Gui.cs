using UnityEngine;
using System.Collections;
using System;

public class Gui : MonoBehaviour 
{
    private Rect WindowRect = new Rect(Screen.width / 2 - 185, Screen.height - 120, 370, 110);

    private Rect SkillWindowRect = new Rect(Screen.width / 2, Screen.height / 2, 400, 400);

    private Rect HelpWindowRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 300, 300);

    public Texture Spell1;
    public Texture Spell2;
    public Texture Spell3;
    public Texture Spell4;
    public Texture Spell5;
    public enum Spell : byte { None, BlueBolt, LightningBolt, Rail, FireBolt, Light, Heal, Rage, Fast, Empower, Shout, Eruption };

    public Texture None;
    public Texture BlueBolt;
    public Texture LightningBolt;
    public Texture Rail;
    public Texture FireBolt;
    public Texture Light;
    public Texture Heal;
    public Texture Rage;
    public Texture Fast;
    public Texture Empower;
    public Texture Shout;
    public Texture Eruption;

    public bool showSkills = false;
    private bool showHelp = false;

    private static readonly float spellCd1 = 3.0f;
    private static readonly float spellCd2 = 3.0f;
    private static readonly float spellCd3 = 3.0f;
    private static readonly float spellCd4 = 3.0f;
    private static readonly float spellCd5 = 3.0f;

    public static bool cd1Start = false;
    public static bool cd2Start = false;
    public static bool cd3Start = false;
    public static bool cd4Start = false;
    public static bool cd5Start = false;

    private static float cd1 = spellCd1;
    private static float cd2 = spellCd2;
    private static float cd3 = spellCd3;
    private static float cd4 = spellCd4;
    private static float cd5 = spellCd5;

    void OnGUI()
    {
        WindowRect = GUI.Window(0, WindowRect, WindowFunction, "Skills");
        GUI.Label(new Rect(30, 30, 60, 100), "Press H for controls");
        var color = GUI.color;
        GUI.color = Color.red;
        GUI.Label(new Rect(110, 30, 60, 100), Marian.currHp.ToString());
        GUI.color = Color.blue;
        GUI.Label(new Rect(110, 60, 60, 100), Marian.currMana.ToString());
        GUI.Label(new Rect(140, 60, 60, 100), Marian.Mana.ToString());
        GUI.color = Color.cyan;
        GUI.Label(new Rect(110, 90, 60, 100), "EXP: " + Marian.Exp.ToString());
        GUI.color = Color.yellow;
        GUI.Label(new Rect(110, 120, 60, 100), "Damage" + Marian.Damage.ToString());
        GUI.color = color;
        if (Input.GetKeyDown("k"))
        {
            showSkills = true;
        }
        if (showSkills)
        {
            SkillWindowRect = GUI.Window(1, SkillWindowRect, SkillWindowFunction, "Abilities");
        }
        if (Input.GetKeyDown("h"))
        {
            showHelp = true;
        }
        if(showHelp)
        {
            HelpWindowRect = GUI.Window(2, HelpWindowRect, HelpWindowFunction, "Help");
        }
    }

    void Update()
    {
        SetSpellTextures();
        if(cd1Start)
        {
            cd1 -= Time.deltaTime;
            if(cd1 < 0)
            {
                cd1Start = false;
                cd1 = spellCd1;
            }
        }
        if (cd2Start)
        {
            cd2 -= Time.deltaTime;
            if (cd2 < 0)
            {
                cd2Start = false;
                cd2 = spellCd2;
            }
        }
        if (cd3Start)
        {
            cd3 -= Time.deltaTime;
            if (cd3 < 0)
            {
                cd3Start = false;
                cd3 = spellCd3;
            }
        }
        if (cd4Start)
        {
            cd4 -= Time.deltaTime;
            if (cd4 < 0)
            {
                cd4Start = false;
                cd4 = spellCd4;
            }
        }
        if (cd5Start)
        {
            cd5 -= Time.deltaTime;
            if (cd5 < 0)
            {
                cd5Start = false;
                cd5 = spellCd5;
            }
        }
    }

    void SetSpellTextures()
    {
        switch (Marian.Spell1)
        {
            case Attack.Spell.None:
                Spell1 = None;
                break;
            case Attack.Spell.BlueBolt:
                Spell1 = BlueBolt;
                break;
            case Attack.Spell.LightningBolt:
                Spell1 = LightningBolt;
                break;
            case Attack.Spell.Rail:
                Spell1 = Rail;
                break;
            case Attack.Spell.FireBolt:
                Spell1 = FireBolt;
                break;
            case Attack.Spell.Light:
                Spell1 = Light;
                break;
            case Attack.Spell.Heal:
                Spell1 = Heal;
                break;
            case Attack.Spell.Rage:
                Spell1 = Rage;
                break;
            case Attack.Spell.Fast:
                Spell1 = Fast;
                break;
            case Attack.Spell.Empower:
                Spell1 = Empower;
                break;
            case Attack.Spell.Shout:
                Spell1 = Shout;
                break;
            case Attack.Spell.Eruption:
                Spell1 = Eruption;
                break;
            default:
                break;
        }
        switch (Marian.spell2)
        {
            case Attack.Spell.None:
                Spell2 = None;
                break;
            case Attack.Spell.BlueBolt:
                Spell2 = BlueBolt;
                break;
            case Attack.Spell.LightningBolt:
                Spell2 = LightningBolt;
                break;
            case Attack.Spell.Rail:
                Spell2 = Rail;
                break;
            case Attack.Spell.FireBolt:
                Spell2 = FireBolt;
                break;
            case Attack.Spell.Light:
                Spell2 = Light;
                break;
            case Attack.Spell.Heal:
                Spell2 = Heal;
                break;
            case Attack.Spell.Rage:
                Spell2 = Rage;
                break;
            case Attack.Spell.Fast:
                Spell2 = Fast;
                break;
            case Attack.Spell.Empower:
                Spell2 = Empower;
                break;
            case Attack.Spell.Shout:
                Spell2 = Shout;
                break;
            case Attack.Spell.Eruption:
                Spell2 = Eruption;
                break;
            default:
                break;
        }
        switch (Marian.spell3)
        {
            case Attack.Spell.None:
                Spell3 = None;
                break;
            case Attack.Spell.BlueBolt:
                Spell3 = BlueBolt;
                break;
            case Attack.Spell.LightningBolt:
                Spell3 = LightningBolt;
                break;
            case Attack.Spell.Rail:
                Spell3 = Rail;
                break;
            case Attack.Spell.FireBolt:
                Spell3 = FireBolt;
                break;
            case Attack.Spell.Light:
                Spell3 = Light;
                break;
            case Attack.Spell.Heal:
                Spell3 = Heal;
                break;
            case Attack.Spell.Rage:
                Spell3 = Rage;
                break;
            case Attack.Spell.Fast:
                Spell3 = Fast;
                break;
            case Attack.Spell.Empower:
                Spell3 = Empower;
                break;
            case Attack.Spell.Shout:
                Spell3 = Shout;
                break;
            case Attack.Spell.Eruption:
                Spell3 = Eruption;
                break;
            default:
                break;
        }
        switch (Marian.spell4)
        {
            case Attack.Spell.None:
                Spell4 = None;
                break;
            case Attack.Spell.BlueBolt:
                Spell4 = BlueBolt;
                break;
            case Attack.Spell.LightningBolt:
                Spell4 = LightningBolt;
                break;
            case Attack.Spell.Rail:
                Spell4 = Rail;
                break;
            case Attack.Spell.FireBolt:
                Spell4 = FireBolt;
                break;
            case Attack.Spell.Light:
                Spell4 = Light;
                break;
            case Attack.Spell.Heal:
                Spell4 = Heal;
                break;
            case Attack.Spell.Rage:
                Spell4 = Rage;
                break;
            case Attack.Spell.Fast:
                Spell4 = Fast;
                break;
            case Attack.Spell.Empower:
                Spell4 = Empower;
                break;
            case Attack.Spell.Shout:
                Spell4 = Shout;
                break;
            case Attack.Spell.Eruption:
                Spell4 = Eruption;
                break;
            default:
                break;
        }
        switch (Marian.spell5)
        {
            case Attack.Spell.None:
                Spell5 = None;
                break;
            case Attack.Spell.BlueBolt:
                Spell5 = BlueBolt;
                break;
            case Attack.Spell.LightningBolt:
                Spell5 = LightningBolt;
                break;
            case Attack.Spell.Rail:
                Spell5 = Rail;
                break;
            case Attack.Spell.FireBolt:
                Spell5 = FireBolt;
                break;
            case Attack.Spell.Light:
                Spell5 = Light;
                break;
            case Attack.Spell.Heal:
                Spell5 = Heal;
                break;
            case Attack.Spell.Rage:
                Spell5 = Rage;
                break;
            case Attack.Spell.Fast:
                Spell5 = Fast;
                break;
            case Attack.Spell.Empower:
                Spell5 = Empower;
                break;
            case Attack.Spell.Shout:
                Spell5 = Shout;
                break;
            case Attack.Spell.Eruption:
                Spell5 = Eruption;
                break;
            default:
                break;
        }
    }

    void WindowFunction(int windowID)
    {
        DrawSpell(Marian.Spell1, Spell1, new Rect(10, 10, 60, 100), new Rect(40, 30, 60, 100), cd1Start, cd1);
        DrawSpell(Marian.spell2, Spell2, new Rect(80, 10, 60, 100), new Rect(110, 30, 60, 100), cd2Start, cd2);
        DrawSpell(Marian.spell3, Spell3, new Rect(150, 10, 60, 100), new Rect(180, 30, 60, 100), cd3Start, cd3);
        DrawSpell(Marian.spell4, Spell4, new Rect(220, 10, 60, 100), new Rect(250, 30, 60, 100), cd4Start, cd4);
        DrawSpell(Marian.spell5, Spell5, new Rect(300, 10, 60, 100), new Rect(330, 30, 60, 100), cd5Start, cd5);
    }

    void DrawSpell(Attack.Spell spell,Texture spellTexture, Rect position, Rect labelPosition, bool cd, float cdTime)
    {
        var color = GUI.color;
        var cdColor = new Color(color.r, color.g, color.b, 0.25f);
        if(spell == Attack.Spell.None)
        {
            GUI.color = cdColor;
            GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
            GUI.color = color;
        }
        else if (spell == Attack.Spell.LightningBolt || spell == Attack.Spell.BlueBolt)
        {
            if (!cd)
            {
                if (Marian.currSpell == Attack.Spell.BlueBolt)
                {
                    GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
                }
                else
                {
                    GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
                }
            }
            else
            {
                if (Marian.currSpell == Attack.Spell.BlueBolt)
                {
                    GUI.color = cdColor;
                    GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
                    GUI.color = Color.red;
                    GUI.Label(labelPosition, Convert.ToInt32(cdTime).ToString());
                    GUI.color = color;
                }
                else
                {
                    GUI.color = cdColor;
                    GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
                    GUI.color = Color.red;
                    GUI.Label(labelPosition, Convert.ToInt32(cdTime).ToString());
                    GUI.color = color;
                }
            }
        }
        else
        {
            if (!cd)
            {
                GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
            }
            else
            {
                GUI.color = cdColor;
                GUI.DrawTexture(position, spellTexture, ScaleMode.ScaleToFit, true);
                GUI.color = Color.red;
                GUI.Label(labelPosition, Convert.ToInt32(cdTime).ToString());
                GUI.color = color;
            }
        }
    }
    void SkillWindowFunction(int windowID)
    {
        GUI.Label(new Rect(30, 50, 100, 100), "Strenght");
        GUI.Label(new Rect(140, 50, 100, 100), "Dexterity ");
        GUI.Label(new Rect(250, 50, 100, 100), "Inteligence");
        GUI.Label(new Rect(30, 80, 100, 100), Marian.Strenght.ToString());
        GUI.Label(new Rect(140, 80, 100, 100), Marian.Dexterity.ToString());
        GUI.Label(new Rect(250, 80, 100, 100), Marian.Inteligence.ToString());

        if(Marian.SkillPoints > 0)
        {
            if (GUI.Button(new Rect(30, 100, 20, 20), "+"))
            {
                Marian.AddSkillPoints("str");
            }
            if (GUI.Button(new Rect(140, 100, 20, 20), "+"))
            {
                Marian.AddSkillPoints("dex");
            }
            if (GUI.Button(new Rect(250, 100, 20, 20), "+"))
            {
                Marian.AddSkillPoints("int");
            }
        }


        if (GUI.Button(new Rect(355, 30, 20, 20), "X"))
        {
            showSkills = false;
        }

    }
    void HelpWindowFunction(int windowID)
    {
        GUI.Label(new Rect(30, 50, 220, 50), "To attack press LPM");
        GUI.Label(new Rect(30, 100, 220, 50), "To fire spells press 1-5");
        GUI.Label(new Rect(30, 150, 220, 50), "To set skills press K");
        GUI.Label(new Rect(30, 200, 220, 50), "To change weapon press q");
        if (GUI.Button(new Rect(255, 30, 20, 20), "X"))
        {
            showHelp = false;
        }
    }
}
