using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour 
{
     

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2 - 200 , Screen.height - 100, 400, 40), "");
        if (GUI.Button(new Rect(Screen.width / 2 - 180, Screen.height - 90, 45, 25), "Fire") || Input.GetKeyDown("1"))
        {
            Attack.currSpell = Attack.Spell.FireBolt;
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height - 90, 45, 25), "Rail") || Input.GetKeyDown("2"))
        {
            Attack.currSpell = Attack.Spell.Rail;
        }
        if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 90, 45, 25), "Bolt") || Input.GetKeyDown("3"))
        {
            Attack.currSpell = Attack.Spell.LightningBolt;
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 90, Screen.height - 90, 45, 25), "Light") || Input.GetKeyDown("4"))
        {
            Attack.currSpell = Attack.Spell.Light;
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 180, Screen.height - 90, 45, 25), "Blue") || Input.GetKeyDown("5"))
        {
            Attack.currSpell = Attack.Spell.BlueBolt;
        }
    }

    void WindowFunction(int windowID)
    {
        
    }
}
