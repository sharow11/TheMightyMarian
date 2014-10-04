using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


public class Map : MonoBehaviour {
    public int generations;
    public int startingGrassPercent;
    public int sizeX, sizeZ;
    private int rsizeX, rsizeZ;
    public MapCell cellPrefab;
    public WaterMapCell waterCellPrefab;
    public GrassMapCell grassCellPrefab;
    public VoidMapCell voidCellPrefab;
    public FloorMapCell floorCellPrefab;


    string path = "mapsavefile.byte";

    //private MapCell[,] map;
    public float generationStepDelay;
	// Use this for initialization

    private int WATER = 1;
    private int VOID = 666; //0
    private int GRASS = 2;
    //private int WALL = 3;
    private int FLOOR = 781; //4

    private int[,] map;

    byte[] saved;
    byte[] loaded;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Generate()
    {
        rsizeX = sizeX + 2;
        rsizeZ = sizeZ + 2;
        map = new int[rsizeX, rsizeZ];
        FillWithVoid();
        FillRandomly();
        for (int i = 0; i < generations; i++)
        {
            CelluralStep();
        }

        //WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        //map = new MapCell[sizeX, sizeZ];
        return DrawMap();

    }

    private IEnumerator DrawMap()
    {
        for (int x = 0; x < rsizeX; x++)
        {
            for (int z = 0; z < rsizeZ; z++)
            {
                yield return 0;
                CreateCell(new IntVector2(x, z), map[x, z]);
            }
        }
    }
    private void FillWithVoid()
    {
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeZ; j++)
            {
                map[i, j] = VOID;
            }
        }
    }

    private void CelluralStep()
    {
        List<IntVector2> toChange = new List<IntVector2>();
        IntVector2 current;

        int voids = 0;
        int floors = 0;

        for (int x = 1; x <= sizeX; x++)
        {
            for (int z = 1; z <= sizeZ; z++)
            {
                voids = CntCellNeighbours(x, z);
                floors = 8 - voids;
                current.x = x;
                current.z = z;

                if (map[x, z] == VOID && floors >= 5)
                    toChange.Add(current);
                else if (map[x, z] == FLOOR && voids >= 5)
                    toChange.Add(current);
            }
        }

        foreach (IntVector2 c in toChange)
        {
            if (map[c.x, c.z] == VOID)
                map[c.x, c.z] = FLOOR;
            else
                map[c.x, c.z] = VOID;
        }
        toChange.Clear();
    }

    private void FillRandomly()
    {
        for (int x = 1; x <= sizeX; x++)
        {
            for (int z = 1; z <= sizeZ; z++)
            {
                if(UnityEngine.Random.Range(0,1000) <= startingGrassPercent*10)
                    map[x, z] = FLOOR;
            }
        }
    }

    private void CreateCell(IntVector2 coordinates, int type)
    {
        if (type == WATER)
        {
            WaterMapCell newCell = Instantiate(waterCellPrefab) as WaterMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z + " type water";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, 0f, coordinates.z - sizeZ * 0.5f + 0.5f);
        }
        else if (type == GRASS)
        {
            GrassMapCell newCell = Instantiate(grassCellPrefab) as GrassMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z + " type grass";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, 0f, coordinates.z - sizeZ * 0.5f + 0.5f);
        }
        else if (type == FLOOR)
        {
            FloorMapCell newCell = Instantiate(floorCellPrefab) as FloorMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z + " type floor";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, 0f, coordinates.z - sizeZ * 0.5f + 0.5f);
        }
        else if (type == VOID)
        {
            VoidMapCell newCell = Instantiate(voidCellPrefab) as VoidMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z + " type void";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, 0f, coordinates.z - sizeZ * 0.5f + 0.5f);
        }
        else
        {
            MapCell newCell = Instantiate(cellPrefab) as MapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z + " type error";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, 0f, coordinates.z - sizeZ * 0.5f + 0.5f);
            Debug.Log("error cell creared, type: " + type);
        }
        //map[coordinates.x, coordinates.z] = newCell;

    }

    private int CntCellNeighbours(int x, int z)
    {

        int n = 0;

        if (map[x + 1,z] == VOID)
            n++;
        if (map[x - 1,z] == VOID)
            n++;
        if (map[x,z + 1] == VOID)
            n++;
        if (map[x,z - 1] == VOID)
            n++;
        if (map[x + 1,z + 1] == VOID)
            n++;
        if (map[x - 1,z - 1] == VOID)
            n++;
        if (map[x + 1,z - 1] == VOID)
            n++;
        if (map[x - 1,z + 1] == VOID)
            n++;
        return n;
    }

    public void Save()
    {
        byte[] bytes = new byte[rsizeX * rsizeZ * sizeof(int)];
        for (int i = 0; i < rsizeX; i++)
            for (int j = 0; j < rsizeZ; j++)
            {
                for(int k = 0; k<sizeof(int);k++)
                {
                    bytes[i * rsizeZ + j+k] = BitConverter.GetBytes(map[i, j])[k];
                }
                if(BitConverter.ToInt32(bytes, i*rsizeZ +j) != map[i,j])
                {
                    Debug.Log("wrong conversion, got " + BitConverter.ToInt32(bytes, i * rsizeZ + j) + " insted of " + map[i, j]);
                }
            }

        File.WriteAllBytes(path, bytes);
        Debug.Log("Map saved, checking");
        saved = bytes;
        MapSavedFine();

    }

    private bool MapSavedFine()
    {
        bool fine = true;
        byte[] bytes = File.ReadAllBytes(path);
        if (bytes.Length != rsizeX * rsizeZ * sizeof(int))
        { Debug.LogError("WRONG FILE SIZE!!!"); return false; }
        if (bytes != saved)
        {
            Debug.LogError("WRONG WRONG WRONG!!!");
        }
        byte[] mybyteint = new byte[sizeof(int)];
        int myint;
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeZ; j++)
            {
                for (int k = 0; k < sizeof(int); k++)
                {
                    mybyteint[k] = bytes[i * rsizeZ + j + k];
                }
                myint = BitConverter.ToInt32(bytes, i * rsizeZ + j);
                if (BitConverter.ToInt32(mybyteint, 0) != map[i, j])
                {
                    Debug.Log("bb Excepted: " + map[i, j] + " insted got: " + myint);
                    fine = false;
                }
                else if (map[i, j] != myint)
                {
                    Debug.Log("Excepted: " + map[i, j] + " insted got: " + myint);
                    fine = false;
                }
                else
                {
                    Debug.Log("Tile " + map[i, j] + "loaded fine");
                }
            }
        }
        
        return fine;
    }
    public IEnumerator Load()
    {
        //FillWithVoid();
        byte[] bytes = File.ReadAllBytes(path);
        if (bytes.Length != rsizeX * rsizeZ * sizeof(int))
        {
            Debug.Log("wrong file size");
            yield return 0;
        }
        else
        {
            for (int i = 0; i < rsizeX; i++)
                for (int j = 0; j < rsizeZ; j++)
                {
                    map[i, j] = BitConverter.ToInt32(bytes, i * rsizeZ + j);
                }
            Debug.Log("Map loaded");
            yield return DrawMap();
        }
     }
}
