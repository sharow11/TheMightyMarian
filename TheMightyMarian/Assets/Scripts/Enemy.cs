using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public enum State : byte { idle, alert, searching, chasing, attacking };
    public State state;
    GameObject Marian;
    public Vector3 lastSeen, prevStep, prevPrevStep, target, patrolTarget;
    //float targetTime;
    public int speed = 5;
    float seenLastTime = 0;
    Ray ray;
    RaycastHit hit;
    Color color;
    public List<Vector3> positions;
    List<float> times;
    bool gotPatrolTarget = false;

    int nr = 0;
    // Use this for initialization
    void Start()
    {
        positions = new List<Vector3>();
        times = new List<float>();
        state = 0;
        Marian = GameObject.Find("Marian");
        prevStep = transform.position;
        prevPrevStep = new Vector3(prevStep.x + 0.01f, prevStep.y, prevStep.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        nr = 0;
        while (times.Count > 0 && Time.time - times[0] > 0.5f)
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
            //print("Widać Mariana!" + Vector3.Distance(Marian.transform.position, transform.position));
            //Debug.Log(hit.collider.name + ", " + hit.collider.tag);
        }
        else
        {
            if (state == State.attacking || state == State.chasing)
            {
                state = State.searching;
                lastSeen = positions[0];
            }
        }
        switch (state)
        {
            case State.idle:
                color = Color.white;
                break;
            case State.alert:
                color = Color.black;
                if (gotPatrolTarget)
                {
                    if (Vector3.Distance(transform.position, patrolTarget) < 0.2f) // or some time has passed (timeout/unstuck)
                        gotPatrolTarget = false;
                    else
                        move(patrolTarget);
                }
                else
                {
                    patrolTarget = new Vector3(transform.position.x + Random.value * 15 - 7.5f, Marian.transform.position.y, transform.position.z + Random.value * 15 - 7.5f);
                    gotPatrolTarget = true;
                    int i = 0;
                    while (Physics.Raycast(transform.position, patrolTarget))
                    {
                        if (i > 20)
                        {
                            gotPatrolTarget = false;
                            break;
                        }
                        patrolTarget = new Vector3(transform.position.x + Random.value * 15 - 7.5f, Marian.transform.position.y, transform.position.z + Random.value * 15 - 7.5f);
                        i++;
                    }
                    if (gotPatrolTarget)
                    {
                        move(patrolTarget);
                    }
                }
                break;
            case State.searching:
                color = Color.yellow;
                if (Time.time - seenLastTime > 6 || Vector3.Distance(transform.position, lastSeen) < 0.2f)
                    state = State.alert;
                else
                    move(lastSeen);
                break;
            case State.chasing:
                color = new Color(1, 0.5f, 0);
                move(lastSeen);
                break;
            case State.attacking:
                color = Color.red;
                break;
            default:
                color = Color.white;
                break;
        }
        Debug.DrawLine(transform.position, Marian.transform.position, color);
        Debug.DrawLine(transform.position, lastSeen, Color.blue);
        Debug.Log(nr + "!!!" + transform.position.ToString("F5") + " " + prevStep.ToString("F5") + " " + prevPrevStep.ToString("F5"));
    }

    void move(Vector3 dest)
    {
        nr++;
        if (Vector3.Distance(dest, transform.position) > 0.2f)
        {
            float multiplayer = speed;
            if (state == State.alert)
                multiplayer /= 2;
            rigidbody.velocity = (dest - transform.position).normalized * multiplayer;
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
        ray = new Ray(pos, Marian.transform.position - pos);
        return (Physics.Raycast(ray, out hit, dist) && hit.collider.name == "Marian");
    }
    bool clearWay(Vector3 pos, Vector3 dest, float dist)
    {
        return !Physics.Raycast(pos, dest - pos, dist);
    }
}
