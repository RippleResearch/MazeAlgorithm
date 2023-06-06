using System.Numerics;
public class PrimsMaze
{
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
    }
}



public class Program
{
    static void Main(String[] args)
    {
        int height = 18;
        int width = 32;
        PrimsMaze prims = new PrimsMaze(width, height, new Vector2(0, 0));

    }
}