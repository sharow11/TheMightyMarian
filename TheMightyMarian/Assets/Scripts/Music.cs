using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {
    AudioSource calm1, calm2, tense1, tense2, tense3, action1, action2;
    public static float hype = 0;
	// Use this for initialization
    void Start()
    {
        calm1 = transform.Find("Calm1").GetComponent<AudioSource>();
        calm2 = transform.Find("Calm2").GetComponent<AudioSource>();
        tense1 = transform.Find("Tense1").GetComponent<AudioSource>();
        tense2 = transform.Find("Tense2").GetComponent<AudioSource>();
        tense3 = transform.Find("Tense3").GetComponent<AudioSource>();
        action1 = transform.Find("Action1").GetComponent<AudioSource>();
        action2 = transform.Find("Action2").GetComponent<AudioSource>();
        /*calm1.mute = true;
        calm2.mute = true;
        tense1.mute = true;
        tense2.mute = true;
        tense3.mute = true;
        action1.mute = true;
        action2.mute = true;*/
	}
	
	// Update is called once per frame
    void Update()
    {
        calm2.mute = false;
        if (hype > 0)
        {
            hype -= Time.deltaTime;
        }
        if (hype < 10)
        {
            muteAll();
            calm2.mute = false;
        }
        else if (hype < 50)
        {
            muteAll();
            calm1.mute = false;
        }
        else if (hype < 100)
        {
            muteAll();
            tense1.mute = false;
        }
        else if (hype < 150)
        {
            muteAll();
            tense2.mute = false;
        }
        else if (hype < 200)
        {
            muteAll();
            tense3.mute = false;
        }
        else if (hype < 250)
        {
            muteAll();
            action1.mute = false;
        }
        else
        {
            muteAll();
            action2.mute = false;
        }
    }
    void muteAll()
    {
        calm1.mute = true;
        calm2.mute = true;
        tense1.mute = true;
        tense2.mute = true;
        tense3.mute = true;
        action1.mute = true;
        action2.mute = true;
    }
}
