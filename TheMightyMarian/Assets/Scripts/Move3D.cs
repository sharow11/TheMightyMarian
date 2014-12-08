using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class Move3D : MonoBehaviour {
    public AudioClip s_drop, s_jump;
    Rigidbody rb;
    public Vector3 input;
    GameObject head;
    public float speed = 6f;
    public float currSpeed = 0f;
    public float acc = 50f;
    public float airCtrl = 0.1f;
    public float friction = 20f;
    public float jumpSpeed = 5f;
    public float gravity = 15f;
    public float circleJumpAcc = 8f;
    public float bonusTime = 0.5f;
    public float bonusMaxUsableTime = 0.1f;
    public float roundTime = 60f;

    float bonusVel = 2.4f;
    bool jumpPressed = false;
    bool jumping = false;
    float jumpPressTime = 0f;
    float ctrl = 1f;
    float enterTime = 0;
    bool isTouchingGround = false;
    float lastTorchZ = -3f;
    float lastTorchY = 2f;
    float mostZ = 0f;
    float onJumpVel = 0f;
    GUIText tl, sp;
    GameObject[] torches;
    float startTime = 0f;

	// Use this for initialization
	void Start () {
        rb = transform.rigidbody;
        head = GameObject.Find("Head");
        torches = GameObject.FindGameObjectsWithTag("Torch");
        tl = GameObject.Find("TimeLeft").guiText;
        sp = GameObject.Find("Speed").guiText;
        startTime = Time.time;
        bonusVel = bonusMaxUsableTime * circleJumpAcc;
	}

    void Update()
    {
        tl.text = "Time left: " + (int)(roundTime + startTime - Time.time);
        sp.text = "Speed: " + (new Vector2(rb.velocity.x, rb.velocity.z).magnitude);
        if (Time.time - startTime > roundTime)
            finish();
        input = (Input.GetAxisRaw("Vertical") * head.transform.forward + Input.GetAxisRaw("Horizontal") * head.transform.right).normalized;
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
            jumpPressTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            circleJumpAcc += 1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            circleJumpAcc -= 1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            roundTime += 30f;
        }
    }

    private void finish()
    {
        Screen.lockCursor = false;
        Marian.Exp += (int)-mostZ;
        Application.LoadLevel(1);
    }
    void FixedUpdate()
    {
        if (transform.position.z < mostZ)
            mostZ = transform.position.z;
        if (rb.velocity.y > 0.1f)
            jumping = true;
        float mag;
        Vector3 dir;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.deltaTime, rb.velocity.z); //GRAVITY
        if (isTouchingGround)
        {
            foreach (GameObject t in torches) //checkpoint
            {
                if (transform.position.z < t.transform.position.z && t.transform.position.z < lastTorchZ)
                {
                    lastTorchZ = t.transform.position.z;
                    lastTorchY = t.transform.position.y;
                }
            }
            if (jumpPressed && Time.time - jumpPressTime < 0.1f) //jumping
            {
                jump();
            }
            else if (Time.time - enterTime > 0.04f && !jumping) //touching ground and aint gonna jump - apply friction
            {
                mag = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
                dir = rb.velocity.normalized;
                float multiply = 1f;
                if (input.magnitude < 0.2f) //stronger friction if no input
                    multiply = 2f;
                float newMag = mag - friction * Time.deltaTime * multiply;
                if (mag > 0)
                    rb.velocity = new Vector3(dir.x * newMag, rb.velocity.y, dir.z * newMag);
                else
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                if (input.magnitude < 0.1f && rb.velocity.magnitude < 0.6f) //to stop the quake
                    rb.velocity = new Vector3();
            }
            ctrl = 1f;
        }
        else
            ctrl = airCtrl;
        if(jumping)
            ctrl = airCtrl;
        dir = rb.velocity.normalized;
        if (input.magnitude > 0.1f)
        {
            Vector2 newVel = new Vector2(rb.velocity.x + input.x * Time.deltaTime * acc * ctrl, rb.velocity.z + input.z * Time.deltaTime * acc * ctrl);
            if (rb.velocity.magnitude < speed && newVel.magnitude > speed)
                rb.velocity = new Vector3((newVel.normalized * speed).x, rb.velocity.y, (newVel.normalized * speed).y);
            else if (rb.velocity.magnitude < speed)
                rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.y);
            else
            {
                mag = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
                rb.velocity = new Vector3(newVel.normalized.x * mag, rb.velocity.y, newVel.normalized.y * mag);
            }
        }
        if (transform.position.y < -2) //reset
        {
            transform.position = new Vector3(9.5f, lastTorchY, lastTorchZ);
            rb.velocity = new Vector3();
        }
        circleJumpAccelerate();

        currSpeed = rb.velocity.magnitude;
	}
    void OnTriggerEnter(Collider other)
    {
        jumping = false;
        enterTime = Time.time;
        isTouchingGround = true;
        audio.PlayOneShot(s_drop);
        Debug.Log("enter: " + enterTime);
        if (jumpPressed && Time.time - jumpPressTime < 0.1f) //jump on touch;
        {
            jump();
        }
    }
    void OnTriggerStay(Collider other)
    {
        isTouchingGround = true; 
        if (rb.velocity.y < 0.1f)
            jumping = false;
    }
    void OnTriggerExit(Collider other)
    {
        isTouchingGround = false;
    }
    void jump()
    {
        jumping = true;
        Debug.Log("jump: " + Time.time);
        audio.PlayOneShot(s_jump);
        Vector2 temp = new Vector2(rb.velocity.x, rb.velocity.z);
        float mag2D = temp.magnitude;
        Vector2 dir2D = temp.normalized;
        float velY = jumpSpeed;
        if (rb.velocity.y > 0)
            velY += rb.velocity.y;
        if (input.magnitude > 0.1f && temp.magnitude < speed && temp.magnitude > speed / 2)
        {
            temp = dir2D * speed;
            rb.velocity = new Vector3(temp.x, velY, temp.y);
        }
        else
        {
            rb.velocity = new Vector3(dir2D.x * mag2D, velY, dir2D.y * mag2D);
        }
        jumpPressed = false;
        onJumpVel = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }
    void circleJumpAccelerate()
    {
        if (jumping && Time.time - jumpPressTime < bonusTime)
        {
            Vector2 temp = new Vector2(rb.velocity.x, rb.velocity.z);
            float mag2D = temp.magnitude;
            Vector2 dir2D = temp.normalized;
            if (Input.GetAxisRaw("Horizontal") * Input.GetAxis("Mouse X") > 0.1f) //circle jump
            {
                mag2D += Time.deltaTime * circleJumpAcc;
                if (mag2D > onJumpVel + bonusVel) //cap maximum strafe jump velocity gain
                {
                    mag2D = onJumpVel + bonusVel;
                }
                rb.velocity = new Vector3(dir2D.x * mag2D, rb.velocity.y, dir2D.y * mag2D);
            }
            else if (Input.GetAxisRaw("Horizontal") * Input.GetAxis("Mouse X") < -0.1f) //wrong directions, slow down
            {
                mag2D -= Time.deltaTime * circleJumpAcc;
                rb.velocity = new Vector3(dir2D.x * mag2D, rb.velocity.y, dir2D.y * mag2D);
            }
        }
    }
}
