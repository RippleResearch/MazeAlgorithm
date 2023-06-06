using System.Diagnostics;
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

    private Random rand;
    public PrimsMaze(int width, int height, Vector2 start) { 
        this.width = width;
        this.height = height;
        this.start = start;

        rand = new Random();
        frontier = new List<Vector2>();
        maze = new int[height, width];


        frontier.Add(start);

        //printGraph();
        Run();
        printGraph();
    }

    public void Run()
    {
        while(frontier.Count > 0)
        {
            int index = rand.Next(0, frontier.Count);
            CheckOppositeNeighbors(frontier[index]);
            frontier.RemoveAt(index);
            //printGraph();
            Console.WriteLine("\n");
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

           // printGraph();
        }
        // If it is unvisited it is either a path, wall or marked as visited if it is a path,
        // mark the inbetween as wall
        //IF VISITED MARK AS PATH
        else if (maze[y, x] == WALL)
        {
            maze[y, (sign == -1) ? x + 1 : x - 1] = PATH;
            //printGraph();
        } 
        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
        {
            maze[y, (sign == -1) ? x + 1 : x - 1] = (maze[y, (sign == -1) ? x + 1 : x - 1] == PATH) ? PATH : WALL;
            //printGraph();
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

           // printGraph();
        }
        //IF MARKED AS WALL OR VISITED CHANGE DIRECT NEIGHBOR TO PATH
        else if (maze[y, x] == WALL)
        {
            maze[(sign == -1) ? y + 1 : y - 1, x] = PATH;

            //printGraph();
        }
        //IF PATH MARK AS WALL
        else if (maze[y, x] == PATH || maze[y, x] == VISITED)
        {
            maze[(sign == -1) ? y + 1 : y - 1, x] = (maze[(sign == -1) ? y + 1 : y - 1, x] == PATH) ? PATH : WALL;
            //printGraph();
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
        }

        if(KeepInRange(pos.X + 2, width - 1))
        {
            BuildMazeX((int) pos.X + 2, (int) pos.Y, 1);
        }

        if(KeepInRange(pos.Y - 2, height - 1))
        {
            BuildMazeY((int) pos.X,(int) pos.Y - 2, -1);
        }

        if (KeepInRange(pos.Y + 2, height - 1))
        {
            BuildMazeY((int) pos.X, (int) pos.Y + 2, 1);
        }
    }


    public bool KeepInRange(float val, int bound)
    {
        return (val <= bound && val >= 0);
    }


    public void printGraph()
    {
        //row length
        for (int i = 0; i < height; i++)
        {
            //col length?
            for (int j = 0; j < width; j++)
            {
                //Thread.Sleep(20);
                if (maze[i, j] == WALL || maze[i, j] == 0)
                    Console.Write("|");
                else Console.Write((char)186);
            }
            Console.WriteLine();
        }
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