using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //��������
    public static GridManager Instance;

    [Header("Grid")]
    //[SerializeField] private GameObject gridVisualPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int depth = 10;
    public float cellSize = 2f;
    [SerializeField] private float heightOffset = 1f;

    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask InsaneLayer;
    //��awake���ɸ��ӱ�֤��ɫ����
    private void Awake()
    {
        Instance = this;
        GenerateGrid();
    }
    //ͨ������xy����10*10map
    private void GenerateGrid()
    {
        grid.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);

                bool hasGround = TryGetGroundHeight(pos, out float height);

                if (!hasGround)
                {
                    continue;
                }

                Node node = new Node(pos, height);
                if (GetObstacle(node))
                {
                    node.isWalkable = false;
                }
                if (GetInsane(node))
                {
                    node.isInsaneGrid = true;
                }

                grid.Add(pos, node);
            }
        }
    }

private bool TryGetGroundHeight(Vector2Int gridPos, out float height)
{
    Vector3 worldPos = transform.position +
        new Vector3(gridPos.x * cellSize, 50f, gridPos.y * cellSize);

    Ray ray = new Ray(worldPos, Vector3.down);

    if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
    {
        height = hit.point.y;
        return true;
    }

    height = 0f;
    return false;
}
    public Node GetNode(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
            return grid[gridPos];

        return null;
    }
    public Vector3 GetWorldPosition(Node node)
    {
        //��ȡ��������
        return new Vector3(
            transform.position.x + node.gridPos.x * cellSize,
            node.height+heightOffset,
            transform.position.z + node.gridPos.y * cellSize
        );
    }

    public Vector3 GetRangePosition(Node node)
    {
        return new Vector3(
             transform.position.x + node.gridPos.x * cellSize,
            node.height + 0.02f,
            transform.position.z + node.gridPos.y * cellSize);
    }

private void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach (var node in grid.Values)
        {
            Gizmos.color = node.isInsaneGrid ? Color.black : node.isWalkable ? Color.green : Color.red;

            Vector3 worldPos = GetWorldPosition(node);

            Gizmos.DrawWireCube(
                worldPos,
                new Vector3(cellSize, 0.1f, cellSize)
            );
        }
    }
    //Ѱ�����ڸ���,�Զ�Ѱ·��
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
        //�������е�ǰλ�õ�dirs
        foreach (var dir in dirs)
        {
            Vector2Int checkPos = node.gridPos + dir;
            Node neighbour = GetNode(checkPos);
            //����и������ܹ�����,����neighbourlist��
            if (neighbour != null && neighbour.CanWalk())
                neighbours.Add(neighbour);
        }

        return neighbours;
    }

    private bool GetObstacle(Node node)
    {
        Vector3 worldPos = GetWorldPosition(node);

        return Physics.CheckBox(
            worldPos,
            new Vector3(cellSize * 0.4f, 1f, cellSize * 0.4f),
            Quaternion.identity,
            obstacleLayer);
    }

    private bool GetInsane(Node node)
    {
        Vector3 worldPos = GetWorldPosition(node);
        return Physics.CheckBox(
            worldPos, new Vector3(cellSize * 0.4f, 1f, cellSize * 0.4f),
            Quaternion.identity,
            InsaneLayer);
    }

/*
    private void CreateGridVisual(Node node)
    {
        Vector3 pos = GetWorldPosition(node);
        pos.y -=1f; ;

        Instantiate(gridVisualPrefab, pos, Quaternion.identity);
    }
   */
}
