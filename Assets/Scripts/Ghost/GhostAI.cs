using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class GhostAI : MonoBehaviour
{
    [SerializeField] GameObject _debugTile;
    public List<Vector2> CalculatePath(Vector3 ghostPosition, Vector3 playerPosition)
    {
        List<Vector2> path = new List<Vector2>();

        bool[,] map = new bool[TileManager.width,TileManager.height];
        for (int i = 0; i < TileManager.tiles.Count; i++)
        {
            Tile tile = TileManager.tiles[i];
            int height = Mathf.FloorToInt(i/TileManager.width);
            int width = i - height*TileManager.width;
            height = TileManager.height - height - 1;
            map[width, height] = !tile.occupied;
        }

        List<Node> pathNodes = AStar.FindPath((int)ghostPosition.x-1, (int)ghostPosition.y-1, (int)playerPosition.x-1, (int)playerPosition.y - 1, map);
        if (pathNodes != null)
        {
            int id=0;
            foreach (Node node in pathNodes)
            {
                id++;
                if (id == 1)
                {
                    continue;
                }
                Vector2 position = Vector2.zero;
                position.x = node.X+1;
                position.y = node.Y+1;
                path.Add(position);
                //Instantiate(_debugTile, position, Quaternion.identity).name = $"path{id}";
            }
        }
        else
        {
            Debug.Log($"Playerpos: {(int)playerPosition.x},{(int)playerPosition.y}");
            Debug.Log($"Ghostpos: {(int)ghostPosition.x},{(int)ghostPosition.y}");
            Debug.Log("No path found.");
        }
        return path;
    }
}

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public double G { get; set; } // cost to move from start to this node
    public double H { get; set; } // estimated cost to move from this node to end
    public double F { get { return G + H; } } // estimated total cost to move from start to end through this node
    public Node Parent { get; set; } // node used to reach this node
    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string ToString()
    {
        return $"X: {X} Y:{Y}";
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return X == ((Node)obj).X && Y == ((Node)obj).Y;
    }
}

public class AStar
{
    public static List<Node> FindPath(int startX, int startY, int endX, int endY, bool[,] map)
    {
        // create lists to store nodes to be explored and nodes already explored
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // create start and end nodes
        Node startNode = new Node(startX, startY);
        Node endNode = new Node(endX, endY);

        // add start node to open list
        openList.Add(startNode);

        // while the open list is not empty
        while (openList.Count > 0)
        {
            if (startNode.Equals(endNode))
            {
                break;
            }

            // find the node in the open list with the lowest F value
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // remove the current node from the open list and add it to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // if the current node is the end node, we have found a path
            if (currentNode.Equals(endNode))
            {
                // construct the path by backtracking from the end node to the start node
                return ConstructPath(currentNode);
            }

            // get a list of the current node's neighbors
            List<Node> neighbors = GetNeighbors(currentNode, map);

            // for each neighbor of the current node
            foreach (Node neighbor in neighbors)
            {
                // if the neighbor is in the closed list, ignore it
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                // calculate the cost to move from the start node to this neighbor
                double cost = currentNode.G + GetDistance(currentNode, neighbor);

                // if the neighbor is not in the open list, or if the cost to move to this neighbor is
                // lower than its current cost, set the neighbor's G value to the cost and set its
                // parent to the current node
                if (!openList.Contains(neighbor) || cost < neighbor.G)
                {
                    neighbor.G = cost;
                    neighbor.H = GetDistance(neighbor, endNode);
                    neighbor.Parent = currentNode;
                    // if the neighbor is not in the open list, add it to the open list
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                        Console.WriteLine(neighbor);
                    }
                }
            }
        }
        // if the open list is empty and we have not found the end node, there is no path
        return null;
    }

    private static List<Node> ConstructPath(Node endNode)
    {
        // create a list to store the path
        List<Node> path = new List<Node>();

        // add the end node to the path
        path.Add(endNode);

        // set the current node to the end node's parent
        Node currentNode = endNode.Parent;

        // while the current node is not the start node
        while (currentNode != null)
        {
            // add the current node to the path
            path.Add(currentNode);

            // set the current node to its parent
            currentNode = currentNode.Parent;
        }

        // reverse the list to get the path from start to end
        path.Reverse();

        return path;
    }

    private static List<Node> GetNeighbors(Node node, bool[,] map)
    {
        // create a list to store the neighbors
        List<Node> neighbors = new List<Node>();

        // add nodes to the list if they are walkable and within the bounds of the map
        if (IsWalkable(node.X - 1, node.Y, map))
        {
            neighbors.Add(new Node(node.X - 1, node.Y));
        }
        if (IsWalkable(node.X + 1, node.Y, map))
        {
            neighbors.Add(new Node(node.X + 1, node.Y));
        }
        if (IsWalkable(node.X, node.Y - 1, map))
        {
            neighbors.Add(new Node(node.X, node.Y - 1));
        }
        if (IsWalkable(node.X, node.Y + 1, map))
        {
            neighbors.Add(new Node(node.X, node.Y + 1));
        }

        return neighbors;
    }

    private static bool IsWalkable(int x, int y, bool[,] map)
    {
        // return true if the coordinates are within the bounds of the map and the tile is walkable
        return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && map[x, y];
    }

    private static double GetDistance(Node node1, Node node2)
    {
        // use the Manhattan distance heuristic to estimate the distance between two nodes
        return Math.Abs(node1.X - node2.X) + Math.Abs(node1.Y - node2.Y);
    }
}