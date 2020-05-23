using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public Transform hero;
    public LayerMask pathMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    public int obstracleProximityPanelty = 10;
    public TerrainType[] walkable;
    Dictionary<int, int> walkableRegion = new Dictionary<int, int>();

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    int paneltyMin = int.MaxValue;
    int paneltyMax = int.MinValue;


    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkable)
        {
            pathMask.value |= region.terrainMask.value;
            walkableRegion.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);

        }


        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX* gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.up * gridWorldSize.y / 2);

        for (int x = 0;x<gridSizeX;x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                bool walkable = (Physics2D.OverlapCircle(worldPoint,nodeRadius,pathMask));

                int movementPenalty = 0;

                              
                    Ray ray = new Ray(worldPoint + Vector3.back * 100, Vector3.forward);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector3.forward, Mathf.Infinity, pathMask);
                    if (hit)
                    {
                        walkableRegion.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        Debug.Log(hit.collider.gameObject.layer);
                    }

                    if(!walkable)
                {
                    movementPenalty += obstracleProximityPanelty;
                }
                

                grid[x, y] = new Node(walkable,worldPoint,x,y,movementPenalty);
            
            
            }
        }

        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize)
    {
        int kernalSize = blurSize * 2 + 1;
        int kernalExtents = (kernalSize - 1) / 2;

        int[,] penaltyHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltyVerticalPass = new int[gridSizeX, gridSizeY];

        for(int y = 0;y < gridSizeY;y++)
        {
            for (int x = -kernalSize; x <= kernalExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernalExtents);
                penaltyHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for(int x = 1; x <gridSizeX;x++)
            {
                int removeIndex = Mathf.Clamp(x - kernalExtents - 1,0,gridSizeX);
                int addIndex = Mathf.Clamp(x + kernalExtents, 0, gridSizeX-1);

                penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernalSize; y <= kernalExtents; y++)
            {
                int sampleY = Mathf.Clamp(x, 0, kernalExtents);
                penaltyVerticalPass[x, 0] += penaltyHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, 0]) / (kernalSize * kernalSize);
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernalExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernalExtents, 0, gridSizeY - 1);

                penaltyVerticalPass[x, y] = penaltyVerticalPass[x, y-1] - penaltyHorizontalPass[x,removeIndex] + grid[x, addIndex].movementPenalty;
                blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, y]) / (kernalSize * kernalSize);
                grid[x, y].movementPenalty = blurredPenalty; 

                if(blurredPenalty > paneltyMax)
                {
                    paneltyMax = blurredPenalty;
                }
                if(blurredPenalty < paneltyMin)
                {
                    paneltyMin = blurredPenalty;
                }
            }
        }
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1;x<= 1;x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x==0 && y==0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
                
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

            
        if (grid != null && displayGridGizmos)            
        {               
            foreach (Node n in grid)              
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(paneltyMin, paneltyMax, n.movementPenalty));

                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                   
                Gizmos.DrawWireCube(n.worldPosition, new Vector3(1, 1, 0.1f) * (nodeDiameter));
               
            }         
        }     
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
