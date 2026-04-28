using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AgentController : MonoBehaviour
{
    [Header("移动相关")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int moveRange = 3;
    [Header("范围相关")]
    private HashSet<Node> reachableNodes;
    private Node currentNode;

    [Header("射击相关")]
    [SerializeField] private float range = 25f;
    [SerializeField] private float rotateSpeed = 350f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float shootHeight = 1.2f;
    [SerializeField] private LineRenderer aimLine;
    private Camera cam;
    public enum PlayerState
    {
        Idle,           
        Moving,
        Aiming
    }
    [Header("玩家状态机")]
    private PlayerState state = PlayerState.Idle;
    private void Start()
    {
        cam = Camera.main;
        currentNode = GetCurrentNode();
        CalculateRange();
        if (aimLine != null)
        {
            aimLine.enabled = false;
        }
    }

    void Update()
    {
        switch (state)
        {
            case PlayerState.Idle:
                HandleIdle();
                break;

            case PlayerState.Moving:
                break;

            case PlayerState.Aiming:
                Aim();
                break;
        }
    }

    void HandleIdle()
    {
        // 左键移动
        if (Input.GetMouseButtonDown(0))
            Move();

        // R进入瞄准
        if (Input.GetKeyDown(KeyCode.R))
        {
            state = PlayerState.Aiming;
        }
    }

    void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //射线点击位置设定为 target
            Vector3 point = hit.point;

            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(point.x / 2f),
                Mathf.RoundToInt(point.z / 2f)
            );
            //获得起点输入path finding
            Node start = GetCurrentNode();
            //获得目的地输入path finding
            Node target = GridManager.Instance.GetNode(gridPos);
            //不能走不行
            if (target == null || !target.CanWalk())
            {
                Debug.Log("没法走");
                return;
            }
            //超出范围不行
            if (!reachableNodes.Contains(target))
            {
                Debug.Log("超出移动范围");
                return;
            }

            List<Node> path = PathFinding.FindPath(start, target);

            if (path != null)
                //开始移动
                StartCoroutine(FollowPath(path));
        }
    }

    void Aim()
    {
        AimRotate();
        UpdateAimLine();
        Shoot();
    }

    Node GetCurrentNode()
    {
        //获得当前玩家位置
        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / 2f),
            Mathf.RoundToInt(transform.position.z / 2f)
        );

        return GridManager.Instance.GetNode(pos);
    }
    //根据path finding算法输出的路径移动
    IEnumerator FollowPath(List<Node> path)
    {
        state = PlayerState.Moving;

        foreach (Node node in path)
        {
            //获得target客观坐标
            Vector3 targetPos = GridManager.Instance.GetWorldPosition(node);
            //根据path移动
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        state = PlayerState.Idle;
        //重新计算range刷新
        currentNode = GetCurrentNode();
        CalculateRange();
        TurnManager.Instance.PlayerFinishedAction();
    }
    //移动范围限制算法调用
    void CalculateRange()
    {
        reachableNodes = GridRange.GetReachableNodes(currentNode, moveRange);
    }
    //范围画线可视化 可删
    private void OnDrawGizmos()
    {
        if (reachableNodes == null) return;
        if (GridManager.Instance == null) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f); // 半透明蓝

        foreach (Node node in reachableNodes)
        {
            Vector3 pos = GridManager.Instance.GetWorldPosition(node);
            pos.y += 0.05f; // 防止和地面Z fighting

            Gizmos.DrawCube(pos, new Vector3(1.8f, 0.02f, 1.8f));
        }
    }

    //跟随鼠标旋转逻辑
    void AimRotate()
    {
        //从cam射出射线打向鼠标位置
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            //获得鼠标在世界的位置
            //方向计算
            Vector3 dir = hit.point - transform.position;
            //防止抬头
            dir.y = 0f;
            //防止鼠标再玩家脚下产生bug
            if (dir.sqrMagnitude < 0.01f) return;
            //面朝dir(鼠标位置)
            Quaternion targetRot = Quaternion.LookRotation(dir);
            //平滑移动
            transform.rotation = Quaternion.RotateTowards(
           transform.rotation,
           targetRot,
           rotateSpeed * Time.deltaTime
           );
        }
    }

    void Shoot()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (AmmoManager.Instance.CurrentAmmo <= 0)
        {
            Debug.Log("没子弹");
            return;
        }

        AmmoManager.Instance.UseAmmo(); // 明确看到“消耗”
        //枪口高度
        Vector3 origin = transform.position + Vector3.up * shootHeight;
        //面朝方向
        Vector3 dir = transform.forward;
        //射击射线
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, enemyMask))
        {
            Debug.Log("命中敌人: " + hit.collider.name);
            state = PlayerState.Idle;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }
        }
        else
        {
            Debug.Log("没命中");
            state = PlayerState.Idle;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }
        }

        TurnManager.Instance.PlayerFinishedAction();
    }
    void UpdateAimLine()
    {
        if (aimLine == null) return;

        Vector3 origin = transform.position + Vector3.up * shootHeight;
        Vector3 dir = transform.forward;

        Vector3 endPoint = origin + dir * range;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, enemyMask))
        {
            endPoint = hit.point;
        }

        aimLine.enabled = true;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, origin);
        aimLine.SetPosition(1, endPoint);
    }
}
