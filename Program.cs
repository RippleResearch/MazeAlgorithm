using System.Numerics;

public class PrimsMaze
{
    int UNVISITED = 0;
    int VISITED = 3;
    int PATH = 1;
    int WALL = 2;

    private int width, height;
    private List<Vector2> frontier;

    private Random rand;

    public Vector2 start { get; set; }
    public int[,] maze { get; set; }

    public PrimsMaze(int width, int height, Vector2 start, bool guranteeBounds = true)
    {
        if (guranteeBounds)
        {
            if (width % 2 == 1 || height % 2 == 1) { 
                throw new FormatException("When using bounds width and height must be even!"); 
            }
            width++; height++; start.X++; start.Y++;
        }
        else if(!guranteeBounds && (width % 2 == 0 || height % 2 == 0)) { 
            throw new FormatException("When NOT using bounds width and height must be odd!"); 
        }
        
        this.width = width;
        this.height = height;
        this.start = new Vector2(start.X, start.Y);

        rand = new Random();
        frontier = new List<Vector2>();
        maze = new int[this.height, this.width];

        frontier.Add(this.start);

        Run();
    }

    /// <summary>
    /// Run checks both neighbors with a distance of 2 from the original vertex.
    /// Frontier stores the next possible paths and processes them randomly. 
    /// After processing the frontier (the now path) is removed. 
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
    /// Sets the given frontier to pathand then checks for surrounding neighbors 
    /// of distance 2 and distance 1 iff a location of distance 2 in a given direction 
    /// is not out of bounds of the maze.
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
        }

        if (KeepInRange(pos.X + 2, width - 1))
        {
            BuildMazeX((int)pos.X + 2, (int)pos.Y, 1);
        }

        if (KeepInRange(pos.Y - 2, height - 1))
        {
            BuildMazeY((int)pos.X, (int)pos.Y - 2, -1);  
        }

        if (KeepInRange(pos.Y + 2, height - 1))
        {
            BuildMazeY((int)pos.X, (int)pos.Y + 2, 1);
        }
    }

    /// <summary>
    /// Builds maze in the +X or -X direction and adds the vertex to the frontier of possible paths
    /// iff vertex is unvisited.
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
        //IF MARKED AS WALL CHANGE DIRECT NEIGHBOR TO PATH
        else if (maze[y, x] == WALL)
            maze[y, (sign == -1) ? x + 1 : x - 1] = PATH;
    }

    /// <summary>
    /// Builds maze in the Y direction and adds the vertex to the frontier of possible paths
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
        //IF MARKED AS WALL CHANGE DIRECT NEIGHBOR TO PATH
        else if (maze[y, x] == WALL)
            maze[(sign == -1) ? y + 1 : y - 1, x] = PATH;
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


    /////////////////////////////////////DEBUG PRINT METHODS////////////////////////////////////////
    public void printGraph()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (maze[i, j] == WALL || maze[i, j] == 0)  Console.Write("|");
                else if (maze[i, j] == PATH)                Console.Write((char)186);
                else                                        Console.Write((char) maze[i, j]);
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
    /////////////////////////////////////END DEBUG PRINT METHODS////////////////////////////////////
}
/// <summary>
/// Breadth First Search class to evalutate all possible
/// continuous paths from the starting location. This Class 
/// gives methods that will return the ending location as well
/// as recursivley trace a path from end to start. It also can 
/// return a 2D Array of Vertex Structs which contain their location
/// in the array and the location of their parent (for recursive path tracing). 
/// 
/// Implemented this pseudo code:
/// https://www.geeksforgeeks.org/breadth-first-search-or-bfs-for-a-graph/
/// </summary>
public class BFS
{
    int UNPROCESSED = 0;
    private Queue<Vector2> vertices;

    public struct Vertex
    {
        public int Turns { get; set; }
        public int Distance { get; set; }
        public Vector2 Prev { get; set; }
        public Vector2 Loc { get; set; }  
    }
    public Vertex[,] distanceMatrix;

    //Max distance vertex is a copy of the longest distance which allows us 
    //to find the end by referencing this vertex. 
    public Vertex maxDistance;
    //Max turns was another implementation used to attempt to create more
    //complex mazes, however, the longest past found by tracing Max Distances
    //location to the start seems to be the better apporach. 
    public Vertex maxTurns;
    public BFS()
    {
        maxDistance.Distance = -1;
        maxTurns.Turns = -1;

        vertices = new Queue<Vector2>();
    }

    //BFS implementation
    public Vertex[,] ComputeDistances(Vector2 start, int[,] graph)
    {
        distanceMatrix = new Vertex[graph.GetLength(0), graph.GetLength(1)];

        vertices.Enqueue(start);

        while(vertices.Count > 0)
        {
            Vector2 currVert = vertices.Dequeue();
            List<Vector2> neighbors = ProcessNeigbors(currVert, distanceMatrix, graph);

            foreach(Vector2 v in neighbors)
            {
                if (distanceMatrix[(int) v.Y, (int) v.X].Distance == UNPROCESSED) //Unprocessed
                {
                    vertices.Enqueue(v);
                    
                    //Set there location
                    distanceMatrix[(int)v.Y, (int)v.X].Loc = v;
                    //Sets them as visited and marks distance as one more than previous
                    distanceMatrix[(int)v.Y, (int)v.X].Distance = (distanceMatrix[(int) currVert.Y, (int) currVert.X].Distance + 1);
                    //Set Prev
                    distanceMatrix[(int)v.Y, (int)v.X].Prev = currVert;
                    //Update Turn
                    //If the current value x and y location are different than the the its parent vertice of distance 2 then a turn has been made
                    if ((distanceMatrix[(int)v.Y, (int)v.X].Loc.X != distanceMatrix[(int)currVert.Y, (int)currVert.X].Prev.X && distanceMatrix[(int)v.Y, (int)v.X].Loc.Y != distanceMatrix[(int)currVert.Y, (int)currVert.X].Prev.Y))
                        //add one to the turn
                        distanceMatrix[(int)v.Y, (int)v.X].Turns = distanceMatrix[(int)currVert.Y, (int)currVert.X].Turns + 1;
                    else
                        //other wise turn is kept the same
                        distanceMatrix[(int)v.Y, (int)v.X].Turns = distanceMatrix[(int)currVert.Y, (int)currVert.X].Turns;
                }
                //Update max distance
                if (maxDistance.Distance < distanceMatrix[(int)v.Y, (int)v.X].Distance + 1)
                    maxDistance = distanceMatrix[(int)v.Y, (int)v.X];

                //update max turns
                //Max turns vertice = the current max or the current vertices turns
                maxTurns = Math.Max(maxTurns.Turns, distanceMatrix[(int)v.Y, (int)v.X].Turns) == maxTurns.Turns ? maxTurns : distanceMatrix[(int)v.Y, (int)v.X];
            }
        }

        return distanceMatrix;
    }

    /// <summary>
    /// BFS Process Neighbors with the needed constraints for the Maze
    /// </summary>
    /// <param name="pos"> Current vertex </param>
    /// <param name="distanceMatrix"> 2D array of Vertices that contains information on distances </param>
    /// <param name="graph"> maze graph </param>
    /// <returns></returns>
    public List<Vector2> ProcessNeigbors(Vector2 pos, Vertex[,] distanceMatrix, int[,] graph)
    {
        List<Vector2> neighbors = new List<Vector2>();
        //Check if both the distanc matrix is not set and the graph at that location is a path
        //Path denoted as 1

        //check x -1
        if(KeepInRange(pos.X - 1, graph.GetLength(1) - 1))
        {
            //If distanceMatrix vertice has not been visited and the maze has a path at that location
            if (distanceMatrix[(int) pos.Y, (int) pos.X - 1].Distance == UNPROCESSED && graph[(int)pos.Y, (int)pos.X - 1] == 1)
            {
                //add to list
                neighbors.Add(new Vector2(pos.X - 1, pos.Y));
            }
        }
        //check x + 1
        if (KeepInRange(pos.X + 1, graph.GetLength(1) - 1))
        {
            //If distanceMatrix vertice has not been visited and the maze has a path at that location
            if (distanceMatrix[(int)pos.Y, (int)pos.X + 1].Distance == UNPROCESSED && graph[(int)pos.Y, (int)pos.X + 1] == 1)
            {
                //add to list
                neighbors.Add(new Vector2(pos.X + 1, pos.Y));
            }
        }
        //check y - 1
        if (KeepInRange(pos.Y - 1, graph.GetLength(0) - 1))
        {
            if (distanceMatrix[(int)pos.Y - 1, (int) pos.X].Distance == UNPROCESSED && graph[(int)pos.Y - 1, (int)pos.X] == 1)
            {
                neighbors.Add(new Vector2(pos.X, pos.Y - 1));
            }
        }
        //check y + 1
        if (KeepInRange(pos.Y + 1, graph.GetLength(0) - 1))
        {
            if (distanceMatrix[(int)pos.Y + 1, (int)pos.X].Distance == UNPROCESSED && graph[(int)pos.Y + 1, (int)pos.X] == 1)
            {
                neighbors.Add(new Vector2(pos.X, pos.Y + 1));
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Method so we can implement back tracking in the game 
    /// </summary>
    /// <param name="distanceMatrix"> 2D array of Vertices that contains information on any given vertex </param>
    /// <param name="maze">the final maze graph</param>
    /// <param name="start"> Original starting location of the maze creation </param>
    /// <param name="end"> the location of the longest path or the maximum distance from the node</param>
    /// <returns></returns>
    public Vector2 TracePath(Vertex[,] distanceMatrix, int[,] maze, Vector2 start, Vector2 end)
    {
        if (end != start)
        { 
            //Recursive call to mark the new end as the parent vertice of the current end
            return TracePath(distanceMatrix, maze, start, distanceMatrix[(int)end.Y, (int)end.X].Prev);
        }
        return end;//essentially returns start
    }


    /// <summary>
    ///  Method to visualize the path by changing the traced path to a different ascii chacater
    /// </summary>
    /// <param name="distanceMatrix"> 2D array of Vertices that contains information on any given vertex </param>
    /// <param name="maze">the final maze graph</param>
    /// <param name="start"> Original starting location of the maze creation </param>
    /// <param name="end"> the location of the longest path or the maximum distance from the node</param>
    /// <param name="pathChar"> Ascii value to change the path to within the maze for printing. Default is @ </param>
    /// <returns></returns>
    public int[,] TracePath(Vertex[,] distanceMatrix, int[,] maze, Vector2 start, Vector2 end, char pathChar = '@')
    {
        if (end != start)
        {
            //Set maze value to a given char
            maze[(int)end.Y, (int)end.X] = pathChar; 
            //Recursive call to mark the new end as the parent vertice of the current end
            return TracePath(distanceMatrix, maze, start, distanceMatrix[(int)end.Y, (int)end.X].Prev, pathChar);
        }
        //Set start value to pathChar
        maze[(int)start.Y, (int)start.X] = pathChar;
        return maze;
    }

    /// <summary>
    /// Return the vertice's location with the maximum distance
    /// from the start of the maze
    /// </summary>
    /// <returns></returns>
    public Vector2 GetEnd()
    {
        return maxDistance.Loc;
    }

    /// <summary>
    /// Simplified function to run BFS and just return the maze.
    /// All relevant fields will still be updated upon use of this
    /// method. 
    /// </summary>
    /// <param name="start"> Origin of the maze creation </param>
    /// <param name="maze"> Already generated maze for paths to be evaluated </param>
    /// <returns></returns>
    public Vector2 ComputeAndGetEnd(Vector2 start, int[,] maze)
    {
        ComputeDistances(start, maze);
        return maxDistance.Loc;
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

    /////////////////////////////////////DEBUG PRINT METHODS////////////////////////////////////////
    public void PrintVerticeTurns(Vertex[,] graph)
    {
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                Console.Write(graph[i, j].Turns);
            }
            Console.WriteLine();
        }
    }
    public void PrintVerticeDistance(Vertex[,] graph)
    {
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                Console.Write(graph[i, j].Distance);
            }
            Console.WriteLine();
        }
    }

    public void PrintVerticeGraphParents(Vertex[,] graph)
    {
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                Console.Write(graph[i, j].Prev.ToString() + "\t");
            }
            Console.WriteLine();
        }
    }
    /////////////////////////////////////END DEBUG PRINT METHODS////////////////////////////////////
}

public class Program
{
    static void Main(String[] args)
    {
        int height = 8;
        int width = 8;
        Vector2 start = new Vector2(0, 0);
        PrimsMaze prims = new PrimsMaze(width, height, start, guranteeBounds: true);
        prims.printGraph();
        Console.WriteLine();

        //BFS
        BFS bfs = new BFS();
        BFS.Vertex[,] distanceMatrixBFS = new BFS.Vertex[height, width];
        distanceMatrixBFS = bfs.ComputeDistances(prims.start, prims.maze);

        //Back Track Path
        prims.maze = bfs.TracePath(distanceMatrixBFS, prims.maze, prims.start, bfs.maxDistance.Loc, pathChar: '@');
        Console.WriteLine("BFS Max Distance: " + bfs.maxDistance.Distance + "\nMax Distance Turns: " + bfs.maxDistance.Turns + "\nMax distance end loc: " + bfs.maxDistance.Loc.ToString());
        //Set start and End
        prims.maze[(int)prims.start.Y, (int)prims.start.X] = '$';
        prims.maze[(int)bfs.maxDistance.Loc.Y, (int)bfs.maxDistance.Loc.X] = '$';
        prims.printGraph();

        /*prims.maze = bfs.TracePath(distanceMatrixBFS, prims.maze, prims.start, bfs.maxTurns.Loc, pathChar: '&');
        Console.WriteLine();
        Console.Write("Max Turns Distance: " + bfs.maxTurns.Distance + "\nMax Turns Value: " + bfs.maxTurns.Turns + "\nMax turns end loc: " + bfs.maxTurns.Loc.ToString() + "\n");
        prims.maze[(int)start.Y, (int)start.X] = '$';
        prims.maze[(int)bfs.maxTurns.Loc.Y, (int)bfs.maxTurns.Loc.X] = '$';
        prims.printGraph();*/


    }
}