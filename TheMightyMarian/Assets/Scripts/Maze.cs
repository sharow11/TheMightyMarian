using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Maze {

	private int[,] maze;
	private int[,] mazeComplete;
	private int roomsX, roomsY;
    private int sizeX, sizeY;
    private int roomSizeX, roomSizeY;
    private int sfp;
    private bool logging;

    public int ShortestPathLength
    { get; private set; }

    public int StartRoomNo = 0;
    public int EndRoomNo = 0;

    public bool Logging
    {
        get { return logging; }
        set { logging = value; }
    }

    public List<int> bossRooms;

    //   n
    //w     e
    //   s

	public Maze(int rX, int rY, int sX, int sY, int roomSX, int roomSY, int startingFloorsPrc)
	{
        bossRooms = new List<int>();
		roomsX = rX;
		roomsY = rY;
        sizeX = sX;
        sizeY = sY;
        roomSizeX = roomSX;
        roomSizeY = roomSY;
        sfp = startingFloorsPrc;
		maze = new int[roomsX*roomsY, roomsX*roomsY];
		mazeComplete = new int[roomsX * roomsY, roomsX * roomsY];

        CompleteClear();      
	}

    public void Generate()
    {
        for (int i = 0; i < roomsX; i++)
            for (int j = 0; j < roomsY; j++)
            {
                if (i != roomsX - 1)
                {
                    int daleko = UnityEngine.Random.Range(3, 660);
                    maze[VertexNum(i, j), VertexNum(i + 1, j)] = daleko;
                    maze[VertexNum(i + 1, j), VertexNum(i, j)] = daleko;
                }
                if (j != roomsY - 1)
                {
                    int daleko = UnityEngine.Random.Range(3, 660);
                    maze[VertexNum(i, j), VertexNum(i, j + 1)] = daleko;
                    maze[VertexNum(i, j + 1), VertexNum(i, j)] = daleko;
                }
            }

        PrimsMagic();
        placeStartEnd();
    }

    private void PrimsMagic()
    {
        int startVertexX = UnityEngine.Random.Range(0, roomsX);
        int startVertexY = UnityEngine.Random.Range(0, roomsY);
        int startVertex = VertexNum(startVertexX, startVertexY);

        int[] distances = new int[roomsX * roomsY];
        int[] daddy = new int[roomsX * roomsY];
        List<int> queue = new List<int>();
        for (int i = 0; i < roomsY * roomsX; i++)
        {
            distances[i] = Int32.MaxValue;
            daddy[i] = -666;
            queue.Add(i);
        }
        distances[startVertex] = 0;

        queue.Sort((v1, v2) => distances[v1].CompareTo(distances[v2]));

        int currentvertex = startVertex;
        while (queue.Count > 0)
        {
            currentvertex = queue[0];
            queue.RemoveAt(0);

            foreach (int neib in GetNeibours(currentvertex))
            {
                if (queue.Contains(neib))
                {
                    if (HowFar(currentvertex, neib) < distances[neib])
                    {
                        distances[neib] = HowFar(currentvertex, neib);
                        daddy[neib] = currentvertex;
                    }
                }
            }
            queue.Sort((v1, v2) => distances[v1].CompareTo(distances[v2]));

        }
        for (int i = 0; i < daddy.Length; i++)
        {
            int passageType = UnityEngine.Random.Range(1, 5);
            if (daddy[i] >= 0 && daddy[i] < daddy.Length)
            {
                mazeComplete[i, daddy[i]] = passageType;
                mazeComplete[daddy[i], i] = passageType;
            }
            Debug.Log("Edge "+i+" to "+daddy[i]+ " = "+passageType);
        }
    }

    private int VertexNum(int x, int y)
    { return x + roomsX * y; }

    private IntVector2 VertexCoords(int num)
    {
        int y = num / roomsX;
        int x = num - (y*roomsX);
        return new IntVector2(x, y);
    }

    private List<int> GetNeibours(int v)
    {
        List<int> sasiedzi = new List<int>();
        for (int i = 0; i < roomsX * roomsY; i++)
            if (maze[v, i] > 0 && i != v)
                sasiedzi.Add(i);
        return sasiedzi;
    }

    private int HowFar(int v1, int v2)
    {
        if (maze[v1, v2] == 0 || maze[v2, v1] == 0)
            return Int32.MaxValue;
        return (maze[v1, v2] + maze[v2, v1]) / 2;
    }

    private void CompleteClear()
    {
        for (int i = 0; i < roomsX * roomsY; i++)
            for (int j = 0; j < roomsY * roomsX; j++)
            {
                maze[i, j] = 0;
                mazeComplete[i, j] = 0;
            }
    }

    public List<Room> GetRooms()
    {
        List<Room> pokoje = new List<Room>();
        for (int i = 0; i < roomsX*roomsY; i++)
        {
            IntVector2 coords = VertexCoords(i);
            int n, s, e, w;
            if (coords.x <= 0)
                n = 0;
            else 
                n = mazeComplete[i,VertexNum(coords.x-1,coords.y)];
            if( coords.x >= roomsX - 1)
                s = 0;
            else
                s = mazeComplete[i,VertexNum(coords.x+1,coords.y)];

            if (coords.y <= 0)
                w = 0;
            else
                w = mazeComplete[i, VertexNum(coords.x, coords.y-1)];
            if (coords.y >= roomsY - 1)
                e = 0;
            else
                e = mazeComplete[i, VertexNum(coords.x, coords.y+1)];

            string newRoomName = "Room no. " + VertexNum(coords.x, coords.y).ToString() + " x:" + coords.x.ToString() + " y:" + coords.y.ToString();
            pokoje.Add(new Room(roomSizeX,roomSizeY,n,s,e,w,sfp,newRoomName,logging));
        }

        return pokoje;
    }

    private void placeStartEnd()
    {
        //Floyd-Warshall algorithm
        int daleko = UnityEngine.Random.Range(roomsX * roomsY+1024, Int32.MaxValue/2-1024);
        int[,] d = new int[roomsX*roomsY,roomsX*roomsY];
        int[,] poprzednik = new int[roomsX * roomsY, roomsX * roomsY];
        for (int i = 0; i < roomsX * roomsY; i++)
        {
            for (int j = 0; j < roomsX * roomsY; j++)
            {
                if (i == j)
                { 
                    d[i, j] = 0;
                    poprzednik[i, j] = -999;
                }
                else if (mazeComplete[i, j] > 0)
                { 
                    d[i, j] = 1;
                    poprzednik[i, j] = i;
                }
                else
                { 
                    d[i, j] = daleko;
                    poprzednik[i, j] = -999;
                }
            }
        }

        for (int k = 0; k < roomsX * roomsY; k++)
        {
            for (int i = 0; i < roomsX * roomsY; i++)
            {
                for (int j = 0; j < roomsX * roomsY; j++)
                {
                    if (d[i, j] > d[i, k] + d[k, j])
                    {
                        d[i, j] = d[i, k] + d[k, j];
                        poprzednik[i, j] = k;
                    }
                }
            }
        }

        int maxDistance = -daleko;
        int r1=-1;
        int r2=-1;
        for (int i = 0; i < roomsX * roomsY; i++)
        {
            for (int j = 0; j < roomsX * roomsY; j++)
            {
                if (logging && i !=j)
                {
                    Debug.Log("room " + i + " to " + j + " distance=" + d[i, j]);
                }
                if (d[i, j] > maxDistance && d[i,j] != 0)
                {
                    r1 = i;
                    r2 = j;
                    maxDistance = d[i, j];
                    
                }
            }
        }
        ShortestPathLength = maxDistance;
        if(UnityEngine.Random.Range(0f,1.0f) < 0.5f)
        {
            StartRoomNo = r2;
            EndRoomNo = r1;
        }
        else
        {
            StartRoomNo = r1;
            EndRoomNo = r2;
        }
        if (logging)
        {
            Debug.Log("start room=" + StartRoomNo);
            Debug.Log("end room=" + EndRoomNo);
        }
    }

    private void selectBossRooms()
    {
        IntVector2 endCords = VertexCoords(EndRoomNo);

        if (endCords.x == roomsX - 1 && endCords.y == roomsY - 1)
        {
            bossRooms.Add(EndRoomNo);
            bossRooms.Add(VertexNum(endCords.x - 1, endCords.y));
            bossRooms.Add(VertexNum(endCords.x - 1, endCords.y - 1));
            bossRooms.Add(VertexNum(endCords.x, endCords.y - 1));
        }
        else if (endCords.x == roomsX - 1)
        {
            bossRooms.Add(EndRoomNo);
            bossRooms.Add(VertexNum(endCords.x - 1, endCords.y));
            bossRooms.Add(VertexNum(endCords.x - 1, endCords.y + 1));
            bossRooms.Add(VertexNum(endCords.x, endCords.y + 1));
        }
        else if (endCords.y == roomsY - 1)
        {
            bossRooms.Add(EndRoomNo);
            bossRooms.Add(VertexNum(endCords.x + 1, endCords.y));
            bossRooms.Add(VertexNum(endCords.x + 1, endCords.y - 1));
            bossRooms.Add(VertexNum(endCords.x, endCords.y - 1));
        }
        else
        {
            bossRooms.Add(EndRoomNo);
            bossRooms.Add(VertexNum(endCords.x + 1, endCords.y));
            bossRooms.Add(VertexNum(endCords.x + 1, endCords.y + 1));
            bossRooms.Add(VertexNum(endCords.x, endCords.y + 1));
        }
        bossRooms.Sort();
        if (logging)
        {
            String b = "boss rooms:";
            foreach (int r in bossRooms)
                b += " " + r;
            Debug.Log(b);
        }

    }


}
