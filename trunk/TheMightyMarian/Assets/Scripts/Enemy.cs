using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public enum State : byte { idle, alert, follow, searching, chasing, attacking };
    public enum Step : byte { downRight, upRight, downLeft, upLeft };
    public State state = State.idle;
    Step step = Step.downRight;
    float stepTime = 0;
    public bool canFollowSteps;
    GameObject Marian;
    MoveMarian moveMarian;
    Enemies enemies;
    public Vector3 lastSeen, prevStep, prevPrevStep, target, patrolTarget, pushAwayFromWalls;
    //float targetTime;
    public float speed = 5;
    public float health = 100;
    float seenLastTime = 0, patrolTargetAssignTime;
    Ray ray;
    RaycastHit hit;
    Color color;
    public List<Vector3> positions;
    List<float> times;
    bool gotPatrolTarget = false;
    bool lostTrack = false;
    int nr = 0;
    // Use this for initialization
    void Start()
    {
        positions = new List<Vector3>();
        times = new List<float>();
        Marian = GameObject.Find("Marian");
        enemies = (Enemies)(GameObject.Find("General").GetComponent("Enemies"));
        moveMarian = (MoveMarian)Marian.GetComponent("MoveMarian");
        //Debug.Log(moveMarian);
        prevStep = transform.position;
        prevPrevStep = new Vector3(prevStep.x + 0.01f, prevStep.y, prevStep.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        nr = 0;
        while (times.Count > 0 && Time.time - times[0] > 0.2f)
        {
            times.RemoveAt(0);
            positions.RemoveAt(0);
        }
        if (canSeeMarian(transform.position, 20))
        {
            seenLastTime = Time.time;
            positions.Add(Marian.transform.position);
            times.Add(Time.time);
            lastSeen = Marian.transform.position;
            //lastSeen = moveMarian.positions[0];
            if (Vector3.Distance(Marian.transform.position, transform.position) < 10)
                state = State.attacking;
            else
                state = State.chasing;
            gotPatrolTarget = false;

            foreach (Enemy enemy in enemies.enemies)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < 20 && canSeeFoe(enemy.transform.position, 20, enemy) && (enemy.state == State.idle || enemy.state == State.alert))
                {
                    enemy.gotPatrolTarget = true;
                    enemy.patrolTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    enemy.patrolTargetAssignTime = Time.time;
                    enemy.lostTrack = false;
                    enemy.state = State.follow;
                }
            }
            //print("Widać Mariana!" + Vector3.Distance(Marian.transform.position, transform.position));
            //Debug.Log(hit.collider.name + ", " + hit.collider.tag);
        }
        else
        {
            if (state == State.attacking || state == State.chasing)
            {
                state = State.searching;
                //lastSeen = Marian.transform.position;
                lastSeen = positions[0];
                lastSeen.z = transform.position.z;
            }
        }
        switch (state) //TODO: odpychanie się jednych od drugich, nie spowalnianie goniac kolege.
        {
            case State.idle:
                color = Color.white;
                break;
            case State.alert:
            case State.follow:
                animateMovement();
                color = Color.black;
                if (gotPatrolTarget)
                {
                    if (Vector3.Distance(transform.position, patrolTarget) < 0.2f || (!canFollowSteps && Time.time - patrolTargetAssignTime > 6))
                    {
                        gotPatrolTarget = false;
                        state = State.alert;
                    }
                    else
                    {
                        move(patrolTarget);
                    }
                    if (Time.time - patrolTargetAssignTime > 15)
                    {
                        lostTrack = true;
                        state = State.alert;
                    }
                }
                else if (!canFollowSteps || lostTrack)
                {
                    patrolTarget = new Vector3(transform.position.x + Random.value * 15 - 7.5f, transform.position.y + Random.value * 15 - 7.5f, transform.position.z);
                    gotPatrolTarget = true;
                    int i = 0;
                    while (Physics.Raycast(transform.position, patrolTarget - transform.position, Vector3.Distance(transform.position, patrolTarget)))
                    {
                        if (i > 20)
                        {
                            gotPatrolTarget = false;
                            break;
                        }
                        patrolTarget = new Vector3(transform.position.x + Random.value * 15 - 7.5f, transform.position.z + Random.value * 15 - 7.5f, transform.position.z);
                        i++;
                    }
                    if (gotPatrolTarget)
                    {
                        move(patrolTarget);
                        patrolTargetAssignTime = Time.time;
                    }
                }
                else
                {
                    gotPatrolTarget = false;
                    int lastSeenPosIndex = moveMarian.GetIndex(seenLastTime);
                    //Debug.Log(lastSeenPosIndex);
                    int posIndex;
                    for (int i = 2; i <= 64; i*=2)
                    {
                        posIndex = ((moveMarian.positions.Count - 1) - lastSeenPosIndex) / i + lastSeenPosIndex;
                        //Debug.Log(posIndex);
                        Debug.DrawLine(transform.position, moveMarian.positions[posIndex], Color.green);
                        if (!Physics.Raycast(transform.position, moveMarian.positions[posIndex] - transform.position, Vector3.Distance(transform.position, moveMarian.positions[posIndex])))
                        {
                            patrolTarget = moveMarian.positions[posIndex];
							patrolTargetAssignTime = Time.time;
							patrolTarget.z = transform.position.z;
                            gotPatrolTarget = true;
                            break;
                        }
                    }
                    //Debug.Log(gotPatrolTarget);
                    if (!gotPatrolTarget)
                        lostTrack = true;
                }
                Debug.DrawLine(transform.position, patrolTarget, Color.cyan);
                break;
            case State.searching:
                animateMovement();
                color = Color.yellow;
                if (Time.time - seenLastTime > 6 || Vector3.Distance(transform.position, lastSeen) < 0.2f)
                {
                    lostTrack = false;
                    state = State.alert;
                }
                else
                {
                    move(lastSeen);
                }
                Debug.DrawLine(transform.position, lastSeen, color);
                //Debug.Log(Time.time - seenLastTime + " dist: " + Vector3.Distance(transform.position, lastSeen));
                break;
            case State.chasing:
                animateMovement();
                color = new Color(1, 0.5f, 0);
                move(lastSeen);
                Debug.DrawLine(transform.position, Marian.transform.position, color);
                break;
            case State.attacking:
                color = Color.red;
                Debug.DrawLine(transform.position, Marian.transform.position, color);
                break;
            default:
                color = Color.white;
                break;
        }
        //Debug.DrawLine(transform.position, lastSeen, Color.blue);
        //Debug.Log(nr + "!!!" + transform.position.ToString("F5") + " " + prevStep.ToString("F5") + " " + prevPrevStep.ToString("F5"));
        foreach (Enemy enemy in enemies.enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 1)
            {
                rigidbody.velocity = rigidbody.velocity + (transform.position - enemy.transform.position).normalized * 2;
            }
        }
    }

    void move(Vector3 dest)
    {
        nr++;
        if (Vector3.Distance(dest, transform.position) > 0.2f)
        {
            float multiplayer = speed;
            if (state == State.alert)
                multiplayer /= 2;
            rigidbody.velocity = (dest - transform.position).normalized * multiplayer + pushAwayFromWalls;
            pushAwayFromWalls = new Vector3();
            if (prevStep == transform.position && prevStep == prevPrevStep) //object didn't move, try moving to right
            {
                Vector3 temp = new Vector3(rigidbody.velocity.z, rigidbody.velocity.y, -rigidbody.velocity.x).normalized;
                temp = new Vector3(temp.x, Marian.transform.position.y, temp.z);
                if (clearWay(target, lastSeen, 21))
                {
                    rigidbody.velocity = new Vector3(rigidbody.velocity.z, 0, -rigidbody.velocity.x);
                    target = temp;
                    Debug.Log(nr + " dupa" + transform.position.ToString("F5") + " " + prevStep.ToString("F5") + " " + prevPrevStep.ToString("F5"));
                }
                else
                {
                    rigidbody.velocity = new Vector3(-rigidbody.velocity.z, 0, rigidbody.velocity.x);
                    target = new Vector3(-rigidbody.velocity.z, Marian.transform.position.y, rigidbody.velocity.x);
                }
            }
            prevPrevStep = prevStep;
            prevStep = transform.position;
        }
        //Debug.Log(Time.time);
    }

    bool canSeeMarian(Vector3 pos, int dist)
    {
        LayerMask mask = ~LayerMask.GetMask("Enemy");
        ray = new Ray(pos, Marian.transform.position - pos);
        return (Physics.Raycast(ray, out hit, dist, mask.value) && hit.collider.name == "Marian");
    }
    bool canSeeFoe(Vector3 FoePos, int dist, Enemy foe)
    {
        ray = new Ray(transform.position, FoePos - transform.position);
        return (Physics.Raycast(ray, out hit, dist) && hit.collider.gameObject == foe.transform.gameObject);
    }
    bool clearWay(Vector3 pos, Vector3 dest, float dist)
    {
        return !Physics.Raycast(pos, dest - pos, dist);
    }
    void animateMovement()
    {
        if (Time.time - stepTime > 1 / (speed * 2))
        {
            var quad = this.transform.Find("Quad");
            switch (step)
            {
                case Step.downRight:
                    quad.transform.eulerAngles = new Vector3(quad.transform.eulerAngles.x, -6, quad.transform.eulerAngles.z);
                    quad.transform.localPosition = new Vector3(quad.transform.localPosition.x, quad.transform.localPosition.y, -0.2f);
                    step = Step.upRight;
                    break;
                case Step.upRight:
                    quad.transform.eulerAngles = new Vector3(quad.transform.eulerAngles.x, 0, quad.transform.eulerAngles.z);
                    quad.transform.localPosition = new Vector3(quad.transform.localPosition.x, quad.transform.localPosition.y, 0f);
                    step = Step.downLeft;
                    break;
                case Step.downLeft:
                    quad.transform.eulerAngles = new Vector3(quad.transform.eulerAngles.x, 6, quad.transform.eulerAngles.z);
                    quad.transform.localPosition = new Vector3(quad.transform.localPosition.x, quad.transform.localPosition.y, -0.2f);
                    step = Step.upLeft;
                    break;
                case Step.upLeft:
                    quad.transform.eulerAngles = new Vector3(quad.transform.eulerAngles.x, 0, quad.transform.eulerAngles.z);
                    quad.transform.localPosition = new Vector3(quad.transform.localPosition.x, quad.transform.localPosition.y, 0f);
                    step = Step.downRight;
                    break;
            }
            stepTime = Time.time;
        }
    }
    public void takeDmg(float dmg)
    {
        health -= dmg;
        if (health < 0)
        {
            enemies.enemies.Remove(this);
            Destroy(gameObject);
        }
    }
}