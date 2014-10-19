using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Map mapPrefab;
    private Map mapInstance;
    public Pathfinder2D path;

    private void Start()
    {
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

    private void BeginGame() {
        mapInstance = Instantiate(mapPrefab) as Map;
        //StartCoroutine(mapInstance.Generate());
        mapInstance.Generate();
        path.manualStart();
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
}
