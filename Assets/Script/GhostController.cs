using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{
    public enum GhostState
    {
        Roaming,
        Chasing
    }
    [SerializeField] private GhostState currentState = GhostState.Roaming;
    [SerializeField] private float randomMoveRadius = 2f;

    [SerializeField] private Transform player;
    [SerializeField] private float detectRange = 8f;

    [SerializeField] private float chaseDistance = 3f;

    private NavMeshAgent agent;

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
    }

    private void Update()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnStarted += Action;
        }

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
        }
    }
    private void ChangeState(GhostState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log("Ghost State Changed To: " + currentState);
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            ChangeState(GhostState.Chasing);
        }
        else
        {
            ChangeState(GhostState.Roaming);
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
            Debug.Log("Ghost moves to: " + hit.position);
        }
        else
        {
            Debug.Log("No valid NavMesh point found.");
        }
    }

    private void Chase()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, randomMoveRadius);
    }
}
