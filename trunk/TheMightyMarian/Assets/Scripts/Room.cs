using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Room {
    int[,] room;
    int n, s, w, e;

    int sizeX, sizeY;
    private int generations = 6;
    private int sfp;


    //   n
    //w     e
    //   s

    List<List<IntVector2>> nFreeTiles = new List<List<IntVector2>>();
    List<List<IntVector2>> sFreeTiles = new List<List<IntVector2>>();

    List<List<IntVector2>> wFreeTiles = new List<List<IntVector2>>();
    List<List<IntVector2>> eFreeTiles = new List<List<IntVector2>>();

    public Room(int sizeX, int sizeY, int north, int south, int east, int west, int startingFloorsPrc)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        room = new int[sizeX, sizeY];
        n = north;
        s = south;
        w = west;
        e = east;

        sfp = startingFloorsPrc;

        Prepare();
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
            temp.Clear();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeX; j++)
            {
                temp.Add(new IntVector2(sizeY - 1, i * passageSizeX + j));
                temp.Add(new IntVector2(sizeY - 2, i * passageSizeX + j));
            }
            sFreeTiles.Add(temp);
            temp.Clear();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeY; j++)
            {
                temp.Add(new IntVector2(i * passageSizeY + j, 0));
                temp.Add(new IntVector2(i * passageSizeY + j, 1));
            }
            wFreeTiles.Add(temp);
            temp.Clear();
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < passageSizeY; j++)
            {
                temp.Add(new IntVector2(i * passageSizeY + j, sizeX - 1));
                temp.Add(new IntVector2(i * passageSizeY + j, sizeX - 2));
            }
            eFreeTiles.Add(temp);
            temp.Clear();
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
            foreach (IntVector2 iv in eFreeTiles[e - 1])
            {
                room[iv.x, iv.y] = TileTypes.FLOOR;
            }
        }
    }

    private void Generate()
    {
        for (int i = 0; i < generations; i++)
        {
            if (i < 3)
            { CelluralStep1(); }
            else
            { CelluralStep2(); }
            PlaceDoors();
        }
    }

    private bool CheckIfAllPassagesExists()
    {
        bool passable = true;
        if (n != 0 && s != 0)
        { }
        if (n != 0 && w != 0)
        { }
        if (n != 0 && e != 0)
        { }
        if (s != 0 && w != 0)
        { }
        if (s != 0 && e != 0)
        { }
        if (w != 0 && e != 0)
        { }


        return passable;
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

    public void SaveBitmap(String filename = "kupa.png")
    {
        Texture2D obrazek = new Texture2D(sizeX * 6 + 1, sizeY * 6 + 1);
        for (int i = 0; i < sizeX * 6 + 1; i++)
        {
            for (int j = 0; j < sizeY * 6 + 1; j++)
            {
                obrazek.SetPixel(i, j, DawnBringer16.Blue);
            }
        }
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Color now;
                int ileftcorner = i * 6 + 1;
                int jleftcorner = j * 6 + 1;
                if (room[i, j] == TileTypes.FLOOR)
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
        for (int i = 0; i < sizeX * 6 + 1; i++)
        { obrazek.SetPixel(i, sizeY * 6, DawnBringer16.DarkGrey); }
        for (int i = 0; i < sizeY * 6 + 1; i++)
        { obrazek.SetPixel(sizeX * 6, i, DawnBringer16.DarkGrey); }

        var bytes = obrazek.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }

}
