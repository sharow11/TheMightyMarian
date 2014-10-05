using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace DuzeIlosciNaRazMapek
{
    public class MapGenerator
    {
        Map map;
        int N, M, continets, continentSize, continentVariation, celluarCycles;

        Random random;

        List<IntVector2> queue;
        int currentContinentSize = 0;

        public MapGenerator(int n, int m, int ik, int rk, int zr, int cc)
        {
            map = new Map(n, m);
            this.N = n; this.M = m;
            this.continets = ik;
            this.continentSize = rk; this.continentVariation = zr;
            this.celluarCycles = cc;
            random = new Random();
        }

        public bool Generate()
        {
            int[] direction = { 1, 2, 3, 4, 5, 6, 7 };
            fillWithGrass();
            return true;
        }

        public Bitmap getBitmap()
        {
            map.generateBitMap();
            return map.getBitmap();
        }

        private void fillWithGrass()
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    map[i, j] = Tile.GRASS;
        }
    }

}

