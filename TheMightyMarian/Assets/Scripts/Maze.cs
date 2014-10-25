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

    //   n
    //w     e
    //   s

	public Maze(int rX, int rY, int sX, int sY, int roomSX, int roomSY, int startingFloorsPrc)
	{
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
        //MazeClear();
        //MazeCompleteClear();

        for (int i = 0; i < roomsX; i++)
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
                    maze[VertexNum(i, j), VertexNum(i + 1, j)] = daleko;
                    maze[VertexNum(i + 1, j), VertexNum(i, j)] = daleko;
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
                    maze[VertexNum(i, j), VertexNum(i, j + 1)] = daleko;
                    maze[VertexNum(i, j + 1), VertexNum(i, j)] = daleko;
                }
            }

        PrimsMagic();
        ToImage("images/mazes/" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png");
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


        //visited.Add(startVertex);
        //queue.Sort((v1, v2) => MazeHowFar(startVertex, v1).CompareTo(MazeHowFar(startVertex, v2)));
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

    private void ToImage(String filename = "maze.png")
    {
        //int XX = roomsX * 4 + (roomsX-1)*4 + 2;
        //int YY = roomsY * 4 + (roomsY-1)*4 + 2;
        int XX = (4 * roomsX) + 2;
        int YY = (4 * roomsY) + 2;
        Texture2D obrazek = new Texture2D(XX, YY);

        for (int i = 0; i < XX; i++)
        {
            for (int j = 0; j < YY; j++)
            {
                obrazek.SetPixel(i, j, DawnBringer16.Black);
            }
        }

        for (int i = 0; i < roomsX; i++)
        {
            for (int j = 0; j < roomsY; j++)
            {
                for (int k = 4 * i; k < 4 * i + 2; k++)
                    for (int l = 4 * j; l < 4 * j + 2; l++)
                        obrazek.SetPixel(k + 2, l + 2, DawnBringer16.White);

                //obrazek.SetPixel(4 * i + 1, 4 * j, DawnBringer16.White);
                //obrazek.SetPixel(4 * i, 4*j+1, DawnBringer16.White);
                //obrazek.SetPixel(4 * i + 1, 4 * j + 1, DawnBringer16.White);

                Color current;
                if (i + 1 < roomsX)
                    current = ColorForPassage(mazeComplete[VertexNum(i, j), VertexNum(i + 1, j)]);
                else
                    current = DawnBringer16.Black;

                for (int k = 4 * i + 2; k < 4 * i + 4; k++)
                    for (int l = 4 * j; l < 4 * j + 2; l++)
                        obrazek.SetPixel(k + 2, l + 2, current);

                if (j + 1 < roomsY)
                    current = ColorForPassage(mazeComplete[VertexNum(i, j), VertexNum(i, j + 1)]);
                else
                    current = DawnBringer16.Black;
                for (int k = 4 * i; k < 4 * i + 2; k++)
                    for (int l = 4 * j + 2; l < 4 * j + 4; l++)
                        obrazek.SetPixel(k + 2, l + 2, current);
                current = DawnBringer16.Black;
                for (int k = 4 * i + 2; k < 4 * i + 4; k++)
                    for (int l = 4 * j + 2; l < 4 * j + 4; l++)
                        obrazek.SetPixel(k + 2, l + 2, current);
            }
        }
        var bytes = obrazek.EncodeToPNG();

        File.WriteAllBytes(filename, bytes);

    }

    private Color ColorForPassage(int p)
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
            pokoje.Add(new Room(roomSizeX,roomSizeY,n,s,e,w,sfp,newRoomName));
        }

        return pokoje;
    }


}
