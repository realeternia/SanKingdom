using System;
using System.Collections.Generic;

/// <summary>
/// 通用 A* 寻路模块：纯 C# 逻辑，不依赖 UnityEngine。
/// open set 用二叉堆（小顶堆）按 f = g + h 排序，平局按 (f, g, gx, gz) 字典序比较，保证回放一致。
/// </summary>
public static class AStarPathfinding
{
    /// <summary>
    /// A* 寻路：返回从 start 到任一满足 isGoal 的格子的完整路径（含起点与终点）。
    /// 无可行路径返回 null。启发函数必须可采纳（heuristic ≤ 真实成本）以保证最优。
    /// </summary>
    public static List<(int gx, int gz)> FindPath(
        (int gx, int gz) start,
        Func<(int gx, int gz), bool> isGoal,
        Func<(int gx, int gz), IEnumerable<((int gx, int gz) cell, int cost)>> expand,
        Func<(int gx, int gz), int> heuristic,
        int maxExpand = 512)
    {
        // 起点即目标，直接返回
        if (isGoal(start))
            return new List<(int, int)> { start };

        // 各格当前最优 g 值（同时充当 closed 表）
        var gScore = new Dictionary<(int gx, int gz), int>();
        // 父格映射，用于回溯路径
        var parent = new Dictionary<(int gx, int gz), (int gx, int gz)>();
        var open = new MinHeap();

        gScore[start] = 0;
        open.Push(new Node(start, 0, heuristic(start)));

        int expanded = 0;
        while (open.Count > 0)
        {
            Node node = open.Pop();

            // 跳过过期项：同一格被更优 g 值重新压入后，旧项作废
            if (node.g > gScore[node.cell])
                continue;

            expanded++;
            if (expanded > maxExpand)
            {
                GameLog.Debug($"AStarPathfinding 扩展超过上限 {maxExpand}，放弃寻路");
                return null;
            }

            if (isGoal(node.cell))
                return ReconstructPath(parent, node.cell, start);

            // 扩展邻格
            foreach (var item in expand(node.cell))
            {
                (int gx, int gz) cell = item.cell;
                int cost = item.cost;
                int tentativeG = node.g + cost;

                // 未发现或更优才入堆
                if (gScore.TryGetValue(cell, out int bestG) && tentativeG >= bestG)
                    continue;

                gScore[cell] = tentativeG;
                parent[cell] = node.cell;
                open.Push(new Node(cell, tentativeG, tentativeG + heuristic(cell)));
            }
        }

        return null;
    }

    /// <summary>
    /// 从终点沿 parent 回溯到起点，反转后得到完整路径（含起点与终点）
    /// </summary>
    private static List<(int gx, int gz)> ReconstructPath(
        Dictionary<(int gx, int gz), (int gx, int gz)> parent,
        (int gx, int gz) end,
        (int gx, int gz) start)
    {
        var path = new List<(int gx, int gz)>();
        (int gx, int gz) cell = end;
        while (true)
        {
            path.Add(cell);
            if (cell == start)
                break;
            cell = parent[cell];
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// 堆节点：cell 为格子坐标，g 为起点到该格的实际代价，f = g + h
    /// </summary>
    private struct Node : IComparable<Node>
    {
        public (int gx, int gz) cell;
        public int g;
        public int f;

        public Node((int gx, int gz) cell, int g, int f)
        {
            this.cell = cell;
            this.g = g;
            this.f = f;
        }

        /// <summary>
        /// 按 (f, g, gx, gz) 字典序比较，保证平局时弹出顺序确定
        /// </summary>
        public int CompareTo(Node other)
        {
            if (f != other.f)
                return f.CompareTo(other.f);
            if (g != other.g)
                return g.CompareTo(other.g);
            if (cell.gx != other.cell.gx)
                return cell.gx.CompareTo(other.cell.gx);
            return cell.gz.CompareTo(other.cell.gz);
        }
    }

    /// <summary>
    /// 小顶堆（二叉堆）：按 Node 字典序排序，提供 Push/Pop/Count
    /// </summary>
    private sealed class MinHeap
    {
        private readonly List<Node> nodes = new List<Node>();

        public int Count => nodes.Count;

        /// <summary>
        /// 入堆：追加到末尾后上浮
        /// </summary>
        public void Push(Node node)
        {
            nodes.Add(node);
            int index = nodes.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (nodes[index].CompareTo(nodes[parentIndex]) >= 0)
                    break;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        /// <summary>
        /// 出堆：取堆顶，末位补位后下沉
        /// </summary>
        public Node Pop()
        {
            Node top = nodes[0];
            int lastIndex = nodes.Count - 1;
            nodes[0] = nodes[lastIndex];
            nodes.RemoveAt(lastIndex);

            int index = 0;
            int count = nodes.Count;
            while (true)
            {
                int left = index * 2 + 1;
                int right = left + 1;
                int smallest = index;
                if (left < count && nodes[left].CompareTo(nodes[smallest]) < 0)
                    smallest = left;
                if (right < count && nodes[right].CompareTo(nodes[smallest]) < 0)
                    smallest = right;
                if (smallest == index)
                    break;
                Swap(index, smallest);
                index = smallest;
            }
            return top;
        }

        private void Swap(int a, int b)
        {
            Node tmp = nodes[a];
            nodes[a] = nodes[b];
            nodes[b] = tmp;
        }
    }
}
