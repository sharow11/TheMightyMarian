#include <iostream>
#include <vector>
#include <stdlib.h>
#include <time.h>
#include <string>

using namespace std;

#define XSIZE 80
#define YSIZE 68
#define WALL_PROCENT 49
#define STEPS_CNT 15

#define WERID_PROCENT 5

#define WALL 8
#define FLOOR 0

struct point {
  int x;
  int y;
} ;

int map[XSIZE][YSIZE];

void fillRandomly()
{
	srand(time(NULL));
	for(int i=0; i<XSIZE; i++)
	{
		for(int j=0; j<YSIZE; j++)
		{
			if((rand()%100) < WALL_PROCENT)
				map[i][j]=WALL;
			else
				map[i][j]=FLOOR;
		}
	}
}

void fillBorders()
{
	for(int i=0; i<XSIZE; i++)
	{
		map[i][0]=WALL;
		map[i][YSIZE-1]=WALL;
	}
	for (int i = 0; i < YSIZE; i++)
	{
		map[0][i]=WALL;
		map[XSIZE-1][i]=WALL;
	}
}

int cntNeighbours(int i, int j)
{
	int x=0;
	if(map[i+1][j]==WALL)
		x++;
	if(map[i-1][j]==WALL)
		x++;
	if(map[i][j+1]==WALL)
		x++;
	if(map[i][j-1]==WALL)
		x++;
	if(map[i+1][j+1]==WALL)
		x++;
	if(map[i-1][j-1]==WALL)
		x++;
	if(map[i+1][j-1]==WALL)
		x++;
	if(map[i-1][j+1]==WALL)
		x++;
	return x;
}

int evolve()
{
	vector<point> toChange;
	point current;
	//fillBorders();
	int walls=0;
	int floors=0;
	for(int i=1; i<XSIZE-1; i++)
		for(int j=1; j<YSIZE-1; j++)
		{
			walls = cntNeighbours(i,j);
			floors = 8- walls;
			current.x = i;
			current.y = j;

			if(map[i][j]==WALL && floors >= 5)
				toChange.push_back(current);
			else if(map[i][j]==FLOOR && walls >= 5)
				toChange.push_back(current);		
		}

		for(int i=0; i<toChange.size(); i++)
		{
			if(map[toChange[i].x][toChange[i].y] == FLOOR)
				map[toChange[i].x][toChange[i].y] = WALL;
			else
				map[toChange[i].x][toChange[i].y] = FLOOR;
		}

		return (toChange.size());
}

void printMap()
{
	string line = "";
	
	for(int i=0; i<XSIZE; i++)
	{
		for(int j=0; j<YSIZE; j++)
		{
			if(map[i][j]==WALL)
				line+="#";
			else
				line+=".";
		}
		cout << line << endl;
		line = "";
	}
}

bool validCords(int x, int y)
{
	if(x >= 0 && y>= 0 && x<XSIZE && y<YSIZE)
		return true;
	return false;
}

//void drunkenWalk(int x, int y, int len, int terrain)
//{
//	int xprev=x;
//	int yprev=y;
//	int xnext=x;
//	int ynext=y;
//	int rprev=78;
//	int r; 
//	for(int i=0; i<len+1; i++)
//	{
//		map[xprev][yprev]=terrain;
//		xnext = xprev; ynext = yprev;
//		rprev = r;
//		r =rand()%3;
//		if(rprev == 0)
//
//		switch (r)
//		{
//			case 0:
//				xnext++;
//				break;
//			case 1:
//				xnext--;
//				break;
//			case 2:
//				ynext++;
//				break;
//			case 3:
//				ynext--;
//				break;
//		}
//		if(xprev == xn
//		while(xprev == xnext && yprev == ynext || !validCords(xprev, yprev))
//		{
//			xnext = xprev; ynext = yprev;
//			r =rand()%4;
//			switch (r)
//			{
//				case 0:
//					xnext++;
//					break;
//				case 1:
//					xnext--;
//					break;
//				case 2:
//					ynext++;
//					break;
//				case 3:
//					ynext--;
//					break;
//			}
//		}
//		xprev = xnext; yprev = ynext;
//	}
//	map[xprev][yprev]=terrain;
//
//}

int floorCnt()
{
	int cnt =0;
	for(int i=0; i<XSIZE; i++)
	{
		for(int j=0; j<YSIZE; j++)
		{
			if(map[i][j]==FLOOR)
				cnt++;
		}
	}
	return cnt;
}

int main()
{
	fillRandomly();
	fillBorders();

	for(int i=0; i<STEPS_CNT; i++)
	{
		evolve();
		//if(rand()%100 <= 5)
		//	drunkenWalk((rand()%(XSIZE-2))+1,(rand()%(YSIZE-2))+1,rand()%XSIZE,WALL);
	}

	printMap();

	return 0;
}