using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {
    AudioSource calm1, calm2, tense1, tense2, tense3, action1, action2;
    public static float hype = 0;
    public float hypometer;
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
        hypometer = hype;
        if (hype > 350)
            hype = 350;
        else if (hype > 0)
        {
            hype *= (1f - Time.deltaTime / 7);
            hype -= Time.deltaTime * 10;
        }
        if (hype < 10)
        {
            mute(calm1);
            turnOn(calm2);
            mute(tense1);
            mute(tense2);
            mute(tense3);
            mute(action1);
            mute(action2);
        }
        else if (hype < 50)
        {
            turnOn(calm1);
            mute(calm2);
            mute(tense1);
            mute(tense2);
            mute(tense3);
            mute(action1);
            mute(action2);
        }
        else if (hype < 100)
        {
            mute(calm1);
            mute(calm2);
            turnOn(tense1);
            mute(tense2);
            mute(tense3);
            mute(action1);
            mute(action2);
        }
        else if (hype < 150)
        {
            mute(calm1);
            mute(calm2);
            mute(tense1);
            turnOn(tense2);
            mute(tense3);
            mute(action1);
            mute(action2);
        }
        else if (hype < 200)
        {
            mute(calm1);
            mute(calm2);
            mute(tense1);
            mute(tense2);
            turnOn(tense3);
            mute(action1);
            mute(action2);
        }
        else if (hype < 250)
        {
            mute(calm1);
            mute(calm2);
            mute(tense1);
            mute(tense2);
            mute(tense3);
            turnOn(action1);
            mute(action2);
        }
        else
        {
            mute(calm1);
            mute(calm2);
            mute(tense1);
            mute(tense2);
            mute(tense3);
            mute(action1);
            turnOn(action2);
        }
    }
    void muteAll()
    {
        mute(calm1);
        mute(calm2);
        mute(tense1);
        mute(tense2);
        mute(tense3);
        mute(action1);
        mute(action2);
    }
    void mute(AudioSource a)
    {
        if (a.mute == false)
            a.volume -= Time.deltaTime / 5;
        if (a.volume < 0)
            a.mute = true;
    }
    void turnOn(AudioSource a)
    {
        a.mute = false;
        a.volume += Time.deltaTime / 5;
        if (a.volume > 0.5f)
            a.volume = 0.5f;
    }
}
