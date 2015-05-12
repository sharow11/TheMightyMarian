using UnityEngine;
using System.Collections;

public class TextButton : MonoBehaviour {

    public bool IsExit = false;

    void OnMauseEnter()
    {
        GetComponent<Renderer>().material.color = Color.gray;

    }

    void OnMauseExit()
    {
        GetComponent<Renderer>().material.color = Color.black;
    }

    void OnMouseUp()
    {
        if(IsExit)
        {
            Application.Quit();
        }
        else
        {
            Application.LoadLevel(3);
        }
    }
}
