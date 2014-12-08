using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

    public bool disabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (disabled)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Marian"))
        {
            GameManager gm = (GameManager)FindObjectOfType(typeof(GameManager));
            gm.levelUp();
        }
     }
}
