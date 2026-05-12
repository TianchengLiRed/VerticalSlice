using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsaneGrid : MonoBehaviour
{
    private void Start()
    {
        TurnManager.Instance.OnTurnStarted += CheckGrid;
    }

    private void CheckGrid(int round)
    {
        Node node = GetCurrentNode();
        if(node != null && node.isInsaneGrid)
        {
            PlayerHealth.Instance.TakeDamage(node.damage);
            Debug.Log("Insane!");
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
