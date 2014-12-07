using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.Scripts
{
    interface IMarianMap
    {
        int SizeX
        { get; }
        int SizeY
        { get; }
        int RoomsX {get;}
        int RoomsY { get; }
        bool Logging
        {
            get;
            set;
        }
        void Generate();
        IntVector2 PlaceEnemyInRoom(int room);
        IntVector2 GetEndLadderPos();
        IntVector2 GetStartPosForPlayer();

        int this[int x, int y]
        { get; }

        int ShortestPathLength
        { get; }

        int LvlNo
        {
            get;
            set;
        }
        int StartRoomNo
        {
            get;
            set;
        }
        int EndRoomNo
        {
            get;
            set;
        }

        GameObject gameObject
        { get; }
        
    }
}
