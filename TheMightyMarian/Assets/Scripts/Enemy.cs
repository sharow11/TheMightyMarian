using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public enum State : byte { idle, alert, follow, searching, chasing, attacking };

    public GameObject sword;
    public GameObject arrow;
    public GameObject staff;
    public enum Step : byte { downRight, upRight, downLeft, upLeft };
    public GameObject greenBolt;
    public GameObject greenBlast;
    public GameObject blood;
    public GameObject meat1;
    public GameObject meat2;
    public GameObject meat3;
    public GameObject death;
    public GameObject meleeSplash;

    /************** PARAMETRY **************/
    public float speed = 5;
    public float maxHealth = 100;
    float health;
    public float viewDistance = 25;
    public float damage = 5;
    public bool isMelee = false;

    /************** ATAKI **************/
    public float spread = 10; //kąt w stopniach okręślający ile maksymalnie od celu mogą pudłować strzały.
    public float predictionProbability = 0.75f; //Prawdopodobieństwo na to że przeciwnik wystrzelić pocisk w miejsce, w którym przewidywalnie znajdzie się Marian, gdy pocisk tam doleci.
    public float predictionImportanceMin = 0.5f; //minimalna waga obliczonej pozycji w stosunku do aktualnej
    public float predictionImportanceMax = 1; //maksymalna waga obliczonej pozycji w stosunku do aktualnej (używany jest random z <min, max>)
    public float atkRange = 15;
    public float projectileSpeed = 15;
    public float attackFreq = 0.5f;

    public bool gotRaild = false; // to jest potrzebne po to, aby nie dostać 2 razy jednym railem.
    float lastAtkTime = 0;
    public State state = State.idle;
    Step step = Step.downRight;
    float stepTime = 0;
    public bool canFollowSteps;
    GameObject MarianObject;
    MoveMarian moveMarian;
    public static List<Enemy> enemies;
    public Vector3 lastSeen, prevStep, prevPrevStep, target, patrolTarget, pushAwayFromWalls;
    float seenLastTime = 0;
    public float patrolTargetAssignTime;
    Ray ray;
    RaycastHit hit;
    Color color;
    GameObject shot;
    EnemyProjectile shotProj;
    public bool gotPatrolTarget = false;
    bool lostTrack = false;
    int nr = 0;
    // Use this for initialization


    void Start()
    {
        health = maxHealth;
        attackFreq = 0.5f;
        MarianObject = GameObject.Find("Marian");
        moveMarian = (MoveMarian)MarianObject.GetComponent("MoveMarian");
        //Debug.Log(moveMarian);
        prevStep = transform.position;
        prevPrevStep = new Vector3(prevStep.x + 0.01f, prevStep.y, prevStep.z);
    }

    void Awake()
    {
        enemies = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gotRaild = false;
        if (Enemy.enemies == null)
            return;

        nr = 0;
        if (canSeeMarian(transform.position, viewDistance))
        {
            seenLastTime = Time.time;
            lastSeen = MarianObject.transform.position;
            if (Vector3.Distance(MarianObject.transform.position, transform.position) < atkRange)
                state = State.attacking;
            else
                state = State.chasing;
            gotPatrolTarget = false;

            //foreach (Enemy enemy in enemies.enemies)
            foreach (Enemy enemy in Enemy.enemies)
            {
                if ((enemy.state == State.idle || enemy.state == State.alert) && Vector3.Distance(transform.position, enemy.transform.position) < viewDistance && canSeeFoe(enemy.transform.position, viewDistance, enemy))
                {
                    enemy.gotPatrolTarget = true;
                    enemy.patrolTarget = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    enemy.patrolTargetAssignTime = Time.time;
                    enemy.lostTrack = false;
                    enemy.state = State.follow;
                    Debug.Log(hit.collider.name + " poinformowany");
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
                lastSeen = MarianObject.transform.position;
                lastSeen.z = transform.position.z;
            }
        }
        switch (state)
        {
            case State.idle:
                color = Color.white;
                break;
            case State.alert:
            case State.follow:
                Music.hype += Time.deltaTime;
                animateMovement();
                color = Color.black;
                if (gotPatrolTarget)
                {
                    if (Vector3.Distance(transform.position, patrolTarget) < 1.5f || (!canFollowSteps && Time.time - patrolTargetAssignTime > 6))
                    {
                        gotPatrolTarget = false;
                        state = State.alert;
                    }
                    else
                    {
                        move(patrolTarget);
                    }
                    if (Time.time - patrolTargetAssignTime > 9)
                    {
                        lostTrack = true;
                        gotPatrolTarget = false;
                        state = State.alert;
                        Debug.Log("Lost track due to timeout");
                    }
                    Debug.DrawLine(transform.position, patrolTarget, Color.cyan);
                }
                else if (!canFollowSteps || lostTrack)
                {
                    LayerMask mask = LayerMask.GetMask("Enemy");
                    mask += LayerMask.GetMask("MarianProjectile");
                    mask = ~mask;
                    patrolTarget = new Vector3(transform.position.x + Random.value * 25 - 12.5f, transform.position.y + Random.value * 25 - 12.5f, transform.position.z);
                    gotPatrolTarget = true;
                    int i = 0;
                    while (Physics.Raycast(transform.position, patrolTarget - transform.position, Vector3.Distance(transform.position, patrolTarget), mask))
                    {
                        if (i > 10)
                        {
                            gotPatrolTarget = false;
                            break;
                        }
                        patrolTarget = new Vector3(transform.position.x + Random.value * 25 - 12.5f, transform.position.y + Random.value * 25 - 12.5f, transform.position.z);
                        i++;
                    }
                    if (gotPatrolTarget)
                    {
                        move(patrolTarget);
                        patrolTargetAssignTime = Time.time;
                        Debug.DrawLine(transform.position, patrolTarget, Color.white);
                    }
                    else
                        Debug.DrawLine(transform.position, patrolTarget, Color.blue);
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
                            lostTrack = false;
                            break;
                        }
                    }
                    //Debug.Log(gotPatrolTarget);
                    if (!gotPatrolTarget)
                        lostTrack = true;
                    Debug.DrawLine(transform.position, patrolTarget, Color.green);
                }
                break;
            case State.searching:
                Music.hype += Time.deltaTime * 2;
                animateMovement();
                color = Color.yellow;
                if (Time.time - seenLastTime > 6 || Vector3.Distance(transform.position, lastSeen) < 1.5f)
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
                Music.hype += Time.deltaTime * 5;
                animateMovement();
                color = new Color(1, 0.5f, 0);
                move(lastSeen);
                Debug.DrawLine(transform.position, MarianObject.transform.position, color);
                break;
            case State.attacking:
                Music.hype += Time.deltaTime * 5;
                if (Time.time - lastAtkTime >= attackFreq)
                {
                    Music.hype += 1f;
                    lastAtkTime = Time.time;
                    if (isMelee)
                    {
                        GameObject atk = (GameObject)Instantiate(meleeSplash, (transform.position + MarianObject.transform.position) / 2, new Quaternion());
                        atk.GetComponent<StickToMarian>().setOffset((transform.position - MarianObject.transform.position)/2);
                        Destroy(atk, 1);
                        MarianObject.GetComponent<Attack>().takeDmg((int)damage);
                    }
                    else
                    {
                        shot = (GameObject)Instantiate(greenBolt, new Vector3(transform.position.x, transform.position.y + 0.5f, -2), new Quaternion());
                        Destroy(shot, 3);
                        Vector3 direction = Quaternion.Euler(0, 0, Random.value * spread * 2 - spread) * (new Vector3(MarianObject.transform.position.x, MarianObject.transform.position.y, -2) - shot.transform.position).normalized; ;
                        Vector3 predictedDirection;
                        if (Random.value < predictionProbability)
                        {
                            float distance = Vector3.Distance(MarianObject.transform.position, transform.position);
                            float flyTime = distance / projectileSpeed;
                            Vector3 marianPosAfter = MarianObject.transform.position + MarianObject.rigidbody.velocity * flyTime;
                            predictedDirection = Quaternion.Euler(0, 0, Random.value * spread * 2 - spread) * (new Vector3(marianPosAfter.x, marianPosAfter.y, -2) - shot.transform.position).normalized;
                            float predictionImportance = predictionImportanceMin + Random.value * (predictionImportanceMax - predictionImportanceMin);
                            direction = direction * (1 - predictionImportance) + predictionImportance * predictedDirection;
                        }
                        shot.transform.LookAt(new Vector3(transform.position.x + direction.x * 64, transform.position.y + direction.y * 64, -2));
                        shot.rigidbody.velocity = direction * projectileSpeed;
                        shot.GetComponent<EnemyProjectile>().dmg = damage;
                    }
                }
                color = Color.red;
                Debug.DrawLine(transform.position, MarianObject.transform.position, color);
                break;
            default:
                color = Color.white;
                break;
        }
        //Debug.DrawLine(transform.position, lastSeen, Color.blue);
        //Debug.Log(nr + "!!!" + transform.position.ToString("F5") + " " + prevStep.ToString("F5") + " " + prevPrevStep.ToString("F5"));
        //foreach (Enemy enemy in enemies.enemies)
        foreach (Enemy enemy in Enemy.enemies)
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
            /*if (prevStep == transform.position && prevStep == prevPrevStep) //object didn't move, try moving to right
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
            prevStep = transform.position;*/
        }
        //Debug.Log(Time.time);
    }

    bool canSeeMarian(Vector3 pos, float dist)
    {
        LayerMask mask = LayerMask.GetMask("Enemy");
        mask += LayerMask.GetMask("MarianProjectile");
        mask = ~mask;
        ray = new Ray(pos, MarianObject.transform.position - pos);
        return (Physics.Raycast(ray, out hit, dist, mask.value) && hit.collider.name == "Marian");
    }

    bool canSeeFoe(Vector3 FoePos, float dist, Enemy foe)
    {
        LayerMask mask = ~LayerMask.GetMask("MarianProjectile");
        ray = new Ray(transform.position, FoePos - transform.position);
        return (Physics.Raycast(ray, out hit, dist, mask.value) && hit.collider.gameObject == foe.transform.gameObject);
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

    public void takeDmg(float dmg, Vector3 dir)
    {
        Music.hype += (float)dmg / 2;
        dir = new Vector3(dir.x, dir.y, -0.1f);
        GameObject bloodObj = (GameObject)Instantiate(blood, new Vector3(transform.position.x, transform.position.y - 0.5f, -2), new Quaternion());
        bloodObj.transform.LookAt(transform.position + dir);
        bloodObj.particleSystem.startSize = dmg / 50 + 1.8f;
        bloodObj.particleSystem.startSpeed = dmg / 3 + 5;
        bloodObj.particleSystem.startLifetime = dmg / 200 + 0.15f;
        //bloodObj.particleSystem.Emit((int)dmg * 3 + 1);
        bloodObj.particleSystem.emissionRate = dmg * 20;
        Destroy(bloodObj, 1);
        health -= dmg;
        if (health < 0)
        {
            Destroy((GameObject)Instantiate(death, new Vector3(transform.position.x, transform.position.y, -1), new Quaternion()), 0.9f); // Tworzę obiekt do usunięcia :)
            for (int i = 0; i < 10; i++)
            {
                GameObject meat;
                if (Random.value < 0.33333f)
                {
                    meat = (GameObject)Instantiate(meat1, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    meat.transform.localScale = new Vector3(Random.value / 25 + 0.02f, Random.value / 25 + 0.02f, Random.value / 25 + 0.02f);
                }
                else if (Random.value < 0.66666f)
                {
                    meat = (GameObject)Instantiate(meat2, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    meat.transform.localScale = new Vector3(Random.value / 25 + 0.02f, Random.value / 25 + 0.02f, Random.value / 25 + 0.02f);
                }
                else
                {
                    meat = (GameObject)Instantiate(meat3, new Vector3(transform.position.x, transform.position.y, -2), new Quaternion());
                    meat.transform.localScale = new Vector3(Random.value / 100 + 0.005f, Random.value / 100 + 0.005f, Random.value / 100 + 0.005f);
                }
                meat.transform.rotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
                meat.rigidbody.velocity = new Vector3(Random.value, Random.value, Random.value) * 10 + dir * (dmg / 3 + 5);
                meat.rigidbody.inertiaTensorRotation = new Quaternion(Random.value, Random.value, Random.value, Random.value);
                Destroy(meat, 2.0f + Random.value * 2);
            }
            Marian.Exp += 10;
            float rng = Random.value;
            if(rng < 0.025f)
            {
                shot = (GameObject)Instantiate(sword, new Vector3(transform.position.x, transform.position.y, -0.2f), Quaternion.identity);
                shot.transform.Rotate(Vector3.right, 90.0f);
            }
            else if(rng < 0.05f)
            {
                shot = (GameObject)Instantiate(arrow, new Vector3(transform.position.x, transform.position.y, -0.2f), Quaternion.identity);
            }
            else if (rng < 0.075f)
            {
                shot = (GameObject)Instantiate(staff, new Vector3(transform.position.x, transform.position.y, -0.2f), Quaternion.identity);
                shot.transform.Rotate(Vector3.right, -90.0f);
            }

            Enemy.enemies.Remove(this);
            Destroy(gameObject);
        }
    }

    public void takeDmg(float dmg)
    {
        takeDmg(dmg, new Vector3(0, 0, -9999.9f));
    }

    public void alertEnemy(Vector3 origin)
    {
        LayerMask mask = LayerMask.GetMask("Enemy");
        mask += LayerMask.GetMask("Marian");
        mask += LayerMask.GetMask("MarianProjectile");
        mask = ~mask;
        Ray ray = new Ray(transform.position, origin - transform.position);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, origin), mask.value))
        {
            state = Enemy.State.follow;
            patrolTarget = origin;
            patrolTarget.z = transform.position.z;
            patrolTargetAssignTime = Time.time;
            gotPatrolTarget = true;
        }
        else if (state != Enemy.State.chasing)
        {
            state = Enemy.State.alert;
        }
    }
}