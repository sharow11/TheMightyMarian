using UnityEngine;
using System.Collections;

public class SuddenDeath : MonoBehaviour {
    public GameObject sd_prefab;
    GameObject sd;
    public float timeToStart = 60;
    bool started = false;
    float startTime;

	// Use this for initialization
	void Start () 
    {
        startTime = Time.time;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            started = false;
            timeToStart += 30;
            Destroy(sd);
        }
        if (!started && Time.time - startTime > timeToStart)
        {
            started = true;
            sd = (GameObject)Instantiate(sd_prefab, new Vector3(), new Quaternion());
            sd.transform.LookAt(new Vector3(0, 1, 0));
        }
        else if (started)
        {
            sd.particleSystem.startColor = new Color(sd.particleSystem.startColor.r, sd.particleSystem.startColor.g, sd.particleSystem.startColor.b, (Time.time - startTime - timeToStart) / 30);
            Marian.currHp -= (Time.time - startTime - timeToStart) * Time.deltaTime / 5;
        }
    }
}
