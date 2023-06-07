﻿using System.Numerics;

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

        distanceMatrix[(int)start.Y, (int)start.X] = 1; //Start is set so we can compute distances in parallel

        frontier.Add(start);

        Run();

        printGraph();
        Console.WriteLine();
       
        print2D(distanceMatrix);
        Console.WriteLine();

        BFS bfs = new BFS();
        int[,] distanceMatrixBFS = new int[height, width];
        distanceMatrixBFS = bfs.ComputeDistances(start, maze);
        print2D(distanceMatrixBFS);


        Console.WriteLine("BFS Max Distance: " + bfs.max.Distance + "\nBFS Location: " + bfs.max.position.ToString());
    }

    /// <summary>
    /// Run Checks both neighbors with a distance of 2 from the original vertice.
    /// Frontier stores the next possible paths and selects them randomly. 
    /// After processing the frontier (now path) is removed. 
    /// </summary>
    public void Run()
    {
        while(frontier.Count > 0)
        {
            int index = rand.Next(0, frontier.Count);
            CheckOppositeNeighbors(frontier[index]);
            frontier.RemoveAt(index);
        }
    }

    /// <summary>
    /// Checks for surrounding neighbors of distance 2 and distance 1 iff 
    /// a location of distance 2 in a given direction is not out of bounds of 
    /// the maze. Then distances are calculated during the computation.
    /// </summary>
    /// <param name="pos"></param>
    public void CheckOppositeNeighbors(Vector2 pos)
    {
        //SET FRONTIER TO PATH
        maze[(int)pos.Y, (int)pos.X] = PATH;
        //CHECK IF OPPOSITE NEIGBOR IS WITHIN BOUNDS
        if (KeepInRange(pos.X - 2, width - 1))
        {
            BuildMazeX((int)pos.X - 2, (int)pos.Y, -1);
            CalculateDistance((int)pos.X - 2, (int)pos.Y, -1, true);
        }

        if (KeepInRange(pos.X + 2, width - 1))
        {
            BuildMazeX((int)pos.X + 2, (int)pos.Y, 1);
            CalculateDistance((int)pos.X + 2, (int)pos.Y, 1, true);
        }

        if (KeepInRange(pos.Y - 2, height - 1))
        {
            BuildMazeY((int)pos.X, (int)pos.Y - 2, -1);
            CalculateDistance((int)pos.X, (int)pos.Y - 2, -1, false);
        }

        if (KeepInRange(pos.Y + 2, height - 1))
        {
            BuildMazeY((int)pos.X, (int)pos.Y + 2, 1);
            CalculateDistance((int)pos.X, (int)pos.Y + 2, 1, false);
        }
    }

    /// <summary>
    /// Builds maze in the X direction and adds the vertice to the frontier of possible paths
    /// </summary>
    /// <param name="x">position x that is in range</param>
    /// <param name="y">position y that is in range</param>
    /// <param name="sign">direction to denote whether we are moving up down left or right (-1 signifies left or up)</param>
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
            maze[y, (sign == -1) ? x + 1 : x - 1] = PATH;
           
        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
            maze[y, (sign == -1) ? x + 1 : x - 1] = (maze[y, (sign == -1) ? x + 1 : x - 1] == PATH) ? PATH : WALL;
    }

    /// <summary>
    /// Builds maze in the Y direction and adds the vertice to the frontier of possible paths
    /// </summary>
    /// <param name="x">position x that is in range</param>
    /// <param name="y">position y that is in range</param>
    /// <param name="sign">direction to denote whether we are moving up down left or right (-1 signifies left or up)</param>
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
            maze[(sign == -1) ? y + 1 : y - 1, x] = PATH;

        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
            maze[(sign == -1) ? y + 1 : y - 1, x] = (maze[(sign == -1) ? y + 1 : y - 1, x] == PATH) ? PATH : WALL;

    }

    /// <summary>
    /// Calculates the distance of direct neighbor then opposite if they have not been set. 
    /// </summary>
    /// <param name="x">position x that is in range</param>
    /// <param name="y">position y that is in range</param>
    /// <param name="sign">direction to denote whether we are moving up down left or right (-1 signifies left or up)</param>
    /// <param name="isX">Determines if the y or x value is being altered</param>
    public void CalculateDistance(int x, int y, int sign, bool isX)
    {
        if (isX)
        {
            //If the value has not been set (use sign to see if subtracting or adding)
            if (distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] == 0)
                //Then set it to one plus the previous value in the direction of the original vertice
                distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] = (distanceMatrix[y, (sign == -1) ? x + 2 : x - 2] + 1) % 10; // mod 10?
            if (distanceMatrix[y, x] == 0)
                distanceMatrix[y, x] = (distanceMatrix[y, (sign == -1) ? x + 1 : x - 1] + 1) % 10;
        }
        else
        {
            if (distanceMatrix[(sign == - 1) ? y + 1 : y - 1, x] == 0)
                distanceMatrix[(sign == -1) ? y + 1 : y - 1, x] = (distanceMatrix[(sign == -1) ? y + 2 : y - 2, x] + 1) % 10;
            if (distanceMatrix[y, x] == 0)
                distanceMatrix[y, x] = (distanceMatrix[(sign == -1) ? y + 1 : y - 1, x] + 1) % 10;
        }
    }

    /// <summary>
    /// Prints maze
    /// </summary>
    public void printGraph()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (maze[i, j] == WALL || maze[i, j] == 0)  Console.Write("|");
                else if (maze[i, j] == PATH)                Console.Write((char)186);
                else                                        Console.Write(maze[i, j]);
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
                Console.Write(graph[i, j] + " ");
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

    /// <summary>
    /// Helper function to determine if something is within the bounds of the array.
    /// It checks if val is <= bound and if val >= 0 and returns true or false accordingly.
    /// </summary>
    /// <param name="val">position to test </param>
    /// <param name="bound"></param>
    /// <returns></returns>
    public bool KeepInRange(float val, int bound)
    {
        return (val <= bound && val >= 0);
    }

}

public class BFS
{
    int UNPROCESS = 0;
    int VISITED = 1;
    private Queue<Vector2> vertices;


    public struct Max
    {
        public int Distance { get; set; }
        public Vector2 position { get; set; }
    }

    public struct Vertice
    {
        public int Distance { get; set; }
        public Vector2 Prev { get; set; }
    }

    public Max max;
    public BFS()
    {
        max = new Max();
        max.Distance = -1;

        vertices = new Queue<Vector2>();
    }

    public int[,] ComputeDistances(Vector2 start, int[,] graph)
    {
        int[,] distanceMatrix = new int[graph.GetLength(0), graph.GetLength(1)];

        vertices.Enqueue(start);
        distanceMatrix[(int)start.Y, (int)start.X] = VISITED;

        while(vertices.Count > 0)
        {
            Vector2 currVert = vertices.Dequeue();
            List<Vector2> neighbors = ProcessNeigbors(currVert, distanceMatrix, graph);

            foreach(Vector2 v in neighbors)
            {
                if (distanceMatrix[(int) v.Y, (int) v.X] == UNPROCESS) //Unprocessed
                {
                    vertices.Enqueue(v);
                    if(max.Distance < distanceMatrix[(int)currVert.Y, (int)currVert.X] + 1)
                    {
                        max.Distance = distanceMatrix[(int)currVert.Y, (int)currVert.X] + 1;
                        max.position = v;
                    }
                    distanceMatrix[(int)v.Y, (int)v.X] =  (distanceMatrix[(int) currVert.Y, (int) currVert.X] + 1);
                }
            }
        }

        return distanceMatrix;
    }

    public List<Vector2> ProcessNeigbors(Vector2 pos, int[,] distanceMatrix, int[,] graph)
    {
        List<Vector2> neighbors = new List<Vector2>();
        //Check if both the distanc matrix is not set and the graph at that location is a path
        //Path denoted as 1

        //check x -1
        if(KeepInRange(pos.X - 1, graph.GetLength(1) - 1))
        {
            //If distanceMatrix vertice has not been visited and the maze has a path at that location
            if (distanceMatrix[(int) pos.Y, (int) pos.X - 1] == UNPROCESS && graph[(int)pos.Y, (int)pos.X - 1] == 1)
            {
                //add to list
                neighbors.Add(new Vector2(pos.X - 1, pos.Y));
            }
        }
        //check x + 1
        if (KeepInRange(pos.X + 1, graph.GetLength(1) - 1))
        {
            //If distanceMatrix vertice has not been visited and the maze has a path at that location
            if (distanceMatrix[(int)pos.Y, (int)pos.X + 1] == UNPROCESS && graph[(int)pos.Y, (int)pos.X + 1] == 1)
            {
                //add to list
                neighbors.Add(new Vector2(pos.X + 1, pos.Y));
            }
        }
        //check y - 1
        if (KeepInRange(pos.Y - 1, graph.GetLength(0) - 1))
        {
            if (distanceMatrix[(int)pos.Y - 1, (int) pos.X] == UNPROCESS && graph[(int)pos.Y - 1, (int)pos.X] == 1)
            {
                neighbors.Add(new Vector2(pos.X, pos.Y - 1));
            }
        }
        //check y + 1
        if (KeepInRange(pos.Y + 1, graph.GetLength(0) - 1))
        {
            if (distanceMatrix[(int)pos.Y + 1, (int)pos.X] == UNPROCESS && graph[(int)pos.Y + 1, (int)pos.X] == 1)
            {
                neighbors.Add(new Vector2(pos.X, pos.Y + 1));
            }
        }

        return neighbors;
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


    public bool KeepInRange(float val, int bound)
    {
        return (val <= bound && val >= 0);
    }
}

public class Program
{
    static void Main(String[] args)
    {
        int height = 50;
        int width = 100;
        PrimsMaze prims = new PrimsMaze(width, height, new Vector2(0, 0));

    }
}