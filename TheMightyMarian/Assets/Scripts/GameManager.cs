using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum EnemyState : byte { idle, alert, follow, searching, chasing, attacking };
//public enum EnemyStep : byte { downRight, upRight, downLeft, upLeft };

public class GameManager : MonoBehaviour
{
    public Map mapPrefab;
    public BossMap bossMapPrefab;
    public FinalMap finalMapPrefab;

    public Enemy enemyPrefab;
    public MoveMarian marianPrefab;
    public Enemy blueGhostPrefab;
    public int blueGhostRarity;
    public Enemy greenGhostPrefab;
    public int greenGhostRarity;
    public Enemy redGhostPrefab;
    public int redGhostRarity;
    public Enemy spiderPrefab;
    public int spiderRarity;
    public Enemy billPrefab;
    private MoveMarian marian;
    private Map mapInstance;
    private BossMap bossMapInstance;
    private FinalMap finalMapInstance;
    public Ladder ladderPrefab;
    private Ladder ladder;
    public bool logging = true;
    public Texture2D czarnosc;
    int startRoomNo, endRoomNo;

    public int EnemiesPerRoom = 3;

    public static int currLevel = 0;
    public static int finalLevel = -1;
    public bool isLoading = true;
    private int mapSizeX, mapSizeY;
    //private bool generateCalled = false;
    private int state = 0;
    public bool bossLvl;
    private void Start()
    {
        isLoading = true;
        this.name = "GameManager";
        state = 0;
        //BeginGame();
    }

    
    private void OnGUI()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SaveMap();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            //LoadMap();
        }*/
        if (state == 0)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), czarnosc);
            GUI.Box(new Rect(10, 10, Screen.width, Screen.height), "Loading level "+currLevel);
            if (finalLevel < 0)
            { 
                finalLevel = 3; // UnityEngine.Random.Range(15, 21); 
            }
            state++;
            //show loading screen
        }
        else if (state == 1)
        {
            //generate friz
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), czarnosc);
            GUI.Box(new Rect(10, 10, Screen.width, Screen.height), "Loading level " + currLevel);
            state++;
            BeginGame();
            
            //state++;
        }
        else if (state == 2)
        {
            //hide loading
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), czarnosc);
            GUI.Box(new Rect(10, 10, Screen.width, Screen.height), "Loading level " + currLevel);
            isLoading = false;
            state++;

        }
        else
        { return; }
        
    }

    public IEnumerator BeginGameCoroutine() {

        mapInstance = Instantiate(mapPrefab) as Map;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
        //StartCoroutine(mapInstance.Generate());
        //yield return mapInstance.GenerateCoroutine();
        StartCoroutine(mapInstance.GenerateCoroutine());
        startRoomNo = mapInstance.StartRoomNo;
        endRoomNo = mapInstance.EndRoomNo;
        yield return null;
        //PlaceMarian();
        PlaceEndLadder();
        //PlaceEnemies();
        //isLoading = false;
        Debug.Log("welcome to level " + currLevel);
        
    }

    public void BeginGame()
    {
        if (currLevel % 5 == 0)
        { bossLvl = true; }
        else
        { bossLvl = false; }
        if (currLevel == finalLevel)
        { FinalBeginGame(); }
        else if (bossLvl)
        { BossBeginGame(); }
        else
        { NormalBeginGame(); }

    }

    private void NormalBeginGame()
    {
        foreach (GameObject lvlLight in GameObject.FindGameObjectsWithTag("BigLight"))
        { lvlLight.light.intensity = 0.025f; }
        foreach (GroundQuad gq in FindObjectsOfType(typeof(GroundQuad)) as GroundQuad[])
        { gq.beFloor(); }
        mapInstance = Instantiate(mapPrefab) as Map;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
        mapSizeX = mapInstance.sizeX;
        mapSizeY = mapInstance.sizeY;
        //StartCoroutine(mapInstance.Generate());
        mapInstance.Generate();
        startRoomNo = mapInstance.StartRoomNo;
        endRoomNo = mapInstance.EndRoomNo;
        PlaceMarian();
        PlaceEndLadder();
        PlaceEnemies();
        //isLoading = false;
        Debug.Log("welcome to level " + currLevel);
    }

    private void BossBeginGame()
    {
        foreach (GameObject lvlLight in GameObject.FindGameObjectsWithTag("BigLight"))
        { lvlLight.light.intensity = 0.020f; }
        foreach (GroundQuad gq in FindObjectsOfType(typeof(GroundQuad)) as GroundQuad[])
        {
            gq.beFloor();
        }
        bossMapInstance = Instantiate(bossMapPrefab) as BossMap;
        bossMapInstance.Logging = logging;
        bossMapInstance.LvlNo = currLevel;
        mapSizeX = bossMapInstance.sizeX;
        mapSizeY = bossMapInstance.sizeY;
        //StartCoroutine(mapInstance.Generate());
        bossMapInstance.Generate();
        startRoomNo = 0;
        endRoomNo = 0;
        PlaceMarian();
        PlaceEndLadder();
        PlaceBoss();
        //isLoading = false;
        Debug.Log("welcome to level " + currLevel);
    }

    private void FinalBeginGame()
    {
        foreach (GroundQuad gq in FindObjectsOfType(typeof(GroundQuad)) as GroundQuad[])
        {
            gq.beWater();
        }
        finalMapInstance = Instantiate(finalMapPrefab) as FinalMap;
        finalMapInstance.Logging = logging;
        finalMapInstance.LvlNo = currLevel;
        mapSizeX = finalMapInstance.sizeX;
        mapSizeY = finalMapInstance.sizeY;
        foreach (GameObject lvlLight in GameObject.FindGameObjectsWithTag("BigLight"))
        { lvlLight.light.intensity = 0.65f; }
        //MapCell[] others = FindObjectsOfType(typeof(MapCell)) as MapCell[];
        //foreach (MapCell other in others)
        //{ Destroy(other.gameObject); }
        finalMapInstance.Generate();
        startRoomNo = 0;
        endRoomNo = 0;
        PlaceMarian();
        //PlaceBoss();
        Debug.Log("welcome to final level " + currLevel);
    }
    private void RestartGame() {
        //StopAllCoroutines();
        if (currLevel == finalLevel)
        { Destroy(finalMapInstance.gameObject); }
        else if (bossLvl)
        { Destroy(bossMapInstance.gameObject); }
        else
        { Destroy(mapInstance.gameObject); }
        
        BeginGame();
    }

    private void SaveMap()
    {
        if (bossLvl || currLevel == finalLevel)
            return;
        mapInstance.Save();
    }

    private void LoadMap()
    {
        //StopAllCoroutines();
        //StartCoroutine(mapInstance.Load());
        if (bossLvl ||currLevel == finalLevel)
            return;
        mapInstance.Load();
    }

    public void PlaceEnemies()
    {
        if (bossLvl)
            return;
        int totalRarity = blueGhostRarity + greenGhostRarity + redGhostRarity + spiderRarity;

        int roomsCnt = mapInstance.roomsX * mapInstance.roomsY;
        if (Enemy.enemies == null)
        {
            Enemy.enemies = new List<Enemy>();
        }
        List<Enemy> temp = new List<Enemy>();
        for (int room = 0; room < roomsCnt; room++)
        {
            for (int j = 0; j < EnemiesPerRoom; j++)
            {
                if (room == mapInstance.StartRoomNo)
                    continue;

                IntVector2 coordinates = mapInstance.PlaceEnemyInRoom(room);
                //Enemy newEnemy = Instantiate(enemyPrefab) as Enemy;
                Enemy newEnemy;
                int srand = UnityEngine.Random.Range(0,totalRarity+1);
                if (srand < blueGhostRarity)
                { 
                    newEnemy = Instantiate(blueGhostPrefab) as Enemy;
                    newEnemy.name = "Zbigniew";
                }
                else if (srand < blueGhostRarity + redGhostRarity)
                { 
                    newEnemy = Instantiate(redGhostPrefab) as Enemy;
                    newEnemy.name = "Bogdan";
                }
                else if (srand < blueGhostRarity + redGhostRarity + greenGhostRarity)
                { 
                    newEnemy = Instantiate(greenGhostPrefab) as Enemy;
                    newEnemy.name = "Apoloniusz";
                }
                else
                { 
                    newEnemy = Instantiate(spiderPrefab) as Enemy;
                    newEnemy.name = "JanMaria";
                }
                newEnemy.state = Enemy.State.idle;
                //newEnemy.transform.localPosition
                
                newEnemy.transform.parent = transform;
                newEnemy.transform.localPosition = new Vector3(coordinates.x - mapInstance.sizeX * 0.5f + 0.5f, coordinates.y - mapInstance.sizeY * 0.5f + 0.5f, -1.5f);
                temp.Add(newEnemy);
            }
        }
        Enemy.enemies = temp;
    }

    private void PlaceBoss()
    {
        if (finalLevel == currLevel)
        {
            return;
        }
        if (Enemy.enemies == null)
        {
            Enemy.enemies = new List<Enemy>();
        }
        List<Enemy> temp = new List<Enemy>();


        IntVector2 coordinates = bossMapInstance.PlaceEnemyInRoom(0);
        //Enemy newEnemy = Instantiate(enemyPrefab) as Enemy;
        Enemy newEnemy;

        newEnemy = Instantiate(billPrefab) as Enemy;
        newEnemy.name = "BillCipher";
                

        //newEnemy.transform.localPosition

        newEnemy.transform.parent = transform;
        newEnemy.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1.5f);
        temp.Add(newEnemy);
            
        
        Enemy.enemies = temp;
    }

    public void PlaceMarian()
    {
        IntVector2 coordinates;
        if (currLevel == finalLevel)
        {
            coordinates = finalMapInstance.GetStartPosForPlayer();
            marian = Instantiate(marianPrefab) as MoveMarian;
            marian.name = "Marian";
            marian.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);
        }
        else if (bossLvl)
        {
            coordinates = bossMapInstance.GetStartPosForPlayer();
            marian = Instantiate(marianPrefab) as MoveMarian;
            marian.name = "Marian";
            marian.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);
        }
        else
        {
            coordinates = mapInstance.GetStartPosForPlayer();
            marian = Instantiate(marianPrefab) as MoveMarian;
            marian.name = "Marian";
            marian.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);
        }


    }

    private void PlaceEndLadder()
    {
        IntVector2 coordinates;
        if (bossLvl)
        {  coordinates = bossMapInstance.GetEndLadderPos(); }
        else
        { coordinates = mapInstance.GetEndLadderPos(); }
        ladder = Instantiate(ladderPrefab) as Ladder;
        ladder.name = "Ladder to "+(currLevel+1);
        ladder.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);
        //if (bossLvl)
        //   ladder.disabled = true;
    }

    public void levelUp()
    {
        currLevel++;
        Application.LoadLevel(1);
    }

}
