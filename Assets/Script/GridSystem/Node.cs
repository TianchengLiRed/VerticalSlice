using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    [Header("格子创建")]
    public Vector2Int gridPos;
    public float height;
    public bool isWalkable = true;
    public bool isOccupied = false;
    public int gCost; // 起点到这里
    public int hCost; // 这里到终点
    public int fCost => gCost + hCost;
    public Node parent;
    public Node(Vector2Int pos, float height)
    {
        gridPos = pos;
        this.height = height;
    }
    public void SetOccupied(bool value)
    {
        //当前格子被占用boolean
        isOccupied = value;
    }

    public bool CanWalk()
    {
       //可移动判断
        return isWalkable && !isOccupied;
    }
}