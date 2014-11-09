using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerForDebug : MonoBehaviour
{
    public Map mapPrefab;
    //public Enemy enemyPrefab;
    //public MoveMarian marianPrefab;
    //private MoveMarian marian;
    private Map mapInstance;
    public bool logging = true;

    int startRoomNo, endRoomNo;
    //public List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        this.name = "GameManagerDebug";
        BeginGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SaveMap();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadMap();
        }
    }

    private void BeginGame()
    {
        mapInstance = Instantiate(mapPrefab) as Map;
        mapInstance.Logging = logging;
        //StartCoroutine(mapInstance.Generate());
        mapInstance.Generate();
        startRoomNo = mapInstance.StartRoomNo;
        endRoomNo = mapInstance.EndRoomNo;

        //PlaceMarian();
        //PlaceEnemies();
    }

    private void RestartGame()
    {
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

}
