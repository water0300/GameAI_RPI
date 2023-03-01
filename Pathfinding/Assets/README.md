#pathfinding_Yuen_Nathan

## Instructions.

WASD to move camera, scroll to zoom in and out.

Left Click to place start/end node of a path

Right click to delete node from graph

Some UI elements adjust the properties of the AStar algorithm (tile vs waypoint, heuristic choice and weight, etc…)

## Code Overview.

###Map Generation

Transformation of the .map files into physical blocks in unity is done using a parser and Unity editor utility in the `MapGenerator.cs` file. At game start is when the actual A* graphs are generated. Any data regarding the graph exists in the `MapData.cs` file.

###Tilemap generation

This is automated by specifying a tilesize. The algorithm iterates through the map file as segmented square blocks (ex. 2x2, 3x3, …), such that the average position is accepted as a `GraphNode` and put into a 2D node matrix. If there are more unwalkable tiles compared to walkable tiles in the selected square block, then no `GraphNode` is added. Once all `GraphNode`s are created, An adjacency list if formed by looking at neighbors based on the 2D node matrix.

###Waypoint generation

The placement of waypoints is a manual process. On start, the map generator iterates through these nodes and determines if it has a line of sight to a neighboring node, up to a certain distance. If such line of sight is established, then a connection is established in the adjacency list. 

### AStar algorithm.

This just lives in the `MapData` as a single function `FindPath`. This is invoked in the `Player` function, and paths are determined by mouse clicks.

##Run Instructions

The only special thing to care about is to unzip one of the .unity scenes due to a Github size constraint.



