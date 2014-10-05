using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DuzeIlosciNaRazMapek
{
    public class Map
    {
        private int[,] map;
        Bitmap prettyPicture;
        int N, M;

        public Map(int N, int M)
        { 
            this.N = N;
            this.M = M;
            map = new int[N, M];
            prettyPicture = new Bitmap(N, M);
        }

        public Bitmap getBitmap()
        { return prettyPicture; }

        public int this[int x,int y]
        {
            get
            { return map[x,y]; }
            set
            { map[x,y] = value; }
        }

         public void generateBitMap()
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                { prettyPicture.SetPixel(i,j,Tile.TileColor(map[i,j]));}
        }


    }
}
