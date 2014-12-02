using UnityEngine;
using System.Collections;

public class Move3D2 : MonoBehaviour
{
    float rotX = 0f;
    float rotY = 0f;
    float mouseSensitivity = 3f;
    GameObject head;
    Camera cam;
    bool isTouchingGround = false;
    public bool jumpPressed = false;
    Rigidbody rb;
    public float jumpSpeed = 8.0f;
    public float moveSpeed = 7.0f;
    public float playerFriction = 0f;
    public float friction = 6;
    public float runDeacceleration = 10f;
    public float runAcceleration = 14f;
    public float airAcceleration = 2f;
    public float gravity = 20f;
    public float airDeacceleration = 2f;
    public float sideStrafeSpeed = 1f;
    public float sideStrafeAcceleration = 50f;
    public float airControl = 0.3f;
    Vector2 input;
    Vector3 moveDirectionNorm;
	// Use this for initialization
    void Start()
    {
        head = GameObject.Find("Head");
        cam = FindObjectOfType<Camera>();
        rb = transform.rigidbody;
	}
	
	// Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
        rotX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY += Input.GetAxis("Mouse X") * mouseSensitivity;
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;
        QueueJump();
        if (isTouchingGround)
            GroundMove();
        else
            AirMove();
        transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates head
        cam.transform.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera
	}
    void OnTriggerEnter(Collider other)
    {
        isTouchingGround = true;
        //audio.PlayOneShot(s_drop);
    }
    void OnTriggerStay(Collider other)
    {
        isTouchingGround = true;
    }
    void OnTriggerExit(Collider other)
    {
        isTouchingGround = false;
    }

    void QueueJump()
    {
        if (Input.GetButtonDown("Jump") && !jumpPressed)
        {
            jumpPressed = true;
            Debug.Log("pressed");
        } if (Input.GetButtonUp("Jump"))
            jumpPressed = false;
    }
    void GroundMove()
    {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if (!jumpPressed)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0f);

        float scale = CmdScale();

        SetMovementDir();

        wishdir = new Vector3(input.x, 0, input.y);
        wishdir = transform.TransformDirection(wishdir);
        wishdir = wishdir.normalized;
        moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (jumpPressed)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            jumpPressed = false;
            Debug.Log("jump");
            //PlayJumpSound();
        }
    }
    void ApplyFriction(float t)
    {
        Vector3 vec = rb.velocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec = new Vector3(vec.x, 0f, vec.z);
        speed = vec.magnitude;
        drop = 0f;

        /* Only if the player is on the ground then apply friction */
        if (isTouchingGround)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        rb.velocity = new Vector3(rb.velocity.x * newspeed, rb.velocity.y, rb.velocity.z);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z * newspeed);
    }
    float CmdScale()
    {
        int max;
        float total;
        float scale;

        max = (int)Mathf.Abs(input.y);
        if (Mathf.Abs(input.x) > max)
            max = (int)Mathf.Abs(input.x);
        if (max == 0)
            return 0;

        total = Mathf.Sqrt(input.y * input.y + input.x * input.x);
        scale = moveSpeed * max / total;

        return scale;
    }
    void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(rb.velocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        rb.velocity = new Vector3(rb.velocity.x + accelspeed * wishdir.x, rb.velocity.y, rb.velocity.z);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z + accelspeed * wishdir.z);
    }
    void SetMovementDir()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    void AirMove()
{
	Vector3 wishdir;
	float wishvel = airAcceleration;
	float accel;

	float scale = CmdScale();

	SetMovementDir();
	
	wishdir = new Vector3(input.x, 0, input.y);
	wishdir = transform.TransformDirection(wishdir);

	float wishspeed = wishdir.magnitude;
	wishspeed *= moveSpeed;
	
	wishdir = wishdir.normalized;
	moveDirectionNorm = wishdir;
	wishspeed *= scale;

	// CPM: Aircontrol
	float wishspeed2 = wishspeed;
    if (Vector3.Dot(rb.velocity, wishdir) < 0)
		accel = airDeacceleration;
	else
		accel = airAcceleration;
	// If the player is ONLY strafing left or right
    if (input.y == 0 && input.x != 0)
	{
		if(wishspeed > sideStrafeSpeed)
			wishspeed = sideStrafeSpeed;
		accel = sideStrafeAcceleration;
	}

	Accelerate(wishdir, wishspeed, accel);
	if(airControl != 0f)
		AirControl(wishdir, wishspeed2);
	// !CPM: Aircontrol

	// Apply gravity
    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.deltaTime, rb.velocity.z);

	// LEGACY MOVEMENT SEE BOTTOM
    }
    void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (input.y == 0 || wishspeed == 0)
            return;

        zspeed = rb.velocity.y;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = rb.velocity.magnitude;
        rb.velocity = rb.velocity.normalized;

        dot = Vector3.Dot(rb.velocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x * speed + wishdir.x * k, rb.velocity.y * speed + wishdir.y * k, rb.velocity.z * speed + wishdir.z * k);

            rb.velocity = rb.velocity.normalized;
            moveDirectionNorm = rb.velocity;
        }
        rb.velocity = new Vector3(rb.velocity.x * speed, zspeed, rb.velocity.z * speed);
    }
}
