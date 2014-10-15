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
    public MapCell cellPrefab;
    public WaterMapCell waterCellPrefab;
    public GrassMapCell grassCellPrefab;
    public VoidMapCell voidCellPrefab;
    public FloorMapCell floorCellPrefab;

    string path = "mapsavefile.byte";

    //private MapCell[,] map;
    private int[,] map;

    private int[,] maze;
    private int[,] mazeComplete;


	void Start () {}
	void Update () {}

    public void Generate()
    {
        rsizeX = sizeX + 2;
        rsizeY = sizeY + 2;
        map = new int[rsizeX, rsizeY];
        maze = new int[roomsX*roomsY, roomsX*roomsY];
        mazeComplete = new int[roomsX * roomsY, roomsX * roomsY];
        MazeCompleteClear();
        MazeGenerate();
    }

    private void MazeGenerate()
    {
        //MazeClear();
        //MazeCompleteClear();

        for(int i=0; i<roomsX; i++)
            for (int j = 0; j < roomsY; j++)
            {
				/*
                if (i != 0)
                { 
                    int daleko = UnityEngine.Random.Range(3,666);
                    maze[MazeVertexNum(i, j),MazeVertexNum(i - 1, j)] = daleko;
                    maze[MazeVertexNum(i-1, j), MazeVertexNum(i, j)] = daleko;
                }*/
                if (i != roomsX - 1)
                {
                    int daleko = UnityEngine.Random.Range(3, 666);
                    maze[MazeVertexNum(i, j), MazeVertexNum(i + 1, j)] = daleko;
                    maze[MazeVertexNum(i + 1, j), MazeVertexNum(i, j)] = daleko;
                }
				/*
                if (j != 0)
                {
                    int daleko = UnityEngine.Random.Range(3, 666);
                    maze[MazeVertexNum(i, j), MazeVertexNum(i, j-1)] = daleko;
                    maze[MazeVertexNum(i, j-1), MazeVertexNum(i, j)] = daleko;
                }*/
                if (j != roomsY - 1)
                {
                    int daleko = UnityEngine.Random.Range(3, 666);
                    maze[MazeVertexNum(i, j), MazeVertexNum(i, j+1)] = daleko;
                    maze[MazeVertexNum(i, j+1), MazeVertexNum(i, j)] = daleko;
                }
            }

        PrimsMagic();
		SaveMazeToImage("images/mazes/"+DateTime.Now.ToString("yyyyMMddHHmmssffff")+".png");
    }

    private void PrimsMagic()
    {
        int startVertexX = UnityEngine.Random.Range(0, roomsX);
        int startVertexY = UnityEngine.Random.Range(0, roomsY);
        int startVertex = MazeVertexNum(startVertexX, startVertexY);

        int[] distances = new int[roomsX*roomsY];
        int[] daddy = new int[roomsX * roomsY];
        List<int> queue = new List<int>();
        for (int i = 0; i < roomsY * roomsX; i++)
        {
            distances[i] = Int32.MaxValue;
            daddy[i] = -666;
            queue.Add(i);
        }
        distances[startVertex] = 0;


        //visited.Add(startVertex);
        //queue.Sort((v1, v2) => MazeHowFar(startVertex, v1).CompareTo(MazeHowFar(startVertex, v2)));
        queue.Sort((v1, v2) => distances[v1].CompareTo(distances[v2]));

        int currentvertex = startVertex;
        while (queue.Count > 0)
        {
            currentvertex = queue[0];
            queue.RemoveAt(0);

            foreach (int neib in MazeGetNeibours(currentvertex))
            { 
                if(queue.Contains(neib))
                {
                    if (MazeHowFar(currentvertex, neib) < distances[neib])
                    {
                        distances[neib] = MazeHowFar(currentvertex, neib);
                        daddy[neib] = currentvertex;
                    }
                }
            }
            queue.Sort((v1, v2) => distances[v1].CompareTo(distances[v2]));

        }
        for (int i = 0; i < daddy.Length; i++)
        {
            int passageType = UnityEngine.Random.Range(1,5);
            if (daddy[i] >= 0 && daddy[i] < daddy.Length)
            {
                mazeComplete[i, daddy[i]] = passageType;
                mazeComplete[daddy[i], i] = passageType;
            }
        }
    }

    private int MazeVertexNum(int x, int y)
    { return x + roomsX * y; }

    private IntVector2 MazeVertexCoords(int num)
    { 
        int y = num/roomsX;
        int x = num - y;
        return new IntVector2(x, y);
    }

    private List<int> MazeGetNeibours(int v)
    {
        List<int> sasiedzi = new List<int>();
        for (int i = 0; i < roomsX * roomsY; i++)
            if (maze[v, i] > 0 && i != v)
                sasiedzi.Add(i);
        return sasiedzi;
    }

    private int MazeHowFar(int v1, int v2)
    {
        if(maze[v1,v2] == 0 || maze[v2,v1] == 0)
            return Int32.MaxValue;
        return (maze[v1, v2] + maze[v2, v1]) / 2;
    }

    private void MazeCompleteClear()
    {
        for (int i = 0; i < roomsX * roomsY; i++)
            for (int j = 0; j < roomsY * roomsX; j++)
            {
                maze[i, j] = 0;
                mazeComplete[i, j] = 0;
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

    private void SaveMazeToImage(String filename = "maze.png")
    {
        int XX = roomsX * 4 + (roomsX-1)*4 + 2;
        int YY = roomsY * 4 + (roomsY-1)*4 + 2;
        Texture2D obrazek = new Texture2D(XX,YY);
        
        for (int i = 0; i < XX; i++)
        {
            for (int j = 0; j < YY; j++)
            {
                obrazek.SetPixel(i, j, DawnBringer16.Blue);
            }
        }
        for (int i = 0; i < roomsX; i++)
        {
            for (int j = 0; j < roomsY; j++)
            {
                for(int k=4*i;k<4*i+2;k++)
                    for(int l=4*j;l<4*j+2;l++)
                        obrazek.SetPixel(k, l, DawnBringer16.White);

                //obrazek.SetPixel(4 * i + 1, 4 * j, DawnBringer16.White);
                //obrazek.SetPixel(4 * i, 4*j+1, DawnBringer16.White);
                //obrazek.SetPixel(4 * i + 1, 4 * j + 1, DawnBringer16.White);

                Color current;
                if (i + 1 < roomsX)
                    current = MazeColorForPassage(mazeComplete[MazeVertexNum(i, j), MazeVertexNum(i + 1, j)]);
                else
                    current = DawnBringer16.Black;

                for (int k = 4 * i+2; k < 4 * i + 4; k++)
                    for (int l = 4 * j; l < 4 * j + 2; l++)
                        obrazek.SetPixel(k, l, current);

                if(j+1 < roomsY)
                    current = MazeColorForPassage(mazeComplete[MazeVertexNum(i, j), MazeVertexNum(i, j + 1)]);
                else
                    current = DawnBringer16.Black;
                for (int k = 4 * i; k < 4 * i + 2; k++)
                    for (int l = 4 * j+2; l < 4 * j + 4; l++)
                        obrazek.SetPixel(k, l, current);
                current = DawnBringer16.Black;
                for (int k = 4 * i+2; k < 4 * i + 4; k++)
                    for (int l = 4 * j + 2; l < 4 * j + 4; l++)
                        obrazek.SetPixel(k, l, current);
            }
        }
        var bytes = obrazek.EncodeToPNG();

        File.WriteAllBytes(filename, bytes);

    }

    private Color MazeColorForPassage(int p)
    {

        Color now;
        if (p == 0)
        { now = DawnBringer16.Black; }
        else if (p == 1)
        { now = DawnBringer16.Orange; }
        else if (p == 2)
        { now = DawnBringer16.Yellow; }
        else if (p == 3)
        { now = DawnBringer16.Red; }
        else if (p == 4)
        { now = DawnBringer16.PinkBeige; }
        else
        { now = DawnBringer16.Green; }

        return now;
    }
}
