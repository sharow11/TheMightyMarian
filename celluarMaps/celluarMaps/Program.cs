using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace celluarMaps
{
    class Program
    {
        static void Main(string[] args)
        {
            MapGenerator mapka = new MapGenerator(78,78,700);
            mapka.printWorld();
        }

    }
}
