using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    [SerializeField] private LayerMask clickMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float shootHeight = 1.2f;
    [SerializeField] private float shootDamage = 25f;
    [SerializeField] private LineRenderer aimLine;
    private Camera cam;

    [Header("Visualization")]
    [SerializeField] private GameObject rangePrefab;

    private List<GameObject> rangeGroup = new List<GameObject>();
    public enum PlayerState
    {
        Idle,           
        Moving,
        Aiming,
        Interacting
    }
    [Header("���״̬��")]
    public PlayerState state = PlayerState.Idle;

    [Header("Interaction")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float detectRange = 2f;
    private List<Blockable> blockObjs = new List<Blockable>();
    [SerializeField] private LayerMask aimBlockMask;

    private void OnEnable()
    {
        LevelSpawn.OnPlayerSpawned += RangeVisualStart;

        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnStarted += RangeVisual;
    }

    private void OnDisable()
    {
        LevelSpawn.OnPlayerSpawned -= RangeVisualStart;

        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTurnStarted -= RangeVisual;
    }

    private void Start()
    {
        cam = Camera.main;
        currentNode = GetCurrentNode();
        CalculateRange();

        if (aimLine != null)
            aimLine.enabled = false;

        RangeVisual(0); // 保险：玩家自己生成完成后也直接画一次
    }

    void Update()
    {
        InterRange();

        switch (state)
        {
            case PlayerState.Idle:
                Idle();
                break;

            case PlayerState.Moving:
                break;

            case PlayerState.Aiming:
                Aim();
                break;
            case PlayerState.Interacting:
                BlockInteract();
                break;
        }
    }

    void Idle()
    {
        // ����ƶ�
        if (Input.GetMouseButtonDown(0))
            Move();

        // R������׼
        if (Input.GetKeyDown(KeyCode.R))
        {
            state = PlayerState.Aiming;
            HideVisual();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            state = PlayerState.Interacting;
            HideVisual();
        }
    }

     private void InterRange()
    {
        foreach (var obj in blockObjs)
        {
            if (obj != null) obj.SetHighlight(false);
        }

        blockObjs.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, blockLayer);
        foreach (var hit in hits)
        {
            Blockable blockable = hit.GetComponent<Blockable>();
            if (blockable!= null)
            {
                blockable.SetHighlight(true);
                blockObjs.Add(blockable);
            }
        }
    }

    void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickMask))
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
                Vector3 facdir = targetPos - transform.position;
                facdir.y = 0f;

                Quaternion targetRot = Quaternion.LookRotation(facdir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    rotateSpeed * Time.deltaTime
                );

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

        if (AmmoManager.Instance.CurrentAmmo <= 0)// no ammo bug fix
        {

            state = PlayerState.Idle;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }

            return;
        }

        AmmoManager.Instance.UseAmmo(); // ��ȷ���������ġ�
        //ǹ�ڸ߶�
        Vector3 origin = transform.position + Vector3.up * shootHeight;
        //�泯����
        Vector3 dir = transform.forward;
        //�������
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, aimBlockMask))
        {
            if (((1 << hit.collider.gameObject.layer) & enemyMask) != 0)
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
                Debug.Log("Block");
                state = PlayerState.Idle;

            if (aimLine != null)
            {
                aimLine.enabled = false;
            }
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

    private void BlockInteract()
    {
        if (!Input.GetMouseButtonDown(0))
        return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, blockLayer))
        {
            Blockable clickedObj = hit.collider.GetComponent<Blockable>();
            if(clickedObj != null && blockObjs.Contains(clickedObj))
            {
                clickedObj.OnBlock();
            }
        }
        state = PlayerState.Idle;
        TurnManager.Instance.PlayerFinishedAction();
    }

    public void RangeVisualStart(PlayerHealth health)
    {
        RangeVisual(0);
    }
   public void RangeVisual(int roundcount)
    {
        HideVisual();
        currentNode = GetCurrentNode();
        CalculateRange();

        foreach(Node node in reachableNodes)
        {
            GameObject rp = Instantiate(rangePrefab, GridManager.Instance.GetWorldPosition(node), Quaternion.Euler(0f, 0f, 0f));

            rangeGroup.Add(rp);
        }
    }

    private void HideVisual()
    {
        foreach(GameObject rp in rangeGroup)
        {
            Destroy(rp);
        }
        rangeGroup.Clear();
    }
}
