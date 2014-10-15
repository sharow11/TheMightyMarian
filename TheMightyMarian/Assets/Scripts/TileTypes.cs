using UnityEngine;
using System.Collections;

static public class TileTypes {

    static private int wwater = 1;
    static private int vvoid = 0; //0
    static private int ggrass = 2;
    static private int wwall = 3;
    static private int ffloor = 4; //4

    public static int WATER
    {
        get { return wwater; }
    }
    public static int VOID
    {
        get { return vvoid; }
    }
    public static int GRASS
    {
        get { return ggrass; }
    }
    public static int WALL
    {
        get { return wwall; }
    }
    public static int FLOOR
    {
        get { return ffloor; }
    }
}
