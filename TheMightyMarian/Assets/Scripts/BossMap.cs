using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using Assets.Scripts;

public class BossMap : MonoBehaviour, IMarianMap {

    public int generations;
    public int startingFloorsPercent;
    public int sizeX, sizeY;
    public int SizeY
    { get { return sizeY; } }

    public int SizeX
    { get { return sizeX; } }

    public int ShortestPathLength
    { get { return 1; } }
    //public int roomsX, roomsY;
    private int rsizeX, rsizeY;
    private int rsX, rsY; //roomSizeX, roomSizeY;
    //public VoidMapCellCollide voidCellPrefabCollide;
    public FloorMapCell floorCellPrefab;
    public VoidMapStripe voidStripePrefab;
    public Wall wallPrefab;
    private int lvlNo = 0;
    bool bossLvl = false;
    int wallsPlaced = 0;
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

    public int RoomsX
    { get { return 1; } }
    public int RoomsY
    { get { return 1; } }

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

    public void Generate()
    {
        //rsX = sizeX / roomsX;
        //rsY = sizeY / roomsY;
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
        myRoom.Prepare();
        myRoom.Generate();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                smallMap[i + 1, j + 1] = myRoom[i, j];
            }
        }
        ScaleUPx2();
        CelluralSmooth();
        if (logging)
        {
            SaveBitmap("images/map_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
        }
        //lastTouch();
        //eliminate1NarrowPassages();
        //fillAndCry();
        erosion();
        //emptyBossRoom();
        DrawMap();
        if (logging)
        {
            SaveBitmap("images/map_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
        }
        //cleanVariables();
    }


    private void erosion()
    {
        int[] box =  {0, 1, 0, 1, 2, 1, 0, 1, 0};
        for(int i=0; i<rsize2X; i++)
            for (int j = 0; j < rsize2Y; j++)
            {
                crapMap[i, j] = map[i, j];
                map[i, j] = TileTypes.FLOOR;
                if (i == 0 || j == 0 || i == rsize2X - 1 || j == rsize2Y - 1)
                    map[i, j] = TileTypes.VOID;
            }

        for (int i = 1; i < rsize2X-1; i++)
        {
            for (int j = 1; j < rsize2Y-1; j++)
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
                                if(isFineCoords(i + tx - 1, j + ty - 1) && crapMap[i + tx - 1, j + ty - 1] == TileTypes.FLOOR)
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
        DestroyCells();
        int width = 0;
        IntVector2 voidStart = new IntVector2(0, 0); ;
        IntVector2 voidEnd = new IntVector2(0, 0);
        for (int x = 0; x < rsize2X; x++)
        {
            for (int y = 0; y < rsize2Y; y++)
            {
                //CreateCell(new IntVector2(x, y), map[x, y]);

                if (map[x, y] == TileTypes.VOID)
                {
                    if (width == 0)
                    {
                        voidStart = new IntVector2(x, y);
                    }
                    voidEnd = new IntVector2(x, y);
                    width++;
                    if (y == rsize2Y - 1)
                    {
                        CreateStripe(voidStart, voidEnd, width);
                        width = 0;
                    }

                }
                else if (map[x, y] == TileTypes.FLOOR)
                {
                    if (isFineCoords(x, y - 1) && map[x, y - 1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 0); } //a
                    if (isFineCoords(x, y + 1) && map[x, y + 1] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 2); } //c
                    if (isFineCoords(x + 1, y) && map[x + 1, y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 3); } //d
                    if (isFineCoords(x - 1, y) && map[x - 1, y] == TileTypes.VOID)
                    { CreateWall(new IntVector2(x, y), 1); } //b  
                    if (width != 0)
                    {
                        CreateStripe(voidStart, voidEnd, width);
                        width = 0;
                    }
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

    private void CreateStripe(IntVector2 left, IntVector2 right, int width)
    {
        VoidMapStripe newStripe = Instantiate(voidStripePrefab) as VoidMapStripe;
        newStripe.Width = width;
        newStripe.name = "Void stripe from " + left.x + " " + left.y + " to " + right.x + " " + right.y;
        newStripe.setCoordinates(left, right);
        newStripe.transform.parent = transform;
        newStripe.transform.localPosition =
                new Vector3(newStripe.X - sizeX * 0.5f + 0.5f, newStripe.Y - sizeY * 0.5f + 0.5f, 0f);
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
        wallsPlaced++;
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
    {
        return RandomTile(TileTypes.FLOOR);
    }

    public IntVector2 PlaceEnemyInRoom(int room)
    { return RandomTile(TileTypes.FLOOR); }

    public IntVector2 GetEndLadderPos()
    { return RandomTile(TileTypes.FLOOR); }

}
