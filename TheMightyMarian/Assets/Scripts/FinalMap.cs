using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using Assets.Scripts;

public class FinalMap : MonoBehaviour, IMarianMap {

    public int generations;
    public int startingFloorsPercent;
    public int sizeX, sizeY;
    public int SizeY
    { get { return sizeY; } }

    public int SizeX
    { get { return sizeX; } }
    //public int roomsX, roomsY;
    private int rsizeX, rsizeY;
    //public VoidMapCellCollide voidCellPrefabCollide;
    public GrassMapCell grassCellPrefab;
    public Wall wallPrefab;

    public int RoomsX
    { get { return 1; } }
    public int RoomsY
    { get { return 1; } }
    public int ShortestPathLength
    { get { return 1; } }

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

    //private MapCell[,] map;
    private int[,] smallMap;
    private int[,] map;
    private int[,] crapMap;

    public int this[int x, int y]
    {
        get
        {
            if (x >= 0 && y >= 0 && x < rsize2X && y < rsize2Y)
            { return map[x, y]; }
            return -1;
        }
    }

    private int size2X, size2Y;
    private int rsize2X, rsize2Y;

    private IntVector2 playerStartPos;
    private IntVector2 bossStartPos;

    public void Generate()
    {
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        size2X = sizeX * 2;
        size2Y = sizeY * 2;
        rsize2X = size2X + 2;
        rsize2Y = size2Y + 2;
        smallMap = new int[rsizeX, rsizeY];
        map = new int[rsize2X, rsize2Y];
        crapMap = new int[rsize2X, rsize2Y];
        FillWithVoid();

        Room myRoom = new Room(sizeX, sizeY, 0, 0, 0, 0, startingFloorsPercent, "BossSuperRoomOMGWOWDOGE", logging);
        myRoom.FinalFight = true;
        myRoom.Prepare();
        myRoom.Generate();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            { smallMap[i + 1, j + 1] = myRoom[i, j]; }
        }
        ScaleUPx2();
        CelluralSmooth();
        erosion();
        DrawMap();
        calculateInitialPositions();
    }

    private void erosion()
    {
        int[] box = { 0, 1, 0, 1, 2, 1, 0, 1, 0 };
        for (int i = 0; i < rsize2X; i++)
            for (int j = 0; j < rsize2Y; j++)
            {
                crapMap[i, j] = map[i, j];
                map[i, j] = TileTypes.FLOOR;
                if (i == 0 || j == 0 || i == rsize2X - 1 || j == rsize2Y - 1)
                    map[i, j] = TileTypes.VOID;
            }

        for (int i = 1; i < rsize2X - 1; i++)
        {
            for (int j = 1; j < rsize2Y - 1; j++)
            {
                if (crapMap[i, j] == TileTypes.VOID)
                {
                    bool ok = true;
                    for (int ty = 0; ty < 3; ty++)
                    {
                        for (int tx = 0; tx < 3; tx++)
                        {
                            if (box[3 * ty + tx] > 0)
                            {
                                if (isFineCoords(i + tx - 1, j + ty - 1) && crapMap[i + tx - 1, j + ty - 1] == TileTypes.FLOOR)
                                { ok = false; }
                            }
                        }
                    }
                    if (ok)
                    { map[i, j] = TileTypes.VOID; }
                }
            }
        }
    }

    public void DrawMap()
    {
        //DestroyCells();
        for (int x = 0; x < rsize2X; x++)
        {
            for (int y = 0; y < rsize2Y; y++)
            {
                //CreateCell(new IntVector2(x, y), map[x, y]);
                if (map[x, y] == TileTypes.FLOOR)
                {
                    CreateCell(new IntVector2(x, y), map[x, y]);
                    if (isFineCoords(x, y - 1) && map[x, y - 1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 0); } //a
                    if (isFineCoords(x, y + 1) && map[x, y + 1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 2); } //c
                    if (isFineCoords(x + 1, y) && map[x + 1, y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 3); } //d
                    if (isFineCoords(x - 1, y) && map[x - 1, y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 1); } //b  
                }
            }
        }
    }

    private void CreateCell(IntVector2 coordinates, int type)
    {
        if (type == TileTypes.FLOOR)
        {
            GrassMapCell newCell = Instantiate(grassCellPrefab) as GrassMapCell;
            newCell.coordinates = coordinates;
            newCell.type = type;
            newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.y + " type grass";
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - sizeX * 0.5f + 0.5f, coordinates.y - sizeY * 0.5f + 0.5f, 0f);
        }
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
        newWall.adjustPosition();
        //newWall.setRightMaterial();
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
                    if (isFineCoords(x + i, y + j))
                    {
                        if (map[x + i, y + j] == TileTypes.VOID)
                            n++;
                    }
                }
            }
        }
        return n;
    }

    private bool isFineCoords(int x, int y)
    {
        if (x >= 0 && x <= size2X + 1 && y >= 0 && y <= size2Y + 1)
        { return true; }
        return false;
    }

    private void ScaleUPx2()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int t = smallMap[i + 1, j + 1];
                int x = 1 + i * 2;
                int y = 1 + j * 2;
                map[x, y] = t;
                map[x + 1, y] = t;
                map[x, y + 1] = t;
                map[x + 1, y + 1] = t;
            }
        }
    }

    private IntVector2 RandomTile(int tileType)
    {
        int x = UnityEngine.Random.Range(1, rsize2X);
        int y = UnityEngine.Random.Range(1, rsize2Y);
        while (map[x, y] != tileType)
        {
            x = UnityEngine.Random.Range(1, rsize2X);
            y = UnityEngine.Random.Range(1, rsize2Y);
        }
        return new IntVector2(x, y);
    }

    public IntVector2 GetStartPosForPlayer()
    { return playerStartPos; }

    public IntVector2 PlaceEnemyInRoom(int room)
    { return bossStartPos; }

    public IntVector2 GetEndLadderPos()
    { return RandomTile(TileTypes.FLOOR); }

    private void calculateInitialPositions()
    {
        playerStartPos = RandomTile(TileTypes.FLOOR);
        bossStartPos = RandomTile(TileTypes.FLOOR);

        while (bossStartPos.distanceTo(playerStartPos) < sizeX)
        {
            playerStartPos = RandomTile(TileTypes.FLOOR);
            bossStartPos = RandomTile(TileTypes.FLOOR);
        }
    }


    public int StartRoomNo
    {
        get
        {
            return 0;
        }
        set
        {
            
        }
    }

    public int EndRoomNo
    {
        get
        {
            return 0;
        }
        set
        {
            
        }
    }

}
