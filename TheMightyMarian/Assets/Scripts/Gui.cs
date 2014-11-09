using UnityEngine;
using System.Collections;
using System;

public class Gui : MonoBehaviour 
{
    private Rect WindowRect = new Rect(Screen.width / 2 - 185, Screen.height - 120, 370, 110);
    public Texture Spell1;
    public Texture Spell2;
    public Texture Spell3;
    public Texture Spell4;
    public Texture Spell5;

    private static readonly float spellCd1 = 10.0f;
    private static readonly float spellCd2 = 5.0f;
    private static readonly float spellCd3 = 20.0f;
    private static readonly float spellCd4 = 12.0f;
    private static readonly float spellCd5 = 2.0f;

    private bool cd1Start = false;
    private bool cd2Start = false;
    private bool cd3Start = false;
    private bool cd4Start = false;
    private bool cd5Start = false;

    private static float cd1 = 10.0f;
    private static float cd2 = 5.0f;
    private static float cd3 = 20.0f;
    private static float cd4 = 12.0f;
    private static float cd5 = 2.0f;

    void OnGUI()
    {
        WindowRect = GUI.Window(0, WindowRect, WindowFunction, "Skills");
        if (Input.GetKeyDown("1") && !cd1Start)
        {
            Attack.fireFireBolt = true;
            cd1Start = true;
        }
        if (Input.GetKeyDown("2") && !cd2Start)
        {
            Attack.fireRail = true;
            cd2Start = true;
        }
        if (Input.GetKeyDown("3") && !cd3Start)
        {
            Attack.currSpell = Attack.Spell.LightningBolt;
            Attack.fireLightningBolt = true;
            cd3Start = true;
        }
        if (Input.GetKeyDown("4") && !cd4Start)
        {
            Attack.fireLight = true;
            cd4Start = true;
        }
        if (Input.GetKeyDown("5") && !cd5Start)
        {
            Attack.currSpell = Attack.Spell.BlueBolt;
            Attack.fireBlueBolt = true;
            cd5Start = true;
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
}
