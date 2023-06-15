# Overview
This random maze generation algorithm is loosely based on [Prims](https://en.wikipedia.org/wiki/Prim%27s_algorithm) minimum spanning tree algorithm. The original attempt was to implement only Prims Maze algorithm using Wikipedia's [Maze Generation Algorithm page](https://en.wikipedia.org/wiki/Maze_generation_algorithm). However, there was an error discovered within the given pseudo code. 

Specifically between the following lines:
```
> Pick a random wall from the list. If only one of the cells that the wall divides is visited, then:
	> Make the wall a passage and mark the unvisited cell as part of the maze.
```
The error lies within the conditional and the result. *If only one of the cells...is visited, mark the unvisited **cell**...*. This is an obvious error; if only one of the cells were marked visited, multiple would have to be marked as unvisited. Therefore, it was up for debate whether the author was implying random selection or another form of interpretation. This discrepancy led to a different approach. 

Based on a [Stack Overflow](https://stackoverflow.com/questions/29739751/implementing-a-randomly-generated-maze-using-prims-algorithm) user's response to a similar question of implementing Prim's algorithm, this implementation assumes, 
```
Cells can either be Blocked (walls) or Passages without storing any extra connectivity information.
```

Information between cells is later computed using the Breadth First Search algorithm (BFS). Using the BFS class creates the ability to find the longest path. In this implementation, the vertex with the longest distance from the start is used as the maze's end (operating under the assumption that the longest path is likely the most complex). While this information could have been computed during the maze's creation, there was little to no hit to performance with the scale of a maze that was necessary for this implementation (limited to both height and width less than 200). It also served for greater clarity of the process (achieved by using a well-known algorithm) but did add seemingly unnecessary steps to locate the end of the maze. See the **How to Implement Section** for examples. Nevertheless, BFS was implemented using the [Geeks for Geeks](https://www.geeksforgeeks.org/breadth-first-search-or-bfs-for-a-graph/#) pseudocode and worked as intended.

# Algorithm
As stated before, the algorithm is loosely based on [Prims](https://en.wikipedia.org/wiki/Prim%27s_algorithm) minimum spanning tree algorithm. This is depicted through the continuous flow of the path (guaranteeing a relatively large continuous path) but is deviated from when determining if the cell is a wall or path. The idea for this implementation was discovered after reviewing an article in The Buckblog titled, *[Maze Generation: Prim's Algorithm](https://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm)*, authored by Jamis Buck. This article features an in-depth breakdown of Maze Generation with both vertices *and weights* (weights are where the implementations differ). If you visit Buck's report, you will find an example that allows you to step through his Maze Generation. You will notice that walls are not shown as entire cells but rather as lines separating cells. This is an important distinction. For Buck's, it is feasible and likely that the walls are represented as cells, but it may be difficult to tell for the reader. In this implementation, walls are certainly represented as their own cells entirely. This leads to the actual implementation. It can be demonstrated with this pseudo code:
```
1. Start with a 2D array of walls (0)
2. Select a Starting grid location and add it to a list of 'frontiers.' 
	(a frontier being a cell of distance two in only the x or y position, not diagonal, from the original cell that is within the bounds of the array)
3. While the frontier list is not empty
	1. Select a random frontier and check for its neighboring frontiers
	2. If the neighboring frontier is within the bounds of the array:
		+If the neighboring frontier is a wall: 
			Make the direct neighbor to the original cell (the cell of distance one in the given direction) 
			and the neighboring frontier from a wall to a path
```
This implementation will create a 2D array of entirely 0's and 1's, which denote wall and path, respectively. 

# How to Clone With Visual Studio 
When this project was created Visual Studio 2022 Community editon was used. 

To clone open visual studio and:
![Clone Button](/Assets/Clone-Visual-Studio.png)
*Select Clone a Repository*

![Link Clone](/Assets/Link-Clone.png)
*Paste the repo's code link, Choose the path, Select Clone*


The source code can then be found in the Program.cs file. 


# How to Implement and Instantiate
To use this program as is, all one needs to do is the following:
1. Define an int height width and Vector2 Starting point.
2. Create a PrimsMaze object using the constructor
```
	PrimsMaze prims = new PrimsMaze(width, height, start, bounds: False)
```
*In addition to the int height and width you defined, there is another parameter, bounds. Bounds determines whether you want the maze to handle making a perimeter of walls or not. It is by default set to false (and requires and odd height and width), and it is recommended to use it this way. See **Notes** for more.*

*The constructor for primsMaze will automatically run and generate the maze. It will be stored in a public field named maze. To access this maze, just use prims.maze.*

3. Create a BFS object using the constructor and use the ComputeAndGetEnd Method:
```
	BFS bfs = new BFS()
	bfs.ComputeAndGetEnd(start, prims.maze, finalPathValue: 2)
```
*This is essentially a shortcut method that will run ComputeDistances to load a 2D array called distanceMatrix in the bfs object, it will then trace the path and update the maze with the given integer value (default int val = 2) to define a path from the start to the longest point.*

An example may look something like this:
```
int height = 31;
int width = 101;
Vector2 start = new Vector2(0, 0);
PrimsMaze prims = new PrimsMaze(width, height, start, bounds: false);
prims.printGraph();
Console.WriteLine();
//BFS
BFS bfs = new BFS();
bfs.ComputeAndGetEnd(prims.start, prims.maze, finalPathValue: 2);
//Set Start and End
Console.WriteLine("BFS Max Distance: " + bfs.maxDistance.Distance + "\nMax Distance Turns: " + bfs.maxDistance.Turns + "\nMax distance end loc: " + bfs.maxDistance.Loc.ToString());
//Set start and End
prims.maze[(int)prims.start.Y, (int)prims.start.X] = '$';
prims.maze[(int)bfs.maxDistance.Loc.Y, (int)bfs.maxDistance.Loc.X] = '$';
prims.printGraph();
```

This will output something similar to the following:
![Run Example](/Assets/Run-Example.png)


### NOTES
When using bounds, the program will automatically increment your height and width of the graph as well as move the location's start value by one; however, the actual 'usable' maze will still be one less than requested.

For example, a height and width of 6 with bounds: true will produce:

![Even Out](/Assets/Even-Out.png)

*You will notice this a 5 * 5 of 'usable' maze*

I would recommend always keeping bounds false and printing an extra border around the surroundings if that is what you wish for. This is because no bounds will always give a correct and 'usable' graph of height * width and does not alter anything upon instantiation.

As a final note, you will notice that when interfacing with the graphs in the source code, the X and Y values are switched. Take the first picture in the **How to Implement and Instantiate** section. The two lines near the bottom state:

prims.maze[(int)prims.start.Y, (int)prims.start.X] = '$';

prims.maze[(int)bfs.maxDistance.Loc.Y, (int)bfs.maxDistance.Loc.X] = '$';

The reason for this is a Vector2 carries two fields, an x and y float value. The x value represents the width, and the y represents the height using the cardinal coordinate system. However, 2D arrays do not necessarily carry a direction. Therefore, when indirectly addressing a 2D array (using prims.maze, for example), prims.maze[0,1] 0 would represent the first position, which means incrementing the 0 changes the rows of the array. While incrementing, the one would change the columns. Therefore, to use Vector2 values, I used prims.maze[Vector.Y, Vector.X] to access values in the maze. This may be tricky, but if you pay close attention, it should be manageable. 
	
	
