﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct IntVector2 
{
    public int x, y;

    public IntVector2(int x, int z) {
        this.x = x;
        this.y = z;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    public double distanceTo(IntVector2 a)
    {
        return Mathf.Sqrt(((this.x - a.x) * (this.x - a.x)) + ((this.y - a.y) * (this.y - a.y)));
    }
}
