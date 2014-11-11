using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Map mapPrefab;
    public Enemy enemyPrefab;
    public MoveMarian marianPrefab;
    public Enemy blueGhostPrefab;
    private MoveMarian marian;
    private Map mapInstance;
    public Ladder ladderPrefab;
    private Ladder ladder;
    public bool logging = true;
    public Texture2D czarnosc;
    int startRoomNo, endRoomNo;

    public int EnemiesPerRoom = 3;

    public static int currLevel = 0;
    public bool isLoading = true;
    //private bool generateCalled = false;
    private int state = 0;
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
            state++;
            //show loading screen
        }
        else if (state == 1)
        {
            //generate friz
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), czarnosc);
            GUI.Box(new Rect(10, 10, Screen.width, Screen.height), "Loading level " + currLevel);
            BeginGame();
            
            state++;
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
        mapInstance = Instantiate(mapPrefab) as Map;
        mapInstance.Logging = logging;
        mapInstance.LvlNo = currLevel;
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

    private void RestartGame() {
        //StopAllCoroutines();
        Destroy(mapInstance.gameObject);
        BeginGame();
    }

    private void SaveMap()
    { 
        mapInstance.Save();
    }

    private void LoadMap()
    {
        //StopAllCoroutines();
        //StartCoroutine(mapInstance.Load());
        mapInstance.Load();
    }

    public void PlaceEnemies()
    {
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
                IntVector2 coordinates = mapInstance.PlaceEnemyInRoom(room);
                //Enemy newEnemy = Instantiate(enemyPrefab) as Enemy;
                Enemy newEnemy = Instantiate(blueGhostPrefab) as Enemy;
                //newEnemy.transform.localPosition
                newEnemy.name = "Zbigniew";
                newEnemy.transform.parent = transform;
                newEnemy.transform.localPosition = new Vector3(coordinates.x - mapInstance.sizeX * 0.5f + 0.5f, coordinates.y - mapInstance.sizeY * 0.5f + 0.5f, -1.5f);
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
        marian.transform.localPosition = new Vector3(coordinates.x - mapInstance.sizeX * 0.5f + 0.5f, coordinates.y - mapInstance.sizeY * 0.5f + 0.5f, -1f);
    }

    private void PlaceEndLadder()
    {
        IntVector2 coordinates = mapInstance.GetEndLadderPos();
        ladder = Instantiate(ladderPrefab) as Ladder;
        ladder.name = "Ladder to "+(currLevel+1);
        ladder.transform.localPosition = new Vector3(coordinates.x - mapInstance.sizeX * 0.5f + 0.5f, coordinates.y - mapInstance.sizeY * 0.5f + 0.5f, -1f);
    }

    public void levelUp()
    {
        currLevel++;
        Application.LoadLevel(1);
    }

}
