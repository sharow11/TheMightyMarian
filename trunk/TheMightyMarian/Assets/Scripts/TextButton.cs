using UnityEngine;
using System.Collections;

public class TextButton : MonoBehaviour {

    public bool IsExit = false;

    void OnMauseEnter()
    {
        renderer.material.color = Color.gray;

    }

    void OnMauseExit()
    {
        renderer.material.color = Color.black;
    }

    void OnMouseUp()
    {
        if(IsExit)
        {
            Application.Quit();
        }
        else
        {
            Application.LoadLevel(1);
        }
    }
}
