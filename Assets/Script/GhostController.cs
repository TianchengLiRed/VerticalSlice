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
    [SerializeField] private float detectAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private float chaseDistance = 3f;

    private NavMeshAgent agent;
    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 20f;


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

        currentState = newState;
    }

   private void DetectPlayer()
{
    if (player == null) return;

        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;
        float angle = Vector3.Angle(transform.forward, dir);
        if (distance > detectRange)
        {
            ChangeState(GhostState.Roaming);
            return;
        }
        if (angle > detectAngle * 0.5f)
        {
            ChangeState(GhostState.Roaming);
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = dir.normalized;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, detectRange, obstacleMask))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                Debug.Log("Detected!");
                ChangeState(GhostState.Chasing);
                return;
            }
        }
        if (distance <= attackRange)
        {
            ChangeState(GhostState.Attacking);
        }
        else
        {
            ChangeState(GhostState.Chasing);
        }
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
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= chaseDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Vector3 limitedTarget = transform.position + direction.normalized * chaseDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(limitedTarget, out hit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
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

            Debug.Log("Ghost teleported outside detect range.");
            ChangeState(GhostState.Roaming);
            return;
        }
    }

    Debug.Log("No valid teleport position found.");

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
