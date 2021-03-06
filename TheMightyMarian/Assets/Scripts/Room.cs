﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Room {
    private int[,] room;
    int n, s, w, e;

    int sizeX, sizeY;
    private int generations = 6;
    private int sfp;
    private string name;
    private bool logging;
    public int this[int x,int y]
    {
        get
        {
            if (x >= 0 && y >= 0 && x < sizeX && y < sizeY)
            { return room[x, y]; }
            return -1;
        }
    }

    private bool finalFight = false;
    public bool FinalFight
    {
        get { return finalFight; }
        set { finalFight = value; }
    }
    //   n
    //w     e
    //   s

    List<List<IntVector2>> nFreeTiles = new List<List<IntVector2>>();
    List<List<IntVector2>> sFreeTiles = new List<List<IntVector2>>();

    List<List<IntVector2>> wFreeTiles = new List<List<IntVector2>>();
    List<List<IntVector2>> eFreeTiles = new List<List<IntVector2>>();

    private int[,] colorfullRoom;

    public Room(int sizeX, int sizeY, int north, int south, int east, int west, int startingFloorsPrc,string roomName,bool log)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        room = new int[sizeX*2, sizeY*2];
        n = north;
        s = south;
        w = west;
        e = east;

        sfp = startingFloorsPrc;
        colorfullRoom = new int[sizeX,sizeY];
        name = roomName;
        logging = log;
        //Prepare();
    }

    public void Prepare()
    {
        FillWithVoid();
        FillRandomly();
        CalculateDoors();
        PlaceDoors();
    }

    private void FillWithVoid()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                room[i, j] = TileTypes.VOID;
            }
        }
    }

    private void FillWithFloors()
    {
        for (int i = 1; i < sizeX-1; i++)
        {
            for (int j = 1; j < sizeY-1; j++)
            {
                room[i, j] = TileTypes.FLOOR;
            }
        }
    }

    private void FillRandomly()
    {
        for (int x = 1; x < sizeX - 1; x++)
        {
            for (int y = 1; y < sizeY - 1; y++)
            {
                if (UnityEngine.Random.Range(0, 101) <= sfp)
                    room[x, y] = TileTypes.FLOOR;
            }
        }
    }

    private void CalculateDoors()
    {
        int sx = sizeX;
        int sy = sizeY;

        while (sx % 4 != 0)
        {
            sx -= 2;
        }
        while (sy % 4 != 0)
        {
            sy -= 2;
        }
        int passageSizeX = sx / 4;
        int passageSizeY = sy / 4;

        int shiftX = sizeX - sx;
        int shiftY = sizeY = sy;


        List<IntVector2> temp = new List<IntVector2>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeX; j++)
            {
                temp.Add(new IntVector2(0, i * passageSizeX + j));
                temp.Add(new IntVector2(1, i * passageSizeX + j));
            }
            nFreeTiles.Add(temp);
            //temp.Clear();
            temp = new List<IntVector2>();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeX; j++)
            {
                temp.Add(new IntVector2(sizeY - 1, i * passageSizeX + j));
                temp.Add(new IntVector2(sizeY - 2, i * passageSizeX + j));
            }
            sFreeTiles.Add(temp);
            //temp.Clear();
            temp = new List<IntVector2>();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeY; j++)
            {
                temp.Add(new IntVector2(i * passageSizeY + j, 0));
                temp.Add(new IntVector2(i * passageSizeY + j, 1));
            }
            wFreeTiles.Add(temp);
            //temp.Clear();
            temp = new List<IntVector2>();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeY; j++)
            {
                temp.Add(new IntVector2(i * passageSizeY + j, sizeX - 1));
                temp.Add(new IntVector2(i * passageSizeY + j, sizeX - 2));
            }
            eFreeTiles.Add(temp);
            //temp.Clear();
            temp = new List<IntVector2>();
        }

    }

    private void PlaceDoors()
    {
        if (n != 0)
        {
            foreach (IntVector2 iv in nFreeTiles[n - 1])
            {
                room[iv.x, iv.y] = TileTypes.FLOOR;
            }
        }
        if (s != 0)
        {
            foreach (IntVector2 iv in sFreeTiles[s - 1])
            {
                room[iv.x, iv.y] = TileTypes.FLOOR;
            }
        }
        if (w != 0)
        {
            foreach (IntVector2 iv in wFreeTiles[w - 1])
            {
                room[iv.x, iv.y] = TileTypes.FLOOR;
            }
        }
        if (e != 0)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    room[eFreeTiles[e - 1][i].x, eFreeTiles[e - 1][i].y] = TileTypes.FLOOR;
            //}
            foreach (IntVector2 iv in eFreeTiles[e - 1])
            {
                room[iv.x, iv.y] = TileTypes.FLOOR;
            }
        }
    }

    public void Generate()
    {
        _Generate();
        while (!CheckIfAllPassagesExists() || (cntPrc() < 0.3f))
        {
            if (logging)
            {
                Debug.Log(name + " not passable sfp:" + sfp.ToString());
            }
            sfp++;
            Prepare();
            _Generate();
        }
        if (logging)
        { Debug.Log(name + " generated. sfp:" + sfp.ToString() + " %:" + cntPrc().ToString()); }
    }

    private bool _Generate()
    {
        int[,] smallRoom = new int[sizeX, sizeY];
        for (int i = 0; i < generations; i++)
        {
            if (i < 4 && !finalFight)
            { CelluralStep1(); }
            else
            { CelluralStep2(); }
            PlaceDoors();
        }
        return true;
    }

    private bool CheckIfAllPassagesExists()
    {
        //bool alreadyFlooded = false;
        int start_color = UnityEngine.Random.Range(66, 666);
        int color = start_color;
        for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                if (room[i, j] == TileTypes.FLOOR)
                { colorfullRoom[i, j] = 1; }
                else
                { colorfullRoom[i, j] = 0; }
            }

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (colorfullRoom[i, j] == 1)
                {
                    floodFill(i, j, color);
                    color++;
                }
            }
        }
        return (start_color - color == -1);
    }

    private float cntPrc()
    {
        float result = 0.0f;
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (room[i, j] == TileTypes.FLOOR)
                { 
                    result += 1.0f;
                }
            }
        }
        result = result / (float)(sizeX * sizeY);

        return result;
    }

    void floodFill(int x, int y, int color)
    {
        colorfullRoom[x, y] = color;
        if (isFineCoords(x + 1, y))
        {
            if (colorfullRoom[x + 1, y] == 1)
            { floodFill(x + 1, y, color); }
        }
        if (isFineCoords(x - 1, y))
        {
            if (colorfullRoom[x - 1, y] == 1)
            { floodFill(x - 1, y, color); }
        }
        if (isFineCoords(x, y + 1))
        {
            if (colorfullRoom[x, y + 1] == 1)
            { floodFill(x, y + 1, color); }
        }
        if (isFineCoords(x, y - 1))
        {
            if (colorfullRoom[x, y - 1] == 1)
            { floodFill(x, y - 1, color); }
        }
    }

    private void CelluralStep2()
    {
        List<IntVector2> flors = new List<IntVector2>();
        List<IntVector2> wals = new List<IntVector2>();
        IntVector2 current;
        int walls = 0;

        for (int x = 1; x < sizeX-1; x++)
        {
            for (int y = 1; y < sizeY-1; y++)
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
            room[c.x, c.y] = TileTypes.VOID;
        }
        wals.Clear();
        foreach (IntVector2 c in flors)
        {
            room[c.x, c.y] = TileTypes.FLOOR;

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

        for (int x = 1; x < sizeX-1; x++)
        {
            for (int y = 1; y < sizeY-1; y++)
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
        { room[c.x, c.y] = TileTypes.VOID; }
        wals.Clear();
        foreach (IntVector2 c in flors)
        { room[c.x, c.y] = TileTypes.FLOOR; }
        flors.Clear();
    }

    private int CntCellNeighboursWalls(int x, int y)
    {
        int n = 0;

        if (room[x + 1, y] == TileTypes.VOID)
            n++;
        if (room[x - 1, y] == TileTypes.VOID)
            n++;
        if (room[x, y + 1] == TileTypes.VOID)
            n++;
        if (room[x, y - 1] == TileTypes.VOID)
            n++;
        if (room[x + 1, y + 1] == TileTypes.VOID)
            n++;
        if (room[x - 1, y - 1] == TileTypes.VOID)
            n++;
        if (room[x + 1, y - 1] == TileTypes.VOID)
            n++;
        if (room[x - 1, y + 1] == TileTypes.VOID)
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
                    if (isFineCoords(x + i, y + j))
                    {
                        if (room[x + i, y + j] == TileTypes.VOID)
                            n++;
                    }
                }
            }
        }
        return n;
    }

    private bool isFineCoords(int x, int y)
    {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
        { return true; }
        return false;
    }
}
