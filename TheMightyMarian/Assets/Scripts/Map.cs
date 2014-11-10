﻿using UnityEngine;
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
    //public VoidMapCellCollide voidCellPrefabCollide;
    public FloorMapCell floorCellPrefab;
    public Wall wallPrefab;
    private int lvlNo = 0;
    bool bossLvl = false;
    public int LvlNo
    {
        get { return lvlNo; }
        set { lvlNo = value; }
    }
    private bool logging;

    public bool Logging
    {
        get { return logging; }
        set { logging = value; }
    }
    string path = "mapsavefile.byte";

    //private MapCell[,] map;
    private int[,] smallMap;
    private int[,] map;

    public int this[int x, int y]
    {
        get
        {
            if (x >= 0 && y >= 0 && x < rsize2X && y < rsize2Y)
            { return map[x, y]; }
            return -1;
        }
    }

    Maze maze;
    List<Room> myRooms;

    private int size2X, size2Y;
    private int rsize2X, rsize2Y;

    private int startRoomNo;
    public int StartRoomNo
    {
        get { return startRoomNo; }
        set { startRoomNo = value; }
    }
    private int endRoomNo;

    public int EndRoomNo
    {
        get { return endRoomNo; }
        set { endRoomNo = value; }
    }

	void Start () {}
	void Update () {}

    public void Generate()
    {
        if (lvlNo != 0 && lvlNo % 5 == 0)
        { bossLvl = true; }
        rsX = sizeX / roomsX;
        rsY = sizeY / roomsY;
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        size2X = sizeX * 2;
        size2Y = sizeY * 2;
        rsize2X = size2X + 2;
        rsize2Y = size2Y + 2;
        smallMap = new int[rsizeX, rsizeY];
        map = new int[rsize2X, rsize2Y];
        FillWithVoid();

        maze = new Maze(roomsY, roomsX, sizeX, sizeY, rsX, rsY, startingFloorsPercent);
        maze.Logging = logging;
        maze.Generate();
        startRoomNo = maze.StartRoomNo;
        endRoomNo = maze.EndRoomNo;
        myRooms = maze.GetRooms();
        initializeRooms();
        translateRoomsToMap();
        ScaleUPx2();
        CelluralSmooth();
        if (logging)
        {
            SaveBitmap("images/map_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
        }
        lastTouch();
        //eliminate1NarrowPassages();
        DrawMap();
        if (logging)
        {
            SaveBitmap("images/map_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
        }
        cleanVariables();
    }

    private void lastTouch()
    {
        List<IntVector2> flors = new List<IntVector2>();
        for (int i = 1; i <= size2X; i++)
        {
            for (int j = 1; j <= size2Y; j++)
            {
                if (map[i, j] == TileTypes.FLOOR && CntCellNeighboursWalls(i,j) > 0)
                {
                    //horizontal
                    if (map[i + 1, j] == TileTypes.FLOOR && map[i - 1, j] == TileTypes.FLOOR)
                    {
                        for (int x = i - 1; x < i + 2; x++)
                            for (int y = j - 1; y < j + 2; y++)
                                flors.Add(new IntVector2(x, y));
                    }

                    //vertical
                    if (map[i, j+1] == TileTypes.FLOOR && map[i, j+1] == TileTypes.FLOOR)
                    {
                        for (int x = i - 1; x < i + 2; x++)
                            for (int y = j - 1; y < j + 2; y++)
                                flors.Add(new IntVector2(x, y));
                    }
                }
            }
        }
        foreach (IntVector2 iv in flors)
        {
            if (iv.x > 0 && iv.x < rsize2X - 1 && iv.y > 0 && iv.y < rsize2Y - 1)
            { map[iv.x, iv.y] = TileTypes.FLOOR; }
        }
    }

    private void eliminate1NarrowPassages()
    {
        List<IntVector2> flors = new List<IntVector2>();
        for (int i = 1; i <= size2X; i++)
        {
            for (int j = 1; j <= size2Y; j++)
            {
                if (map[i, j] == TileTypes.FLOOR && CntCellNeighboursWalls(i, j) > 1)
                {
                    //horizontal
                    if (map[i + 1, j] == TileTypes.FLOOR && map[i - 1, j] == TileTypes.FLOOR)
                    {
                        int leftNeib = 0;
                        int rightNeib = 0;

                        for (int m = j - 1; m < j + 2; m++)
                        {
                            if (map[i - 1, m] != TileTypes.FLOOR)
                            { leftNeib++; }
                            if (map[i + 1, m] != TileTypes.FLOOR)
                            { rightNeib++; }
                        }

                        if (leftNeib == 0 || rightNeib == 0)
                            continue;

                        bool left = false;
                        if (leftNeib > rightNeib)
                        { left = true; }
                        else if (leftNeib == rightNeib)
                        { left = (i < rsize2X / 2) ? true : false; }
                        else
                        { left = false; }

                        if (left)
                        {
                            for (int y = j - 1; y < j + 2; y++)
                                flors.Add(new IntVector2(i - 1, y));
                        }
                        else
                        {
                            for (int y = j - 1; y < j + 2; y++)
                                flors.Add(new IntVector2(i + 1, y));
                        }
                    }

                    //vertical
                    if (map[i, j + 1] == TileTypes.FLOOR && map[i, j + 1] == TileTypes.FLOOR)
                    {
                        int leftNeib = 0;
                        int rightNeib = 0;

                        for (int m = i - 1; m < i + 2; m++)
                        {
                            if (map[m, j-1] != TileTypes.FLOOR)
                            { leftNeib++; }
                            if (map[m, j+1] != TileTypes.FLOOR)
                            { rightNeib++; }
                        }

                        if (leftNeib == 0 || rightNeib == 0)
                            continue;                        

                        bool left = false;
                        if (leftNeib > rightNeib)
                        { left = true; }
                        else if (leftNeib == rightNeib)
                        { left = (j < rsize2Y / 2) ? true : false; }
                        else
                        { left = false; }

                        if (left)
                        {
                            for (int x = i - 1; x < i + 2; x++)
                                flors.Add(new IntVector2(x, j-1));
                        }
                        else
                        {
                            for (int x = i - 1; x < i + 2; x++)
                                flors.Add(new IntVector2(x, j + 1));
                        }
                    }
                }
            }
        }

        foreach (IntVector2 iv in flors)
        {
            if (iv.x > 0 && iv.x < rsize2X - 1 && iv.y > 0 && iv.y < rsize2Y - 1)
            { map[iv.x, iv.y] = TileTypes.FLOOR; }
        }
    }

    private void cleanVariables()
    {
        smallMap = null;
        myRooms.Clear();
        maze = null;
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
                        smallMap[x+offsetX+1,y+offsetY+1] = myRooms[ i + roomsX * j][x,y];
                    }
                }
            }
        }
    }

    public void DrawMap()
    {
        DestroyCells();
        for (int x = 0; x < rsize2X; x++)
        {
            for (int y = 0; y < rsize2Y; y++)
            {
                CreateCell(new IntVector2(x, y), map[x, y]);
                if (map[x, y] == TileTypes.FLOOR)
                {
                    if (isFineCoords(x, y - 1) && map[x,y-1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 0); } //a
                    if (isFineCoords(x, y + 1) && map[x,y+1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 2); } //c
                    if (isFineCoords(x + 1, y) && map[x+1,y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 3); } //d
                    if (isFineCoords(x - 1, y) && map[x-1,y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 1); } //b
                    
                }
            }
        }
    }

    private void FillWithVoid()
    {
        for (int i = 0; i < rsizeX; i++)
        {
            for (int j = 0; j < rsizeY; j++)
            {
                smallMap[i, j] = TileTypes.VOID;
            }
        }
        for (int i = 0; i < rsize2X; i++)
        {
            for (int j = 0; j < rsize2Y; j++)
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
        { map[c.x, c.y] = TileTypes.VOID; }
        wals.Clear();
        foreach (IntVector2 c in flors)
        { map[c.x, c.y] = TileTypes.FLOOR; }
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

    private void CelluralSmooth()
    {
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;
        int walls = 0;
        for (int x = 1; x < size2X + 1; x++)
        {
            for (int y = 1; y < size2Y + 1; y++)
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
        { map[c.x, c.y] = TileTypes.VOID; }
        wals.Clear();
        foreach (IntVector2 c in flors)
        { map[c.x, c.y] = TileTypes.FLOOR; }
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
            /*FloorMapCell newCell = Instantiate(floorCellPrefab) as FloorMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type floor";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);*/
        }
        else if (type == TileTypes.VOID)
        {
            /*
            if (CntCellNeighboursWalls(coordinates.x, coordinates.y) < 8)
            {
                VoidMapCellCollide newCell = Instantiate(voidCellPrefabCollide) as VoidMapCellCollide;
                newCell.coordinates = coordinates;
                newCell.type = type;
                newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type void collider";
                newCell.transform.parent = transform;
                newCell.transform.localPosition =
                    new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
            }
            else
            {
                VoidMapCell newCell = Instantiate(voidCellPrefab) as VoidMapCell;
                newCell.coordinates = coordinates;
                newCell.type = type;
                newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type void";
                newCell.transform.parent = transform;
                newCell.transform.localPosition =
                    new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
            }*/
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
            Debug.Log("error cell created, type: " + type);
        }
        //map[coordinates.x, coordinates.z] = newCell;

    }

    private void CreateWall(IntVector2 coordinates, int rotation)
    {
        Wall newWall = Instantiate(wallPrefab) as Wall;
        newWall.Coordinates = coordinates;
        newWall.name = "Wall " + coordinates.x + ", " + coordinates.y;
        newWall.transform.parent = transform;
        newWall.transform.localPosition =
            new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        newWall.setRotation(rotation);
        newWall.adjustPosition(rotation);
    }

    private int CntCellNeighboursWalls(int x, int y)
    {
        int n = 0;
        for (int i = x - 1; i < x + 2; i++)
        {
            for (int j = y - 1; j < y + 2; j++)
            {
                if (isFineCoords(i, j))
                {
                    if (!(j == y && i == x) && map[i, j] == TileTypes.VOID)
                    { n++; }
                }
            }
        }
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
        { Destroy(other.gameObject);}

        GrassMapCell[] othersGrass = FindObjectsOfType(typeof(GrassMapCell)) as GrassMapCell[];
        foreach (GrassMapCell other in othersGrass)
        { Destroy(other.gameObject); }

        VoidMapCell[] othersVoid = FindObjectsOfType(typeof(VoidMapCell)) as VoidMapCell[];
        foreach (VoidMapCell other in othersVoid)
        { Destroy(other.gameObject);}

        FloorMapCell[] othersFloor = FindObjectsOfType(typeof(FloorMapCell)) as FloorMapCell[];
        foreach (FloorMapCell other in othersFloor)
        { Destroy(other.gameObject);}

        WaterMapCell[] othersWater = FindObjectsOfType(typeof(WaterMapCell)) as WaterMapCell[];
        foreach (WaterMapCell other in othersWater)
        { Destroy(other.gameObject);}

        Wall[] walls = FindObjectsOfType(typeof(Wall)) as Wall[];
        foreach (Wall other in walls)
        { Destroy(other.gameObject);}
    }

    private bool isFineCoords(int x, int y)
    {
        if (x >= 0 && x <= size2X+1 && y >= 0 && y <= size2Y+1)
        { return true; }
        return false;
    }

    public void SaveBitmap(String filename = "kupa.png")
    {
        Texture2D obrazek = new Texture2D(rsize2X * 6 + 1, rsize2Y * 6 + 1);
        for (int i = 0; i < rsize2X * 6 + 1; i++)
        {
            for (int j = 0; j < rsize2Y * 6 + 1; j++)
            {
                obrazek.SetPixel(i, j, DawnBringer16.Blue);
            }
        }
        for (int i = 0; i < rsize2X; i++)
        {
            for (int j = 0; j < rsize2Y; j++)
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
        for (int i = 0; i < rsize2X * 6 + 1; i++)
        { obrazek.SetPixel(i, rsize2Y * 6, DawnBringer16.DarkGrey); }
        for (int i = 0; i < rsize2Y * 6 + 1; i++)
        { obrazek.SetPixel(rsize2X * 6, i, DawnBringer16.DarkGrey); }
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

    private void ScaleUPx2()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int t = smallMap[i + 1, j + 1];
                int x = 1 + i*2;
                int y = 1 + j*2;
                map[x, y] = t;
                map[x + 1, y] = t;
                map[x, y + 1] = t;
                map[x + 1, y + 1] = t;
            }
        }
    }

    private int readTileFromRoom(int room, int x, int y)
    {
        if (room >= roomsX * roomsY || x >= rsX*2 || y >= rsY*2)
        {
            return TileTypes.ERROR;
        }
        IntVector2 coords = convertRoomCordsToMap(room, x, y);
        if (isFineCoords(coords.x,coords.y) && CntCellNeighboursWalls2(coords.x, coords.y) == 0)
        {
            return map[coords.x, coords.y];
        }
        return TileTypes.ERROR;
    }

    private IntVector2 convertRoomCordsToMap(int room, int x, int y)
    {
        int roomPosY = room / roomsX;
        int roomPosX = room - (roomPosY * roomsX);
        return new IntVector2(x + roomPosX*rsX*2 + 1, y + roomPosY*rsY*2 + 1);
    }

    private IntVector2 RandomTileFromRoom(int room, int tileType)
    {
        int x = UnityEngine.Random.Range(0, rsX * 2);
        int y = UnityEngine.Random.Range(0, rsY * 2);
        int roomPosY = room / roomsX;
        int roomPosX = room - (roomPosY * roomsX);
        while (readTileFromRoom(room, x, y) != tileType)
        {
            x = UnityEngine.Random.Range(0, rsX * 2);
            y = UnityEngine.Random.Range(0, rsY * 2);
        }
        return convertRoomCordsToMap(room, x, y);
        

    }

    public IntVector2 GetStartPosForPlayer()
    {
        return RandomTileFromRoom(startRoomNo, TileTypes.FLOOR);
    }

    public IntVector2 PlaceEnemyInRoom(int room)
    { return RandomTileFromRoom(room, TileTypes.FLOOR); }

    public IntVector2 GetEndLadderPos()
    { return RandomTileFromRoom(startRoomNo, TileTypes.FLOOR); }

}