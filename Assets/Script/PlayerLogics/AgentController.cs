using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AgentController : MonoBehaviour
{
    [Header("�ƶ����")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int moveRange = 3;
    [Header("��Χ���")]
    private HashSet<Node> reachableNodes;
    private Node currentNode;

    [Header("������")]
    [SerializeField] private float range = 25f;
    [SerializeField] private float rotateSpeed = 350f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float shootHeight = 1.2f;
    [SerializeField] private float shootDamage = 25f;
    [SerializeField] private LineRenderer aimLine;
    private Camera cam;
    public enum PlayerState
    {
        Idle,           
        Moving,
        Aiming
    }
    [Header("���״̬��")]
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
        // ����ƶ�
        if (Input.GetMouseButtonDown(0))
            Move();

        // R������׼
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
            //���ߵ��λ���趨Ϊ target
            Vector3 point = hit.point;

            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(point.x / 2f),
                Mathf.RoundToInt(point.z / 2f)
            );
            //����������path finding
            Node start = GetCurrentNode();
            //���Ŀ�ĵ�����path finding
            Node target = GridManager.Instance.GetNode(gridPos);
            //�����߲���
            if (target == null || !target.CanWalk())
            {
                Debug.Log("û����");
                return;
            }
            //������Χ����
            if (!reachableNodes.Contains(target))
            {
                Debug.Log("�����ƶ���Χ");
                return;
            }

            List<Node> path = PathFinding.FindPath(start, target);

            if (path != null)
                //��ʼ�ƶ�
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
        //��õ�ǰ���λ��
        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / 2f),
            Mathf.RoundToInt(transform.position.z / 2f)
        );

        return GridManager.Instance.GetNode(pos);
    }
    //����path finding�㷨�����·���ƶ�
    IEnumerator FollowPath(List<Node> path)
    {
        state = PlayerState.Moving;

        foreach (Node node in path)
        {
            //���target�͹�����
            Vector3 targetPos = GridManager.Instance.GetWorldPosition(node);
            //����path�ƶ�
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
        //���¼���rangeˢ��
        currentNode = GetCurrentNode();
        CalculateRange();
        TurnManager.Instance.PlayerFinishedAction();
    }
    //�ƶ���Χ�����㷨����
    void CalculateRange()
    {
        reachableNodes = GridRange.GetReachableNodes(currentNode, moveRange);
    }
    //��Χ���߿��ӻ� ��ɾ
    private void OnDrawGizmos()
    {
        if (reachableNodes == null) return;
        if (GridManager.Instance == null) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f); // ��͸����

        foreach (Node node in reachableNodes)
        {
            Vector3 pos = GridManager.Instance.GetWorldPosition(node);
            pos.y += 0.05f; // ��ֹ�͵���Z fighting

            Gizmos.DrawCube(pos, new Vector3(1.8f, 0.02f, 1.8f));
        }
    }

    //���������ת�߼�
    void AimRotate()
    {
        //��cam������ߴ������λ��
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
        {
            //�������������λ��
            //�������
            Vector3 dir = hit.point - transform.position;
            //��ֹ̧ͷ
            dir.y = 0f;
            //��ֹ�������ҽ��²���bug
            if (dir.sqrMagnitude < 0.01f) return;
            //�泯dir(���λ��)
            Quaternion targetRot = Quaternion.LookRotation(dir);
            //ƽ���ƶ�
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
            Debug.Log("û�ӵ�");
            return;
        }

        AmmoManager.Instance.UseAmmo(); // ��ȷ���������ġ�
        //ǹ�ڸ߶�
        Vector3 origin = transform.position + Vector3.up * shootHeight;
        //�泯����
        Vector3 dir = transform.forward;
        //�������
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, enemyMask))
        {
            Debug.Log("命中敌人: " + hit.collider.name);

            if (GhostHealth.Instance != null)
            {
                GhostHealth.Instance.TakeDamage(shootDamage);
            }
            state = PlayerState.Idle;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }
            
            
        }
        else
        {
            Debug.Log("miss");
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
