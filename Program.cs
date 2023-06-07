﻿using System.Diagnostics;
using System.Numerics;
using System.Text;

public class PrimsMaze
{
    int UNVISITED = 0;
    int VISITED = 3;
    int PATH = 1;
    int WALL = 2;


    private int width, height;
    private Vector2 start;
    private List<Vector2> frontier;
    private int[,] maze;
    private int[,] distanceMatrix;

    private Random rand;
    public PrimsMaze(int width, int height, Vector2 start) { 
        this.width = width;
        this.height = height;
        this.start = start;

        rand = new Random();
        frontier = new List<Vector2>();
        maze = new int[height, width];
        distanceMatrix = new int[height, width];

        distanceMatrix[(int)start.Y, (int)start.X] = 1;

        frontier.Add(start);
        //fill(distanceMatrix, -1);

        Run();
        printGraph();
        Console.WriteLine();
        //print2D(maze);
        //Console.WriteLine();
        print2D(distanceMatrix);
    }

  

    public void Run()
    {
        while(frontier.Count > 0)
        {
            int index = rand.Next(0, frontier.Count);
            CheckOppositeNeighbors(frontier[index]);
            frontier.RemoveAt(index);
            printGraph();
            Console.WriteLine();
            print2D(distanceMatrix);
            Console.WriteLine();
        }
    }

    //Takes a position that we already know is in range
    public void BuildMazeX(int x, int y, int sign)
    {
        //IF UNVISITED ADD TO LIST OF FRONTIER AND MAKE PATH
        if (maze[y, x] == UNVISITED)
        {
            //If sign is negative 1 then we have checked -2 to the left
            //And therefore need to change the path only 1 to the left
            //meaning we add 1. Otherwise we were moving right and need to 
            //subtract 1
            maze[y, (sign == -1) ? x + 1 : x - 1] = PATH; //Set direct neighbor to path
            frontier.Add(new Vector2(x, y)); //Add opposite neibor to list of possibe frontier
            maze[y, x] = VISITED; //add opposite neighbor to visited and now possible frontier
        }
        // If it is unvisited it is either a path, wall or marked as visited if it is a path,
        // mark the inbetween as wall
        //IF VISITED MARK AS PATH
        else if (maze[y, x] == WALL)
        {
            maze[y, (sign == -1) ? x + 1 : x - 1] = PATH;
           
        } 
        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
        {
            maze[y, (sign == -1) ? x + 1 : x - 1] = (maze[y, (sign == -1) ? x + 1 : x - 1] == PATH) ? PATH : WALL;
        }
    }

    public void BuildMazeY(int x, int y, int sign)
    {
        //IF UNVISITED ADD TO LIST OF FRONTIER AND MAKE PATH
        if (maze[y, x] == UNVISITED)
        {
            maze[(sign == -1) ? y + 1 : y - 1, x] = PATH;
            frontier.Add(new Vector2(x, y));
            maze[y, x] = VISITED;
        }
        //IF MARKED AS WALL OR VISITED CHANGE DIRECT NEIGHBOR TO PATH
        else if (maze[y, x] == WALL)
        {
            maze[(sign == -1) ? y + 1 : y - 1, x] = PATH;

        }
        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
        {
            maze[(sign == -1) ? y + 1 : y - 1, x] = (maze[(sign == -1) ? y + 1 : y - 1, x] == PATH) ? PATH : WALL;
        }
    }

    public void CheckOppositeNeighbors(Vector2 pos)
    {
        //SET FRONTIER TO PATH
        maze[(int) pos.Y, (int) pos.X] = PATH; 
        //CHECK IF OPPOSITE NEIGBOR IS WITHIN BOUNDS
        if(KeepInRange(pos.X - 2, width - 1))
        {
            BuildMazeX((int) pos.X - 2, (int) pos.Y, -1);
            CalculateDistance((int) pos.X - 2,(int) pos.Y, -1, true);
        }

        if(KeepInRange(pos.X + 2, width - 1))
        {
            BuildMazeX((int) pos.X + 2, (int) pos.Y, 1);
            CalculateDistance((int)pos.X + 2, (int)pos.Y, 1, true);
        }

        if(KeepInRange(pos.Y - 2, height - 1))
        {
            BuildMazeY((int) pos.X,(int) pos.Y - 2, -1);
            CalculateDistance((int)pos.X, (int)pos.Y-2, -1, false);
        }

        if (KeepInRange(pos.Y + 2, height - 1))
        {
            BuildMazeY((int)pos.X, (int)pos.Y + 2, 1);
            CalculateDistance((int)pos.X, (int)pos.Y + 2, 1, false);
        }
    }

    public void CalculateDistance(int x, int y, int sign, bool isX)
    {
        if (isX)
        {
            //If the value has not been set (use sign to see if subtracting or adding)
            if (distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] == 0)
                //Then set it to one plus the previous value in the direction of the original vertice
                distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] = (distanceMatrix[y, (sign == -1) ? x + 2 : x - 2] + 1); // mod 10?
            if (distanceMatrix[y, x] == 0)
                distanceMatrix[y, x] = (distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] + 1);
        }
        else
        {
            if (distanceMatrix[(sign == - 1) ? y + 1 : y - 1, x] == 0)
                distanceMatrix[(sign == -1) ? y + 1 : y - 1, x] = (distanceMatrix[(sign == -1) ? y + 2 : y - 2, x] + 1);
            if (distanceMatrix[y, x] == 0)
                distanceMatrix[y, x] = (distanceMatrix[(sign == -1) ? y + 1 : y - 1, x] + 1);
        }
    }


    public bool KeepInRange(float val, int bound)
    {
        return (val <= bound && val >= 0);
    }


    public void printGraph()
    {
        //Width++ because we added an extra wall at start of col
        /*for(int i = 0; i < width + 1; i++)
        {
            Console.Write("|");
        }
        Console.WriteLine();*/

        //row length
        for (int i = 0; i < height; i++)
        {
            //col length?
            for (int j = 0; j < width; j++)
            {
                //if (j == 0) Console.Write("|");
                //Thread.Sleep(20);
                if (maze[i, j] == WALL || maze[i, j] == 0)
                    Console.Write("|");
                else if (maze[i, j] == PATH)
                {
                    Console.Write((char)186);
                }
                else Console.Write(maze[i, j]);
            }
            Console.WriteLine();
        }
    }

    public void print2D(int[,] graph)
    {
        for(int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                Console.Write(graph[i, j]);
            }
            Console.WriteLine();
        }
    }

    public void fill(int[,] matrix, int value)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = value;
            }
        }
    }
}

public class BFS
{
    int VISITED = 1;

    private Queue<Vector2> vertices;
    private int[,] graph;
    private Vector2 start;

    public BFS(int[,] graph, Vector2 start)
    {
        this.start = start;
        this.graph = graph;
        vertices = new Queue<Vector2>();
    }

    public int[,] ComputeDistances(Vector2 start, int[,] graph)
    {
        int[,] distanceMatrix = new int[graph.GetLength(0), graph.GetLength(1)];
        vertices.Enqueue(start);
        distanceMatrix[(int)start.Y, (int)start.X] = VISITED;

        while(vertices.Count > 0)
        {
            Vector2 pos = vertices.Dequeue();
            List<Vector2> neighbors = ProcessNeigbors(pos, distanceMatrix);

            foreach(Vector2 v in neighbors)
            {
                if (distanceMatrix[(int) v.Y, (int) v.X] == 0) //Unprocessed
                {
                    vertices.Enqueue(v);
                    distanceMatrix[(int)v.Y, (int)v.X] = VISITED;
                }
            }
        }

        return distanceMatrix;
    }

    public List<Vector2> ProcessNeigbors(Vector2 pos, int[,] distanceMatrix)
    {
        List<Vector2> neighbors = new List<Vector2>();
        //Check Neighbors
        

        return neighbors;
    }

    



}

public class Program
{
    static void Main(String[] args)
    {
        int height = 5;
        int width = 10;
        PrimsMaze prims = new PrimsMaze(width, height, new Vector2(0, 0));

    }
}