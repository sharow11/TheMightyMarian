using UnityEngine;
using System.Collections;

public class OnMarianCamera : MonoBehaviour {
    public float camDistance = 15;
    public float camSpeed = 3;
    public float camYoffsetMultiplayer = 1.2f;
    public float camCurrPosImportanceMult = 5;
    public RaycastHit hit;
    GameObject marian;
    Texture2D cursorTexture;
	CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;
    Ray ray;
	void OnMouseEnter () {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
    void OnMouseExit()
    {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
	// Use this for initialization
	void Start () {
	    marian = GameObject.Find("Marian");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        ray = transform.camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("Terrain"));
        Debug.DrawLine(marian.transform.position, hit.point, Color.magenta);
        float centerX = marian.transform.position.x + (hit.point.x - marian.transform.position.x) / camCurrPosImportanceMult;
        float centerY = marian.transform.position.y + (hit.point.y - marian.transform.position.y) / camCurrPosImportanceMult;
        float cameraX = (transform.position.x + centerX * Time.deltaTime * camSpeed) / (1 + Time.deltaTime * camSpeed);
        float cameraY = ((transform.position.y + camDistance * camYoffsetMultiplayer) + centerY * Time.deltaTime * camSpeed) / (1 + Time.deltaTime * camSpeed);
        transform.position = new Vector3(cameraX, cameraY - camDistance * camYoffsetMultiplayer, marian.transform.position.z - camDistance);
        //transform.position = new Vector3(marian.transform.position.x, marian.transform.position.y - 10, marian.transform.position.z - 10);
	}
}
