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
    public int roomsX, roomsY;
    private int rsizeX, rsizeY;
    private int rsX, rsY; //roomSizeX, roomSizeY;
    public MapCell cellPrefab;
    public WaterMapCell waterCellPrefab;
    public GrassMapCell grassCellPrefab;
    public VoidMapCell voidCellPrefab;
    public FloorMapCell floorCellPrefab;

    string path = "mapsavefile.byte";

    //private MapCell[,] map;
    private int[,] map;

    Maze maze;
    List<Room> myRooms;



	void Start () {}
	void Update () {}

    public void Generate()
    {
        rsX = sizeX / roomsX;
        rsY = sizeY / roomsY;
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        map = new int[rsizeX, rsizeY];
        FillWithVoid();

        maze = new Maze(roomsY, roomsX, sizeX, sizeY, rsX, rsY, startingFloorsPercent);
        maze.Generate();

        myRooms = maze.GetRooms();
        initializeRooms();
        translateRoomsToMap();
        DrawMap();
        SaveBitmap("images/map_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
    }

    public void translateRoomsToMap()
    {
        int offsetX, offsetY;
        for (int i = 0; i < roomsX; i++)
        {
            for (int j = 0; j < roomsY; j++)
            {
                offsetX = i * rsX;
                offsetY = j * rsY;
                for (int x = 0; x < rsX; x++)
                {
                    for (int y = 0; y < rsY; y++)
                    {
                        map[x+offsetX,y+offsetY] = myRooms[ i + roomsX * j][x,y];
                    }
                }
            }
        }
    }
    //public IEnumerator Generate()
    public void GenerateOld()
    {
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        map = new int[rsizeX, rsizeY];


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

        DrawMap();

    }

    public void DrawMap()
    {
        DestroyCells();
        for (int x = 0; x < rsizeX; x++)
        {
            for (int y = 0; y < rsizeY; y++)
            {
                CreateCell(new IntVector2(x, y), map[x, y]);
            }
        }
    }

    private void FillWithVoid()
    {
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeY; j++)
            {
                map[i, j] = TileTypes.VOID;
            }
        }
    }

    private void CelluralStep2()
    {
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;

        int walls = 0;


        for (int x = 1; x <= sizeX; x++)
        {
            for (int y = 1; y <= sizeY; y++)
            {
                walls = CntCellNeighboursWalls(x, y);
                current.x = x;
                current.y = y;

                if (walls >= 5)
                    wals.Add(current);
                else
                    flors.Add(current);
            }
        }

        foreach (IntVector2 c in wals)
        {
            map[c.x, c.y] = TileTypes.VOID;
        }
        wals.Clear();
        foreach (IntVector2 c in flors)
        {
            map[c.x, c.y] = TileTypes.FLOOR;

        }
        flors.Clear();
    }

    private void CelluralStep1()
    {
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;

        int walls = 0;
        int walls2 = 0;

        for (int x = 1; x <= sizeX; x++)
        {
            for (int y = 1; y <= sizeY; y++)
            {
                walls = CntCellNeighboursWalls(x, y);
                walls2 = CntCellNeighboursWalls2(x, y);

                current.x = x;
                current.y = y;

                if (walls >= 5 || walls2 <= 2)
                    wals.Add(current);
                else
                    flors.Add(current);
            }
        }

        foreach (IntVector2 c in wals)
        {
           map[c.x, c.y] = TileTypes.VOID;
        }
        wals.Clear();
        foreach (IntVector2 c in flors)
        {
           map[c.x, c.y] = TileTypes.FLOOR;

        }
        flors.Clear();
    }

    private void CreateCell(IntVector2 coordinates, int type)
    {
        if (type == TileTypes.WATER)
        {
            WaterMapCell newCell = Instantiate(waterCellPrefab) as WaterMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type water";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == TileTypes.GRASS)
        {
            GrassMapCell newCell = Instantiate(grassCellPrefab) as GrassMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type grass";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == TileTypes.FLOOR)
        {
            FloorMapCell newCell = Instantiate(floorCellPrefab) as FloorMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type floor";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
        else if (type == TileTypes.VOID)
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

        if (map[x + 1, y] == TileTypes.VOID)
            n++;
        if (map[x - 1, y] == TileTypes.VOID)
            n++;
        if (map[x, y + 1] == TileTypes.VOID)
            n++;
        if (map[x, y - 1] == TileTypes.VOID)
            n++;
        if (map[x + 1, y + 1] == TileTypes.VOID)
            n++;
        if (map[x - 1, y - 1] == TileTypes.VOID)
            n++;
        if (map[x + 1, y - 1] == TileTypes.VOID)
            n++;
        if (map[x - 1, y + 1] == TileTypes.VOID)
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
                        if (map[x + i, y + j] == TileTypes.VOID)
                            n++;
                    }
                }
            }
        }
        
        return n;
    }

    public void Save()
    {
        try
        {
            using (Stream fileStream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(fileStream, map);
            }
        }
        catch (IOException)
        {
            Debug.Log("nie udalo sie");
        }
    }

    public void Load()
    {
        int[,] loaded;
        using (Stream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
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
                obrazek.SetPixel(i, j, DawnBringer16.Blue);
            }
        }
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeY; j++)
            {
                Color now;
                int ileftcorner = i * 6 + 1;
                int jleftcorner = j * 6 + 1;
                if (map[i, j] == TileTypes.FLOOR)
                { now = DawnBringer16.White; }
                else
                { now = DawnBringer16.Black; }

                for (int ii = ileftcorner; ii < ileftcorner + 6; ii++)
                {
                    for (int jj = jleftcorner; jj < jleftcorner + 6; jj++)
                    {
                        obrazek.SetPixel(ii, jj, now);
                    }
                }
                for (int ii = 0; ii < 6; ii++)
                {
                    obrazek.SetPixel(i * 6, j * 6 + ii, DawnBringer16.DarkGrey);
                    obrazek.SetPixel(i * 6 + ii, j * 6, DawnBringer16.DarkGrey);
                }
            }
        }
        for (int i = 0; i < rsizeX * 6 + 1; i++)
        { obrazek.SetPixel(i, rsizeY * 6, DawnBringer16.DarkGrey); }
        for (int i = 0; i < rsizeY * 6 + 1; i++)
        { obrazek.SetPixel(rsizeX * 6, i, DawnBringer16.DarkGrey); }
        //obrazek.Save("kupa.bmp");
		obrazek.SetPixel (0, 0, Color.red);
        var bytes = obrazek.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }

    private void initializeRooms()
    {
        foreach (Room room in myRooms)
        {
            room.Prepare();
            room.Generate();
        }
    }
}
