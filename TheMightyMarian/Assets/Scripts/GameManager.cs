using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

//public enum EnemyState : byte { idle, alert, follow, searching, chasing, attacking };
//public enum EnemyStep : byte { downRight, upRight, downLeft, upLeft };

public class GameManager : MonoBehaviour
{
    public Map mapPrefab;
    public BossMap bossMapPrefab;
    public FinalMap finalMapPrefab;

    public List<Enemy> enemiesPrefabs;
    public List<int> enemiesRarity;
    public List<Enemy> bossesPrefabs;
    public List<int> bossesRarity;
    public MoveMarian marianPrefab;
    private MoveMarian marian;
    private IMarianMap mapInstance;
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
    private int roomsCnt;
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
        mapInstance = Instantiate(mapPrefab) as IMarianMap;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
        mapSizeX = mapInstance.SizeX;
        mapSizeY = mapInstance.SizeY;
        //StartCoroutine(mapInstance.Generate());
        mapInstance.Generate();
        startRoomNo = mapInstance.StartRoomNo;
        endRoomNo = mapInstance.EndRoomNo;
        roomsCnt = mapInstance.RoomsX * mapInstance.RoomsY;
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
        mapInstance = Instantiate(bossMapPrefab) as IMarianMap;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
        mapSizeX = mapInstance.SizeX;
        mapSizeY = mapInstance.SizeY;
        roomsCnt = 1;
        mapInstance.Generate();
        startRoomNo = 0;
        endRoomNo = 0;
        PlaceMarian();
        PlaceEndLadder();
        PlaceEnemies();
        //isLoading = false;
        Debug.Log("welcome to level " + currLevel);
    }

    private void FinalBeginGame()
    {
        foreach (GroundQuad gq in FindObjectsOfType(typeof(GroundQuad)) as GroundQuad[])
        {
            gq.beWater();
        }
        mapInstance = Instantiate(finalMapPrefab) as IMarianMap;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
        mapSizeX = mapInstance.SizeX;
        mapSizeY = mapInstance.SizeY;
        foreach (GameObject lvlLight in GameObject.FindGameObjectsWithTag("BigLight"))
        { lvlLight.light.intensity = 0.65f; }
        mapInstance.Generate();
        startRoomNo = 0;
        endRoomNo = 0;
        roomsCnt = 1;
        PlaceMarian();
        PlaceEnemies();
        Debug.Log("welcome to final level " + currLevel);
    }

    private void RestartGame() {
        Destroy(mapInstance.gameObject);      
        BeginGame();
    }

    private void SaveMap()
    {
        mapInstance.Save();
    }

    private void LoadMap()
    {
        mapInstance.Load();
    }

    public void PlaceEnemies()
    {
        List<Enemy> myEnemies;
        List<int> myRarity;
        int myEnemiesPerRoom;
        if (bossLvl || currLevel == finalLevel)
        {
            myEnemies = bossesPrefabs;
            myRarity = bossesRarity;
            myEnemiesPerRoom = 1;
        }
        else
        {
            myEnemies = enemiesPrefabs;
            myRarity = enemiesRarity;
            myEnemiesPerRoom = EnemiesPerRoom;
        }

        int totalRarity = myRarity.Sum();
        if (Enemy.enemies == null)
        { Enemy.enemies = new List<Enemy>(); }
        List<Enemy> temp = new List<Enemy>();

        for (int room = 0; room < roomsCnt; room++)
        {
            for (int j = 0; j < myEnemiesPerRoom; j++)
            {
                if (room == mapInstance.StartRoomNo && !(bossLvl || currLevel == finalLevel))
                    continue;

                IntVector2 coordinates = mapInstance.PlaceEnemyInRoom(room);
                //Enemy newEnemy = Instantiate(enemyPrefab) as Enemy;
                Enemy newEnemy = null;
                int currentSum = 0;
                int srand = UnityEngine.Random.Range(0, totalRarity + 1);
                for (int i = 0; i < myEnemies.Count(); i++)
                {
                    currentSum += myRarity[i];
                    if (srand <= currentSum)
                    {
                        newEnemy = Instantiate(myEnemies[i]) as Enemy;
                        break;
                    }
                }
                if (newEnemy == null)
                    return;
                newEnemy.state = Enemy.State.idle;
                newEnemy.transform.parent = transform;
                newEnemy.transform.localPosition = new Vector3(coordinates.x - mapInstance.SizeX * 0.5f + 0.5f, coordinates.y - mapInstance.SizeY * 0.5f + 0.5f, -1.5f);
                temp.Add(newEnemy);
            }
        }
        Enemy.enemies = temp;
    }

    public void PlaceMarian()
    {
        IntVector2 coordinates = mapInstance.GetStartPosForPlayer();
        marian = Instantiate(marianPrefab) as MoveMarian;
        marian.name = "Marian";
        marian.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);

    }

    private void PlaceEndLadder()
    {
        IntVector2 coordinates = mapInstance.GetEndLadderPos(); 
        ladder = Instantiate(ladderPrefab) as Ladder;
        ladder.name = "Ladder to "+(currLevel+1);
        ladder.transform.localPosition = new Vector3(coordinates.x - mapSizeX * 0.5f + 0.5f, coordinates.y - mapSizeY * 0.5f + 0.5f, -1f);
        //if (bossLvl)
        //   ladder.disabled = true;
    }

    public void levelUp()
    {
        currLevel++;
        //Application.LoadLevel(1);
        Application.LoadLevel(3);
    }

}
