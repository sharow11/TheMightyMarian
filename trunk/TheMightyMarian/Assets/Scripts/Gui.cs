using UnityEngine;
using System.Collections;
using System;

public class Gui : MonoBehaviour 
{
    private Rect WindowRect = new Rect(Screen.width / 2 - 185, Screen.height - 120, 370, 110);

    private Rect SkillWindowRect = new Rect(Screen.width / 2, Screen.height / 2, 400, 400);

    public Texture Spell1;
    public Texture Spell2;
    public Texture Spell3;
    public Texture Spell4;
    public Texture Spell5;

    public bool showSkills = false;

    private static readonly float spellCd1 = 0.0f;
    private static readonly float spellCd2 = 0.0f;
    private static readonly float spellCd3 = 0.0f;
    private static readonly float spellCd4 = 0.0f;
    private static readonly float spellCd5 = 0.0f;

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
        var color = GUI.color;
        GUI.color = Color.red;
        GUI.Label(new Rect(110, 30, 60, 100), Marian.currHp.ToString());
        GUI.color = Color.blue;
        GUI.Label(new Rect(110, 60, 60, 100), Marian.currMana.ToString());
        GUI.Label(new Rect(140, 60, 60, 100), Marian.Mana.ToString());
        GUI.color = Color.cyan;
        GUI.Label(new Rect(110, 90, 60, 100), "EXP: " + Marian.Exp.ToString());
        GUI.color = color;
        if (Input.GetKeyDown("1") && !cd1Start)
        {
            cd1Start = true;
        }
        if (Input.GetKeyDown("2") && !cd2Start)
        {
            cd2Start = true;
        }
        if (Input.GetKeyDown("3") && !cd3Start)
        {
            Marian.currSpell = Attack.Spell.LightningBolt;
            cd3Start = true;
        }
        if (Input.GetKeyDown("4") && !cd4Start)
        {
            cd4Start = true;
        }
        if (Input.GetKeyDown("5") && !cd5Start)
        {
            Marian.currSpell = Attack.Spell.BlueBolt;
            cd5Start = true;
        }
        if (Input.GetKeyDown("k"))
        {
            showSkills = true;
        }
        if (showSkills)
        {
            SkillWindowRect = GUI.Window(1, SkillWindowRect, SkillWindowFunction, "Abilities");
        }
    }

    void Update()
    {
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

    void WindowFunction(int windowID)
    {
        var color = GUI.color;
        var cdColor = new Color(color.r, color.g, color.b, 0.25f);
        if (!cd1Start)
        {
            GUI.DrawTexture(new Rect(10, 10, 60, 100), Spell1, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.color = cdColor;
            GUI.DrawTexture(new Rect(10, 10, 60, 100), Spell1, ScaleMode.ScaleToFit, true);
            GUI.color = Color.red;
            GUI.Label(new Rect(40, 30, 60, 100), Convert.ToInt32(cd1).ToString());
            GUI.color = color;
        }
        if (!cd2Start)
        {
            GUI.DrawTexture(new Rect(80, 10, 60, 100), Spell2, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.color = cdColor;
            GUI.DrawTexture(new Rect(80, 10, 60, 100), Spell2, ScaleMode.ScaleToFit, true);
            GUI.color = Color.red;
            GUI.Label(new Rect(110, 30, 60, 100), Convert.ToInt32(cd2).ToString());
            GUI.color = color;
        }
        if (!cd3Start)
        {

            GUI.DrawTexture(new Rect(150, 10, 60, 100), Spell3, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.color = cdColor;
            GUI.DrawTexture(new Rect(150, 10, 60, 100), Spell3, ScaleMode.ScaleToFit, true);
            GUI.color = Color.red;
            GUI.Label(new Rect(180, 30, 60, 100), Convert.ToInt32(cd3).ToString());
            GUI.color = color;
        }
        if (!cd4Start)
        {
            GUI.DrawTexture(new Rect(220, 10, 60, 100), Spell4, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.color = cdColor;
            GUI.DrawTexture(new Rect(220, 10, 60, 100), Spell4, ScaleMode.ScaleToFit, true);
            GUI.color = Color.red;
            GUI.Label(new Rect(250, 30, 60, 100), Convert.ToInt32(cd4).ToString());
            GUI.color = color;
        }
        if (!cd5Start)
        {
            GUI.DrawTexture(new Rect(300, 10, 60, 100), Spell5, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.color = cdColor;
            GUI.DrawTexture(new Rect(300, 10, 60, 100), Spell5, ScaleMode.ScaleToFit, true);
            GUI.color = Color.red;
            GUI.Label(new Rect(330, 30, 60, 100), Convert.ToInt32(cd5).ToString());
            GUI.color = color;
        }
    }
    void SkillWindowFunction(int windowID)
    {
        GUI.Label(new Rect(0, 0, 60, 100), "Skills");
        if (GUI.Button(new Rect(30, 30, 60, 45), "X"))
        {
            showSkills = false;
        }
    }
}
