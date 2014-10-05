using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DuzeIlosciNaRazMapek
{
    public static class Tile
    {
        
        public static int WATER
        { get { return 1; } }
        public static int VOID
        { get { return 0; } }
        public static int GRASS
        { get { return 2; } }
        public static int SAND
        { get { return 3; } }
        public static int DEEPWATER
        { get { return 4; }}
        public static int SPECIALPOINT
        { get { return 5; } }

        private static Color[] colorTable  = new Color[]  { DawnBringer16.Black, DawnBringer16.SkyBlue, DawnBringer16.Green, DawnBringer16.Yellow, DawnBringer16.Navy, DawnBringer16.Red};

        public static Color TileColor(int x)
        {
            if (x < colorTable.Length)
            { return colorTable[x]; }
            return DawnBringer16.Red;
        }
    }
}
