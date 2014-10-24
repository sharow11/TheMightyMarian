using UnityEngine;
using System.Collections;


public static class DawnBringer16
{
	private static Color ConvertColor(int r,int g,int b)
	{
		return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
	}
    public static Color Black
    {
        get { return ConvertColor(20, 12, 28); }
    }
	public static Color Kupa
	{
		get { return ConvertColor(20, 12, 28); }
	}
    public static Color Brown
    {
        get { return ConvertColor(68, 36, 52); }
    }
    public static Color Navy
    {
        get { return ConvertColor(48, 52, 109); }
    }
    public static Color DarkGrey
    {
        get { return ConvertColor(78, 74, 78); }
    }
    public static Color LightBrown
    {
        get { return ConvertColor(133, 76, 48); }
    }
    public static Color DarkGreen
    {
        get { return ConvertColor(52, 101, 36); }
    }
    public static Color Red
    {
        get { return ConvertColor(208, 70, 72); }
    }
    public static Color Gray
    {
        get { return ConvertColor(117, 113, 97); }
    }
    public static Color SkyBlue
    {
        get { return ConvertColor(89, 125, 206); }
    }
    public static Color Orange
    {
        get { return ConvertColor(210, 125, 44); }
    }
    public static Color Grey
    {
        get { return ConvertColor(133, 149, 161); }
    }
    public static Color Green
    {
        get { return ConvertColor(109, 170, 44); }
    }
    public static Color PinkBeige
    {
        get { return ConvertColor(210, 170, 153); }
    }
    public static Color Blue
    {
        get { return ConvertColor(109, 194, 202); }
    }
    public static Color Yellow
    {
        get { return ConvertColor(218, 212, 94); }
    }
    public static Color White
    {
        get { return ConvertColor(222, 238, 214); }
    }
}



