using UnityEngine;
using System.Collections;

public class OnMarianCamera : MonoBehaviour {
    public float camDistance = 15;
    public float camSpeed = 3;
    public float camYoffsetMultiplayer = 1.2f;
    public float camCurrPosImportanceMult = 5;
    public RaycastHit hit;
    public Vector3 hitAbove;
    GameObject marian = null;

    Texture2D cursorTexture;
	CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;
    Ray ray;
    GameManager gm;
	void OnMouseEnter () {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

    void OnMouseExit()
    {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}

	// Use this for initialization
	void Start () {
        gm = FindObjectOfType(typeof(GameManager)) as GameManager;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (true || !gm.isLoading)
        {
            if (marian == null)
            { 
                marian = GameObject.Find("Marian");
                return;
            }
            ray = transform.camera.ScreenPointToRay(Input.mousePosition);
            float multiplayer = 2f / ray.direction.z;
            Vector3 offset = ray.direction * multiplayer;
            LayerMask mask = LayerMask.GetMask("Terrain");
            Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value);
            hitAbove = hit.point - offset;
            Debug.DrawLine(marian.transform.position, hit.point, Color.magenta);
            float centerX = marian.transform.position.x + (hit.point.x - marian.transform.position.x) / camCurrPosImportanceMult;
            float centerY = marian.transform.position.y + (hit.point.y - marian.transform.position.y) / camCurrPosImportanceMult;
            float cameraX = (transform.position.x + centerX * Time.deltaTime * camSpeed) / (1 + Time.deltaTime * camSpeed);
            float cameraY = ((transform.position.y + camDistance * camYoffsetMultiplayer) + centerY * Time.deltaTime * camSpeed) / (1 + Time.deltaTime * camSpeed);
            transform.position = new Vector3(cameraX, cameraY - camDistance * camYoffsetMultiplayer, marian.transform.position.z - camDistance);
        }
	}
}
