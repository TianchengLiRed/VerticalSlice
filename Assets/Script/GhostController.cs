using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

public class GhostController : MonoBehaviour
{
    public enum GhostState
    {
        Roaming,
        Chasing,
        Attacking
    }
    [SerializeField] private GhostState currentState = GhostState.Roaming;
    [SerializeField] private float randomMoveRadius = 2f;

    [SerializeField] private Transform player;
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float detectAngle = 360f;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private float chaseDistance = 3f;
    private Coroutine chaseRoutine;

    private NavMeshAgent agent;
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private GameObject alertImage;
    [SerializeField] private float alertDuration = 1f;

    private Coroutine alertRoutine;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnStarted += Action;
        }
        

        LevelSpawn.OnPlayerSpawned += SetPlayer;
    }

    void SetPlayer(PlayerHealth health)
    {
         GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }


    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnStarted -= Action;
        }
    }

    private void Action(int round)
{
    DetectPlayer();

    agent.ResetPath();

    switch (currentState)
    {
        case GhostState.Roaming:
            RandomMove();
            break;

        case GhostState.Chasing:
            Chase();
            break;

        case GhostState.Attacking:
            Attack();
            break;

    }
}
    private void ChangeState(GhostState newState)
    {
        if (currentState == newState) return;

        if(newState == GhostState.Chasing)
        {
            GhostAlertUI.Instance.ShowAlert();
            Debug.Log("Alert!");

        }

        currentState = newState;

        
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        if (distance <= attackRange)
        {
            ChangeState(GhostState.Attacking);
            return;
        }
        if (distance > detectRange)
        {
            ChangeState(GhostState.Roaming);
            return;
        }
        /*
        float angle = Vector3.Angle(transform.forward, dir);

        if (angle > detectAngle * 0.5f)
        {
            ChangeState(GhostState.Roaming);
            return;
        }
        */
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 direction = (target - origin).normalized;
        float rayDistance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, direction, rayDistance, obstacleMask))
        {
            ChangeState(GhostState.Roaming);
            return;
        }
        ChangeState(GhostState.Chasing);
    }

    private void RandomMove()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomMoveRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomMoveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void Chase()
    {
        EventBus.Trigger("GhostDetected");

        if (player == null) return;

        NavMeshPath path = new NavMeshPath();

        if (!agent.CalculatePath(player.position, path))
        {
            Debug.Log("No path to player.");
            return;
        }

        if (path.corners.Length < 2)
        {
            return;
        }

        Vector3 targetPos = GetPointAlongPath(path, chaseDistance);

        agent.ResetPath();
        agent.isStopped = false;
        agent.SetDestination(targetPos);

        if (chaseRoutine != null)
            StopCoroutine(chaseRoutine);

        chaseRoutine = StartCoroutine(StopAfterDistance(chaseDistance));
    }

    private Vector3 GetPointAlongPath(NavMeshPath path, float maxDistance)
    {
        float remainingDistance = maxDistance;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            float distance = Vector3.Distance(start, end);

            if (distance <= remainingDistance)
            {
                remainingDistance -=distance;
            }
            else
            {
                Vector3 direction = (end - start).normalized;
                return start + direction * remainingDistance;
            }
        }

        return path.corners[path.corners.Length - 1];
    }

    private IEnumerator StopAfterDistance(float maxDistance)
    {
        Vector3 lastPos = transform.position;
        float movedDistance = 0f;

        while (agent.pathPending || agent.hasPath)
        {
            float stepDistance = Vector3.Distance(transform.position, lastPos);
            movedDistance += stepDistance;
            lastPos = transform.position;

            if (movedDistance >= maxDistance)
            {
                agent.ResetPath();
                agent.isStopped = true;
                Debug.Log("Ghost stopped after moving max distance: " + movedDistance);
                yield break;
            }

            yield return null;
        }
    }

    private void Attack()
{
    Debug.Log("Ghost attacks player!");

    if (PlayerHealth.Instance != null)
    {
        PlayerHealth.Instance.TakeDamage(attackDamage);
    }

    TeleportOutsideDetectRange();
}

private void TeleportOutsideDetectRange()
{
    if (player == null) return;

    float minDistance = detectRange + 2f;
    float maxDistance = detectRange + 8f;

    for (int i = 0; i < 30; i++)
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);

        Vector3 targetPos = player.position + new Vector3(
            randomCircle.x,
            0f,
            randomCircle.y
        );

        NavMeshHit hit;

        if (NavMesh.SamplePosition(targetPos, out hit, 3f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // 传送 NavMeshAgent
            agent.ResetPath();

            ChangeState(GhostState.Roaming);
            return;
        }
    }


    ChangeState(GhostState.Roaming);
}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Vector3 left = Quaternion.Euler(0, -detectAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, detectAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;

        Gizmos.DrawRay(transform.position, left * detectRange);
        Gizmos.DrawRay(transform.position, right * detectRange);
    }
}
