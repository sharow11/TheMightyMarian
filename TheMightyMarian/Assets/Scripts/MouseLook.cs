using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    GameObject marian;
    public GameObject rocket;
	public float sensitivityX = 5f;
	public float sensitivityY = 5f;

	public float minimumX = -360f;
	public float maximumX = 360f;

	public float minimumY = -90f;
	public float maximumY = 90f;
    GameObject head;
	float rotationY = 0F;
    public float projSpeed = 20f;
    public float range = 1.5f;
    public float pushMultiplayer = 10f;
    public float fireFreq = 0.5f;
    float lastShotTime = 0f;
    List<float> rTimes = new List<float>();
    List<Vector3> rPoints = new List<Vector3>();

    void Update()
    {
        while (rTimes.Count!=0 && rTimes[0] < Time.time)
        {
            float dist = Vector3.Distance(marian.transform.position, rPoints[0]);
            Debug.Log("Boom " + dist);
            if (dist < range)
            {
                marian.GetComponent<Rigidbody>().velocity = marian.GetComponent<Rigidbody>().velocity + (marian.transform.position - rPoints[0]).normalized * (range - dist) * pushMultiplayer;
            }
            rTimes.RemoveAt(0);
            rPoints.RemoveAt(0);
        }
        if (Input.GetKeyDown("escape"))
            Screen.lockCursor = false;
        float rotationX = head.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        head.transform.localEulerAngles = new Vector3(0, rotationX, 0);
        if (Input.GetButtonDown("Fire1") && Time.time - lastShotTime > fireFreq)
        {
            lastShotTime = Time.time;
            RaycastHit hitInfo;
            Vector3 origin = transform.position + transform.right / 2 - transform.up / 3;
            GameObject shot;
            shot = (GameObject)Instantiate(rocket, origin, new Quaternion());
            LayerMask mask = ~LayerMask.GetMask("Marian");
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, mask))
            {
                shot.transform.LookAt(hitInfo.point);
                rTimes.Add(Time.time + Vector3.Distance(origin, hitInfo.point) / projSpeed);
                rPoints.Add(hitInfo.point);
            }
            else //go to space
            {
                shot.transform.LookAt(origin + transform.forward);
            }
            Destroy(shot, 10f);
        }
    }
	
	void Start ()
    {
        marian = GameObject.Find("Marian");
        Screen.lockCursor = true;
        head = GameObject.Find("Head");
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}