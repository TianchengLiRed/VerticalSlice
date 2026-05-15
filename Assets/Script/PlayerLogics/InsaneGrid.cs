using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsaneGrid : MonoBehaviour
{
    private int turnCount = 0;
    private void Start()
    {
        TurnManager.Instance.OnTurnStarted += CheckGrid;
    }

    private void CheckGrid(int round)
    {
        Node node = GetCurrentNode();
        if(node != null && node.isInsaneGrid)
{
    turnCount++;

    if(turnCount >= 2)
    {
        PlayerHealth.Instance.TakeDamage(node.damage);
        Debug.Log("Insane!");
    }
}
else
{
    turnCount = 0;
}
    }

    private Node GetCurrentNode()
    {
        Vector3 localPos = transform.position - GridManager.Instance.transform.position;

        int x = Mathf.RoundToInt(localPos.x / GridManager.Instance.cellSize);
        int z = Mathf.RoundToInt(localPos.z / GridManager.Instance.cellSize);

        return GridManager.Instance.GetNode(new Vector2Int(x, z));
    }
}
