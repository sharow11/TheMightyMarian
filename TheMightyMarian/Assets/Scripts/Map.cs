using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
//using System.Security.Cryptography;
 


public class Map : MonoBehaviour {
    public int generations;
    public int startingFloorsPercent;
    public int sizeX, sizeY;
    private int rsizeX, rsizeY;
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

    //byte[] saved;
    //byte[] loaded;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public IEnumerator Generate()
    public void Generate()
    {
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        map = new int[rsizeX, rsizeY];
        FillWithVoid();
        FillRandomly();
        SaveBitmap("images/it0.png");
        for (int i = 0; i < 4; i++)
        {
            CelluralStep1();
            SaveBitmap("images/it"+(i+1)+".png");
        }

        for (int i = 0; i < 2; i++)
        {
            CelluralStep2();
            SaveBitmap("images/it" + (i+5) + ".png");
        }

        //WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        //map = new MapCell[sizeX, sizeZ];
        DrawMap();

    }

    //private IEnumerator DrawMap()
    private void DrawMap()
    {
        DestroyCells();
        //renderer.enabled = false;
        for (int x = 0; x < rsizeX; x++)
        {
            for (int y = 0; y < rsizeY; y++)
            {
                //yield return 0;
                CreateCell(new IntVector2(x, y), map[x, y]);
            }
        }
        //renderer.enabled = true;
    }

    private void FillWithVoid()
    {
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeY; j++)
            {
                map[i, j] = VOID;
            }
        }
    }

    private void CelluralStep2()
    {
        //List<IntVector2> toChange = new List<IntVector2>();
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;

        int walls = 0;
        //int floors = 0;

        for (int x = 1; x <= sizeX; x++)
        {
            for (int y = 1; y <= sizeY; y++)
            {
                walls = CntCellNeighboursWalls(x, y);
                //floors = 8 - walls;
                current.x = x;
                current.y = y;

                //if (map[x, y] == VOID && floors >= 5)
                //    toChange.Add(current);
                //else if (map[x, y] == FLOOR && voids >= 5)
                //    toChange.Add(current);
                if (walls >= 5)
                    wals.Add(current);
                else
                    flors.Add(current);
            }
        }

        //foreach (IntVector2 c in toChange)
        //{
        //    if (map[c.x, c.y] == VOID)
        //        map[c.x, c.y] = FLOOR;
        //    else
        //        map[c.x, c.y] = VOID;
        //}
        //toChange.Clear();
        foreach (IntVector2 c in wals)
        {
            map[c.x, c.y] = VOID;
        }
        wals.Clear();
        foreach (IntVector2 c in flors)
        {
            map[c.x, c.y] = FLOOR;

        }
        flors.Clear();
    }

    private void CelluralStep1()
    {
        //List<IntVector2> toChange = new List<IntVector2>();
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;

        int walls = 0;
        int walls2 = 0;
        //int floors = 0;
        //int floors2 = 0;

        for (int x = 1; x <= sizeX; x++)
        {
            for (int y = 1; y <= sizeY; y++)
            {
                walls = CntCellNeighboursWalls(x, y);
                walls2 = CntCellNeighboursWalls2(x, y);
                //floors = 8 - walls;
                //floors2 = 6 - walls2;
                current.x = x;
                current.y = y;

                //if (map[x, z] == VOID && (floors >= 5 || floors2 <= 1))
                //    toChange.Add(current);
                //else if (map[x, z] == FLOOR && voids >= 5)
                //    toChange.Add(current);

                if (walls >= 5 || walls2 <= 2)
                    wals.Add(current);
                else
                    flors.Add(current);
            }
        }

        //foreach (IntVector2 c in toChange)
        //{
        //    if (map[c.x, c.y] == VOID)
        //        map[c.x, c.y] = FLOOR;
        //    else
        //        map[c.x, c.y] = VOID;
        //}
        //toChange.Clear();
        foreach (IntVector2 c in wals)
        {
           map[c.x, c.y] = VOID;
        }
        wals.Clear();
        foreach (IntVector2 c in flors)
        {
           map[c.x, c.y] = FLOOR;

        }
        flors.Clear();
    }

    private void FillRandomly()
    {
        for (int x = 1; x <= sizeX; x++)
        {
            for (int y = 1; y <= sizeY; y++)
            {
                if(UnityEngine.Random.Range(0,101) <= startingFloorsPercent)
                    map[x, y] = FLOOR;
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
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type water";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == GRASS)
        {
            GrassMapCell newCell = Instantiate(grassCellPrefab) as GrassMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type grass";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == FLOOR)
        {
            FloorMapCell newCell = Instantiate(floorCellPrefab) as FloorMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type floor";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == VOID)
        {
            VoidMapCell newCell = Instantiate(voidCellPrefab) as VoidMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type void";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else
        {
            MapCell newCell = Instantiate(cellPrefab) as MapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type error";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
            Debug.Log("error cell creared, type: " + type);
        }
        //map[coordinates.x, coordinates.z] = newCell;

    }

    private int CntCellNeighboursWalls(int x, int y)
    {

        int n = 0;

        if (map[x + 1,y] == VOID)
            n++;
        if (map[x - 1,y] == VOID)
            n++;
        if (map[x,y + 1] == VOID)
            n++;
        if (map[x,y - 1] == VOID)
            n++;
        if (map[x + 1,y + 1] == VOID)
            n++;
        if (map[x - 1,y - 1] == VOID)
            n++;
        if (map[x + 1,y - 1] == VOID)
            n++;
        if (map[x - 1,y + 1] == VOID)
            n++;
        return n;
    }

    private int CntCellNeighboursWalls2(int x, int y)
    {
        int n = 0;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if (i != 0 || j != 0)
                {
                    if(isFineCoords(x+i,y+j))
                    {
                        if (map[x+i, y+j] == VOID)
                            n++;
                    }
                }
            }
        }
        
        //if (isFineCoords(x - 2, z - 2))
        //{
        //    if (map[x-2, z-2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 1, z - 2))
        //{
        //    if (map[x - 1, z - 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x, z - 2))
        //{
        //    if (map[x, z - 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 1, z - 2))
        //{
        //    if (map[x + 1, z - 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 2, z - 2))
        //{
        //    if (map[x + 2, z - 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 2, z - 1))
        //{
        //    if (map[x + 2, z - 1] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 2, z))
        //{
        //    if (map[x + 2, z] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 2, z + 1))
        //{
        //    if (map[x + 2, z + 1] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 2, z + 2))
        //{
        //    if (map[x + 2, z + 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x + 1, z + 2))
        //{
        //    if (map[x + 1, z + 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x, z + 2))
        //{
        //    if (map[x, z + 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 1, z + 2))
        //{
        //    if (map[x - 1, z + 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 2, z + 2))
        //{
        //    if (map[x - 2, z + 2] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 2, z + 1))
        //{
        //    if (map[x - 2, z + 1] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 2, z))
        //{
        //    if (map[x - 2, z] == VOID)
        //        n++;
        //}
        //if (isFineCoords(x - 2, z - 1))
        //{
        //    if (map[x - 2, z-1] == VOID)
        //        n++;
        //}
        ////if (isFineCoords(x - 2, z - 2))
        ////{
        ////    if (map[x - 2, z] == VOID)
        ////        n++;
        ////}

        return n;
    }

    public void Save()
    {
        //byte[] bytes = new byte[rsizeX * rsizeZ * sizeof(int)];
        //for (int i = 0; i < rsizeX; i++)
        //    for (int j = 0; j < rsizeZ; j++)
        //    {
        //        for(int k = 0; k<sizeof(int);k++)
        //        {
        //            bytes[i * rsizeZ + j+k] = BitConverter.GetBytes(map[i, j])[k];
        //        }
        //        if(BitConverter.ToInt32(bytes, i*rsizeZ +j) != map[i,j])
        //        {
        //            Debug.Log("wrong conversion, got " + BitConverter.ToInt32(bytes, i * rsizeZ + j) + " insted of " + map[i, j]);
        //        }
        //    }

        //File.WriteAllBytes(path, bytes);
        //Debug.Log("Map saved, checking");
        //saved = bytes;
        //MapSavedFine();
        try
        {

            using (Stream fileStream = File.Open(path, FileMode.Create))
            {
                //using (var stream = new GZipStream(fileStream, CompressionMode.Compress))
                //{
                //    BinaryFormatter bin = new BinaryFormatter();
                //    bin.Serialize(stream, map);
                //}
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(fileStream, map);

            }
        }
        catch (IOException)
        {
            Debug.Log("nie udalo sie");
        }

    }

/*
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
 */

    public void Load()
    {
        int[,] loaded;
        //FillWithVoid();
        //byte[] bytes = File.ReadAllBytes(path);
        //if (bytes.Length != rsizeX * rsizeZ * sizeof(int))
        //{
        //    Debug.Log("wrong file size");
        //    yield return 0;
        //}
        //else
        //{
        //    for (int i = 0; i < rsizeX; i++)
        //        for (int j = 0; j < rsizeZ; j++)
        //        {
        //            map[i, j] = BitConverter.ToInt32(bytes, i * rsizeZ + j);
        //        }
        //    Debug.Log("Map loaded");
        //    yield return DrawMap();
        //}

        using (Stream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
        {

            //using (var stream = new GZipStream(fileStream, CompressionMode.Decompress))
            //{
            //    BinaryFormatter bin = new BinaryFormatter();
            //    loaded = (int[,])bin.Deserialize(stream);
            //}

            BinaryFormatter bin = new BinaryFormatter();
            loaded = (int[,])bin.Deserialize(fileStream);

        }

        if (loaded.Length == map.Length)
        {
            map = loaded;
            DrawMap();
        }
        else
        {
            Debug.Log("wrong size");
            
        }
        

     }

    public void DestroyCells()
    {
        MapCell[] others = FindObjectsOfType(typeof(MapCell)) as MapCell[];

        foreach (MapCell other in others)
        {
            Destroy(other.gameObject);
        }

        GrassMapCell[] othersGrass = FindObjectsOfType(typeof(GrassMapCell)) as GrassMapCell[];

        foreach (GrassMapCell other in othersGrass)
        {
            Destroy(other.gameObject);
        }

        VoidMapCell[] othersVoid = FindObjectsOfType(typeof(VoidMapCell)) as VoidMapCell[];

        foreach (VoidMapCell other in othersVoid)
        {
            Destroy(other.gameObject);
        }

        FloorMapCell[] othersFloor = FindObjectsOfType(typeof(FloorMapCell)) as FloorMapCell[];

        foreach (FloorMapCell other in othersFloor)
        {
            Destroy(other.gameObject);
        }

        WaterMapCell[] othersWater = FindObjectsOfType(typeof(WaterMapCell)) as WaterMapCell[];

        foreach (WaterMapCell other in othersWater)
        {
            Destroy(other.gameObject);
        }
    }

    private bool isFineCoords(int x, int y)
    {
        if (x >= 0 && x <= sizeX+1 && y >= 0 && y <= sizeY+1)
        { return true; }
        return false;
    }

    public void SaveBitmap(String filename = "kupa.png")
    {
        Texture2D obrazek = new Texture2D(rsizeX * 6 + 1, rsizeY * 6 + 1);
        for (int i = 0; i < rsizeX * 6 + 1; i++)
        {
            for (int j = 0; j < rsizeY * 6 + 1; j++)
            {
                obrazek.SetPixel(i, j, Color.cyan);
            }
        }
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeY; j++)
            {
                Color now;
                int ileftcorner = i * 6 + 1;
                int jleftcorner = j * 6 + 1;
                if (map[i, j] == FLOOR)
                { now = Color.white; }
                else
                { now = Color.black; }

                for (int ii = ileftcorner; ii < ileftcorner + 6; ii++)
                {
                    for (int jj = jleftcorner; jj < jleftcorner + 6; jj++)
                    {
                        obrazek.SetPixel(ii, jj, now);
                    }
                }
                for (int ii = 0; ii < 6; ii++)
                {
                    obrazek.SetPixel(i * 6, j * 6 + ii, Color.gray);
                    obrazek.SetPixel(i * 6 + ii, j * 6, Color.gray);
                }
            }
        }
        for (int i = 0; i < rsizeX * 6 + 1; i++)
        { obrazek.SetPixel(i, rsizeY * 6, Color.gray); }
        for (int i = 0; i < rsizeY * 6 + 1; i++)
        { obrazek.SetPixel(rsizeX * 6, i, Color.gray); }
        //obrazek.Save("kupa.bmp");
        var bytes = obrazek.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }
}
