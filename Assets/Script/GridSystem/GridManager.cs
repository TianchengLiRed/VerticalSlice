using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //单例设置
    public static GridManager Instance;

    [Header("Grid")]
    [SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int depth = 10;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float heightOffset = 1f;

    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    //在awake生成格子保证角色生成
    private void Awake()
    {
        Instance = this;
        GenerateGrid();
    }
    //通过遍历xy创建10*10map
    private void GenerateGrid()
    {
        grid.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);
                //导入对应height信息到height
                float height = GetHeightFromTerrain(pos);
                //存入diction
                Node node = new Node(pos, height);
                grid.Add(pos, node);

                CreateGridVisual(node);

            }
        }
    }

  
    float GetHeightFromTerrain(Vector2Int gridPos)
    {
        //遍历每一个格子通过射线获取高度
        Vector3 worldPos = transform.position +
                           new Vector3(gridPos.x * cellSize, 50f, gridPos.y * cellSize);

        Ray ray = new Ray(worldPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point.y;
        }

        return 0;
    }
    public Node GetNode(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
            return grid[gridPos];

        return null;
    }
    public Vector3 GetWorldPosition(Node node)
    {
        //获取世界坐标
        return new Vector3(
            transform.position.x + node.gridPos.x * cellSize,
            node.height+heightOffset,
            transform.position.z + node.gridPos.y * cellSize
        );
    }

    //格子在scene中的可视化
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.green;

        foreach (var node in grid.Values)
        {
            Vector3 worldPos = GetWorldPosition(node);

            Gizmos.DrawWireCube(
                worldPos,
                new Vector3(cellSize, 0.1f, cellSize)
            );
        }
    }
    //寻找相邻格子,自动寻路用
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        Vector2Int[] dirs =
        {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };
        //遍历所有当前位置的dirs
        foreach (var dir in dirs)
        {
            Vector2Int checkPos = node.gridPos + dir;
            Node neighbour = GetNode(checkPos);
            //如果有格子且能够行走,加入neighbourlist中
            if (neighbour != null && neighbour.CanWalk())
                neighbours.Add(neighbour);
        }

        return neighbours;
    }


    private void CreateGridVisual(Node node)
    {
        Vector3 pos = GetWorldPosition(node);
        pos.y -=1f; ;

        Instantiate(gridVisualPrefab, pos, Quaternion.identity);
    }
}
