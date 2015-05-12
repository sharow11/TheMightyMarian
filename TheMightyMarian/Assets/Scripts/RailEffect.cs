using UnityEngine;
using System.Collections;

public class RailEffect : MonoBehaviour {
    float time;

	// Use this for initialization
	void Start () {
        time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(Color.white, Color.black, (Time.time - time) * 3));
        this.gameObject.transform.Find("Rail_around2").GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(Color.white, Color.black, (Time.time - time) * 3));
	}
}
