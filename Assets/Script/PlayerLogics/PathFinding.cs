using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinding 
{
    public static List<Node> FindPath(Node start, Node target)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        //起点加入检测.从头扩散
        openSet.Add(start);
        //有能够检测的格子就继续检测
        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            //遍历寻找距离cost最小的
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < current.fCost ||
                   (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                    current = openSet[i];
            //检测过了移动到完成列表
            openSet.Remove(current);
            closedSet.Add(current);
            //到达目的地后生成路径
            if (current == target)
                return RetracePath(start, target);
            //获得这个格子的所有格子
            foreach (Node neighbour in GridManager.Instance.GetNeighbours(current))
            {
                //排除检测过的格子
                if (closedSet.Contains(neighbour))
                    continue;
                //新移动成本
                int newCost = current.gCost + 10;
                //如果新路径更好胡总和邻居未进入openset 进入if
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //判断新路径成本距离
                    neighbour.gCost = newCost;
                    neighbour.hCost = Heuristic(neighbour, target);
                    neighbour.parent = current;
                    //如果是邻居没检测,进入检测
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    static List<Node> RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    static int Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.gridPos.x - b.gridPos.x) +
               Mathf.Abs(a.gridPos.y - b.gridPos.y);
    }
}
