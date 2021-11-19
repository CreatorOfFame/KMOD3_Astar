using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Astar
{

    Node[,] nodes;
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        //Init the nodes in case they arent initialized
        DateTime start = DateTime.Now;
        if(nodes == null){
            nodes = new Node[grid.GetLength(0), grid.GetLength(1)];
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(0); y++){
                    nodes[x,y] = new Node(new Vector2Int(x,y),null,int.MaxValue,int.MaxValue);
                }
            }
        }

        //setup StartNode and HashSets
        nodes[startPos.x,startPos.y] = new Node(startPos,null,0,0);

        HashSet<Node> openNodes = new HashSet<Node>(){nodes[startPos.x,startPos.y]};
        HashSet<Node> closedNodes = new HashSet<Node>();

        //loop through it all!!!!
        int loops = 0;
        while(true){
            ++loops;

            var node = GetBestNode(openNodes);
            openNodes.Remove(node);
            closedNodes.Add(node);
            foreach(Node neighbour in GetNodeNeighbours(node,nodes,closedNodes)){
                if(!IsAccessible(grid[node.position.x,node.position.y],grid[neighbour.position.x,neighbour.position.y])) continue;
                openNodes.Add(neighbour);
                Vector2Int travelled = node.position - neighbour.position;
                Vector2Int distance = neighbour.position - endPos;
                neighbour.GScore = node.GScore + Mathf.Abs(travelled.x) + Mathf.Abs(travelled.y);
                neighbour.HScore = Mathf.Abs(distance.x) + Mathf.Abs(distance.y);
                if(node.parent == null || node.parent != neighbour)
                    neighbour.parent = node;
                if(neighbour.position == endPos){
                    List<Vector2Int> path = new List<Vector2Int>(){endPos};
                    Node parent = node;
                    while(parent != null){
                        path.Add(parent.position);
                        parent = parent.parent;
                    }
                    path.Reverse();
                    Debug.Log((DateTime.Now-start).Milliseconds + "ms");
                    return path;
                }
            }

            if(loops >= 1000000) break;
        }
        return null;
    }

    //Get the current best node
    Node GetBestNode(HashSet<Node> openNodes){
        Node best = null;
        foreach(Node node in openNodes){
            if(best == null || node.FScore < best.FScore){
                best = node;
            }
        }
        return best;
    }

    //you think I didnt notice your easy to copy GetNeihbours function? >:)
    List<Node> GetNodeNeighbours(Node node, Node[,] grid, HashSet<Node> closedNodes){
        List<Node> result = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++){
                if(!(x == 0 || y == 0)) continue;
                int nodeX = node.position.x + x;
                int nodeY = node.position.y + y;
                if (nodeX < 0 || nodeX >= grid.GetLength(0) || nodeY < 0 || nodeY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Node candidateNode = grid[nodeX, nodeY];
                if(!closedNodes.Contains(candidateNode))
                    result.Add(candidateNode);
            }
        }
        return result;
    }

    //Check whether or not a wall is blocking the node
    bool IsAccessible(Cell origin, Cell goal){
        if(origin.gridPosition.y > goal.gridPosition.y){
            return !goal.HasWall(Wall.UP);
        }else if(origin.gridPosition.y < goal.gridPosition.y){
            return !goal.HasWall(Wall.DOWN);
        }else if(origin.gridPosition.x < goal.gridPosition.x){
            return !goal.HasWall(Wall.LEFT);
        }else if(origin.gridPosition.x > goal.gridPosition.x){
            return !goal.HasWall(Wall.RIGHT);
        }
        return false;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
