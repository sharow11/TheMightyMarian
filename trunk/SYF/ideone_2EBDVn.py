import random
import sys
import operator

sizex = 100;
sizey = 100;
graph = list()
msp = list()
edgesW = {}

def vertexNum(x,y):
    return  x + sizex*y

def printGraph():
    line =''
    for i in range (0, sizex*2+1):
        line+='X'
    line+='\n'
    print line,
    line = ''

    for j in range (0, sizey):
        line+='X'
        for i in range (0,sizex):
            line+=' '
            if(i!=sizex-1 and vertexNum(i+1, j) in graph[vertexNum(i,j)]):
                line+=' '
            else:
                line+='X'
        line+='\n'
        print line,
        line=''

        line +='X'
        for i in range (0, sizex):
            if(j!=sizey-1 and vertexNum(i, j+1) in graph[vertexNum(i,j)]):
                line+=' '
            else:
                line+='X'
            line+='X'
        line+='\n'
        print line,
        line =''

def printMsp():
    line =''
    for i in range (0, sizex*2+1):
        line+='X'
    line+='\n'
    print line,
    line = ''

    for j in range (0, sizey):
        line+='X'
        for i in range (0,sizex):
            line+=' '
            if(i!=sizex-1 and vertexNum(i+1, j) in msp[vertexNum(i,j)]):
                line+=' '
            else:
                line+='X'
        line+='\n'
        print line,
        line=''

        line +='X'
        for i in range (0, sizex):
            if(j!=sizey-1 and vertexNum(i, j+1) in msp[vertexNum(i,j)]):
                line+=' '
            else:
                line+='X'
            line+='X'
        line+='\n'
        print line,
        line =''

def howfar(v1, v2):
    if v1 in graph[v2]:
        return (edgesW[v1,v2]+edgesW[v2,v1])/2    
    else:
        return sys.maxint

#populate graph
for j in range (0, sizey):
    for i in range (0,sizex):
        myneigbourlist= list()
        if i!=0:
            myneigbourlist.append(vertexNum(i-1, j))
            edgesW[vertexNum(i-1,j),vertexNum(i,j)]=random.randint(3,10)
            edgesW[vertexNum(i,j),vertexNum(i-1,j)]=edgesW[vertexNum(i-1,j),vertexNum(i,j)]
        if i!=sizex-1:
            myneigbourlist.append(vertexNum(i+1, j))
            edgesW[(vertexNum(i+1,j),vertexNum(i,j))]=random.randint(3,10)
            edgesW[(vertexNum(i,j),vertexNum(i+1,j))]=edgesW[(vertexNum(i+1,j),vertexNum(i,j))]
        if j!=0:
            myneigbourlist.append(vertexNum(i, j-1))
            edgesW[(vertexNum(i,j-1),vertexNum(i,j))] = random.randint(3,10)
            edgesW[(vertexNum(i,j),vertexNum(i,j-1))] = edgesW[(vertexNum(i,j-1),vertexNum(i,j))] 

        if j!=sizey-1:
            myneigbourlist.append(vertexNum(i, j+1))
            edgesW[(vertexNum(i,j+1),vertexNum(i,j))] = random.randint(3,10)
            edgesW[(vertexNum(i,j),vertexNum(i,j+1))] = edgesW[(vertexNum(i,j+1),vertexNum(i,j))] 

        graph.append(myneigbourlist)
        msp.append(list())

#prims alogithm
distances = []
daddy = []
vertexQueue = {}

#printGraph()

for i in range (0, len(graph)):
    vertexQueue[i] = sys.maxint
    distances.append(sys.maxint)
    daddy.append(-1)

seedVertex = random.randint(0, len(graph)-1)  
vertexQueue[seedVertex] = 0
distances[seedVertex]=0
#sortedVQ = sorted(vertexQueue.iteritems(), key=operator.itemgetter(1))
sortedVQ = sorted(vertexQueue.items(), key=lambda x: x[1])

while len(vertexQueue) != 0:
    myVertex = sortedVQ[0]
    vertexQueue.pop(myVertex[0])
    for neib in graph[myVertex[0]]:
        if neib in vertexQueue.keys():
            if(howfar(neib,myVertex[0]) < distances[neib]):
                vertexQueue[neib]=howfar(neib,myVertex[0])
                distances[neib]=howfar(neib,myVertex[0])
                daddy[neib] = myVertex[0]

    #sortedVQ = sorted(vertexQueue.iteritems(), key=operator.itemgetter(1))
    sortedVQ = sorted(vertexQueue.items(), key=lambda x: x[1])

for vertex in range (0, len(daddy)):
    if vertex!=seedVertex:
        msp[vertex].append(daddy[vertex])
        msp[daddy[vertex]].append(vertex)

printMsp()
