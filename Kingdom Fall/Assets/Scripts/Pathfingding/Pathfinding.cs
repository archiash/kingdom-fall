using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;

    Grid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos,Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos,Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //startNode.walkable && 
        if (targetNode.walkable)
        { 
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closetSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closetSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    sw.Stop();
                    print("Path Found: " + sw.ElapsedMilliseconds + " ms");
                    break;
                }


                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closetSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMoventCostToNeighhbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMoventCostToNeighhbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMoventCostToNeighhbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode); 
        }
        requestManager.FinishProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
       
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero; 
        
        for(int i = 1;i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node a , Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
