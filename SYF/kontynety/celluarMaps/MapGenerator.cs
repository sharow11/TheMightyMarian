using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;
using System.Diagnostics;





// ...




namespace celluarMaps
{
    public class MapGenerator
    {
        int N = 2000;
        int M = 1500;
        int p = 75; //100% to 1000

        int[,] map;
        ConsoleColor[] colors = new ConsoleColor[3];
        Random random;

        const int emptyness = 0;
        const int water = 1;
        const int ground = 2;

        float eproc =0;
        float wproc =0;
        float gproc =0;

        const int continents = 7;
        const int maxSize = 400;
        const float standardDiff = 0.5F;

        int dumped = 0;

        List<int> direction = new List<int>();
        Stopwatch sw = new Stopwatch();

        //Queue<PointInContinent> kolejeczka = new Queue<PointInContinent>();
        List<PointInContinent> kolejeczka = new List<PointInContinent>();
        List<PointInContinent> startingPoints = new List<PointInContinent>();
        int continentSize = 0;

        double pathsTime = 0;

        public MapGenerator(int N, int M, int p)
        {
            this.N = N;
            this.M = M;
            this.p = p*10;
            map = new int[N, M];
            fillWithColors();
            random = new Random();
            //fillMap();
            //simplexNoise();
            //celluarAutomata(3);

            for (int i = 0; i < 8; i++)
            { direction.Add(i); }

            
            for (dumped = 0; ; dumped++)
            {
                sw.Reset();
                sw.Start();
                if (start())
                {
                    sw.Stop();
                    pathsTime = sw.Elapsed.TotalMilliseconds;
                    sw.Start();
                    joinWithPaths();
                    sw.Stop();
                    pathsTime = sw.Elapsed.TotalMilliseconds - pathsTime;
                    break; 
                }
                sw.Stop();
                printWorld();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This map is shitty, starting again");
                Console.ResetColor();
                if (dumped == 50)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This wont work, already tried 50 times");
                    Console.ResetColor();
                    break;
                }
                
            }
        }

        bool landOfOoo()
        {
            int x, y;

            for (int c = 0; c < continents; c++)
            {
                continentSize = 0;
                kolejeczka.Clear();

                int mytilenb = 100 + c;
                x = random.Next(1, N - 1);
                y = random.Next(1, M - 1);
                int i;
                for (i = 0; i < 50 && !checkStartingPoint(x, y); i++)
                {
                    x = random.Next(1, N - 1);
                    y = random.Next(1, M - 1);
                }
                if(i == 49 && !checkStartingPoint(x,y))
                { return false; }

                PointInContinent pic = new PointInContinent();
                pic.x = x;
                pic.y = y;
                startingPoints.Add(pic);

                map[x, y] = mytilenb;
                continentSize++;
                direction.Shuffle();
                foreach (int dir in direction)
                {
                    switch (dir)
                    {
                        case 0:
                        {
                            if(checkIfFree(x-1,y-1,mytilenb))
                            { markAndAddToQueue(x - 1, y - 1,mytilenb); }
                            break;
                        }
                        case 1:
                        {
                            if (checkIfFree(x - 1, y,mytilenb))
                            { markAndAddToQueue(x - 1, y, mytilenb); }
                            break;
                        }
                        case 2:
                        {
                            if (checkIfFree(x - 1, y + 1,mytilenb))
                            { markAndAddToQueue(x - 1, y + 1, mytilenb); }
                            break;
                        }
                        case 3:
                        {
                            if (checkIfFree(x, y - 1,mytilenb))
                            { markAndAddToQueue(x, y - 1, mytilenb); }
                            break;
                        }
                        case 4:
                        {
                            if (checkIfFree(x + 1, y - 1,mytilenb))
                            { markAndAddToQueue(x + 1, y - 1, mytilenb); }
                            break;
                        }
                        case 5:
                        {
                            if (checkIfFree(x + 1, y + 1,mytilenb))
                            { markAndAddToQueue(x + 1, y + 1, mytilenb); }
                            break;
                        }
                        case 6:
                        {
                            if (checkIfFree(x, y + 1,mytilenb))
                            { markAndAddToQueue(x, y + 1, mytilenb); }
                            break;
                        }
                        case 7:
                        {
                            if (checkIfFree(x + 1, y,mytilenb))
                            { markAndAddToQueue(x + 1, y, mytilenb); }
                            break;
                        }
                    }
                }

                bool result = runQueue(mytilenb);
                if (result == false)
                { return result; }
            }

            return true;
        }

        bool runQueue(int tilenb)
        {
            while (continentSize < maxSize && kolejeczka.Count != 0)
            {
                //kolejeczka = (Stack<PointInContinent>)kolejeczka.OrderBy(a => Guid.NewGuid());
                PointInContinent pic = kolejeczka[random.Next(0, kolejeczka.Count - 1)];
                kolejeczka.Remove(pic);
                
                processPointNeibours(pic.x, pic.y, tilenb);
                direction.Shuffle();
                //printWorld();
            }
            if ((float)continentSize >= maxSize * standardDiff)
            { return true; }
            else
            { return false; }
        }

        void processPointNeibours(int x, int y, int tilenb)
        { 
                direction.Shuffle();
                foreach (int dir in direction)
                {
                    switch (dir)
                    {
                        case 0:
                        {
                            if(checkIfFree(x-1,y-1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x - 1, y - 1,tilenb);}
                            }
                            break;
                        }
                        case 1:
                        {
                            if (checkIfFree(x - 1, y,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x - 1, y, tilenb); }
                            }
                            break;
                        }
                        case 2:
                        {
                            if (checkIfFree(x - 1, y + 1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x - 1, y + 1, tilenb); }
                            }
                            break;
                        }
                        case 3:
                        {
                            if (checkIfFree(x, y - 1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x, y - 1, tilenb); }
                            }
                            break;
                        }
                        case 4:
                        {
                            if (checkIfFree(x + 1, y - 1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x + 1, y - 1, tilenb); }
                            }
                            break;
                        }
                        case 5:
                        {
                            if (checkIfFree(x + 1, y + 1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x + 1, y + 1, tilenb); }
                            }
                            break;
                        }
                        case 6:
                        {
                            if (checkIfFree(x, y + 1,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x, y + 1, tilenb); }
                            }
                            break;
                        }
                        case 7:
                        {
                            if (checkIfFree(x + 1, y,tilenb))
                            {
                                if (random.Next(0, 100) <= p)
                                { markAndAddToQueue(x + 1, y, tilenb); }
                            }
                            break;
                        }
                    }
                }
            }
        
        bool checkIfFree(int i,int j,int tilenb)
        {
            if (map[i, j] != water)
            { return false; }
            else if (map[i - 1, j - 1] != water && map[i - 1, j - 1] != tilenb)
            { return false; }
            else if (map[i -1, j] != water && map[i- 1, j] != tilenb)
            { return false; }
            else if (map[i - 1, j + 1] != water && map[i - 1, j + 1] != tilenb)
            { return false; }
            else if (map[i, j - 1] != water && map[i, j - 1] != tilenb)
            { return false; }
            else if (map[i + 1, j - 1] != water && map[i + 1, j - 1] != tilenb)
            { return false; }
            else if (map[i + 1, j + 1] != water && map[i + 1, j + 1] != tilenb)
            { return false; }
            else if (map[i, j + 1] != water && map[i, j + 1] != tilenb)
            { return false; }
            else if (map[i + 1, j] != water && map[i + 1, j] != tilenb)
            { return false; }

            return true;
        }

        bool checkStartingPoint(int i, int j)
        {
            if (map[i, j] != water)
            { return false; }
            else if (map[i-1,j-1] != water || map[i-1,j] != water || map[i-1,j+1] != water)
            { return false; }
            else if (map[i, j - 1] != water || map[i + 1, j - 1] != water || map[i + 1, j + 1] != water)
            { return false; }
            else if (map[i, j + 1] != water || map[i + 1, j] != water)
            { return false; }
            return true;
        }

        bool start()
        {
            startingPoints.Clear();
            kolejeczka.Clear();
            fillMap();
            return landOfOoo();
        }

        void markAndAddToQueue(int x, int y, int tilenb)
        {
            PointInContinent poi = new PointInContinent();
            poi.x = x;
            poi.y = y;
            kolejeczka.Add(poi);
            map[x, y] = tilenb;
            continentSize++;
        }


        void joinWithPaths()
        {
            List<PointInContinent> nonEvaluatedPoints = new List<PointInContinent>();
            for (int i = 0; i < startingPoints.Count; i++)
            { nonEvaluatedPoints.Add(startingPoints[i]); }
            List<PointInContinent> EvaluatedPoints = new List<PointInContinent>();

            Dictionary<PointInContinent, int> howFar = new Dictionary<PointInContinent, int>();

            PointInContinent root = nonEvaluatedPoints[random.Next(0, startingPoints.Count)];
        }

        float cntDistance(PointInContinent a, PointInContinent b)
        {
            double c = 0F;
            c = (a.x - b.x) * (a.x - b.x) + (a.y - b.y)*(a.y - b.y);
            c = Math.Sqrt(c);
            return (float)c;
        }

        void fillMap()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                { map[i, j] = emptyness; }
            }
            for (int i = 1; i < N - 1; i++)
            {
                for (int j = 1; j < M - 1; j++)
                { map[i, j] = water; }
            }
        }

        public void printWorld()
        {
            PointInContinent pic = new PointInContinent();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    pic.x = i;
                    pic.y = j;
                    if (startingPoints.Contains(pic))
                    { Console.ForegroundColor = ConsoleColor.Red; }
                    else if (map[i, j] < 100)
                    { Console.ForegroundColor = colors[map[i, j]]; }
                    else
                    { Console.ForegroundColor = colors[ground]; }
                    Console.Write((char)0x2588);
                }
                Console.Write("\n");
            }

            cntPrc();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Stats: Water [" + wproc.ToString("0.00") + "%] Ground [" + gproc.ToString("0.00") + "%] Void [" + eproc.ToString("0.00") + "%] Dumped maps [" + dumped.ToString() + "]");
            Console.WriteLine("Generated in {0} miliseconds, Paths generated in {1} miliseconds", sw.Elapsed.TotalMilliseconds, pathsTime);
            Console.ResetColor();
        }

        void cntPrc()
        {
            int e = 0; int w = 0; int g = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    switch (map[i, j])
                    {
                        case water:
                            {
                                w++;
                                break;
                            }
                        case ground:
                            {
                                g++;
                                break;
                            }
                        case emptyness:
                            {
                                e++;
                                break;
                            }
                    }
                    if (map[i, j] >= 100)
                    { g++; }
                }
            }

            eproc = (((float)e) / ((float)N * M)) * (float)100.0;
            wproc = (((float)w) / ((float)N * M)) * (float)100.0;
            gproc = (((float)g) / ((float)N * M)) * (float)100.0;

        }

        void fillWithColors()
        {
            colors[0] = ConsoleColor.DarkGray;
            colors[1] = ConsoleColor.Blue;
            colors[2] = ConsoleColor.DarkGreen;
            
        }

        void simplexNoise()
        { }

        void celluarAutomata(int iterations)
        {
            for (int iter = 0; iter < iterations; iter++)
            { }
        }
    }

    struct PointInContinent
    {
        public int x;
        public int y;
    }
}
