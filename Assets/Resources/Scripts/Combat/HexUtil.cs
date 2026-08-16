using UnityEngine;

/// <summary>
/// 六边形网格工具：平顶(flat-top)六边形，奇数列整体下移半行(odd-q 偏移)。
/// 六方向：上/下(同列 gz±1)、左上/左下/右上/右下(邻列，随列奇偶偏移)。
/// 尺寸：外接圆半径 HEX_RADIUS，列间距 1.5R，行高 √3R。
/// </summary>
public static class HexUtil
{
    private const float COL_SPACING = SystemConst.Battle.HEX_COL_SPACING;
    private const float ROW_HEIGHT = SystemConst.Battle.HEX_ROW_HEIGHT;
    private const float HALF_ROW = SystemConst.Battle.HEX_HALF_ROW;

    /// <summary>
    /// 格子坐标转世界坐标（格子中心）
    /// </summary>
    public static Vector3 GridToWorld(int gx, int gz, float y = 7f)
    {
        float x = gx * COL_SPACING;
        float z = gz * ROW_HEIGHT + (((gx & 1) == 1) ? HALF_ROW : 0f);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 世界坐标转格子坐标（四舍五入到最近格心）
    /// </summary>
    public static (int gx, int gz) WorldToGrid(Vector3 worldPos)
    {
        int gx = Mathf.RoundToInt(worldPos.x / COL_SPACING);
        float offset = ((gx & 1) == 1) ? HALF_ROW : 0f;
        int gz = Mathf.RoundToInt((worldPos.z - offset) / ROW_HEIGHT);
        return (gx, gz);
    }

    /// <summary>
    /// 六方向邻格（奇偶列偏移不同）
    /// </summary>
    public static System.Collections.Generic.IEnumerable<(int gx, int gz)> GetNeighbors(int gx, int gz)
    {
        if ((gx & 1) == 0)
        {
            yield return (gx, gz + 1);       // 上
            yield return (gx, gz - 1);       // 下
            yield return (gx + 1, gz);       // 右上
            yield return (gx + 1, gz - 1);   // 右下
            yield return (gx - 1, gz);       // 左上
            yield return (gx - 1, gz - 1);   // 左下
        }
        else
        {
            yield return (gx, gz + 1);       // 上
            yield return (gx, gz - 1);       // 下
            yield return (gx + 1, gz + 1);   // 右上
            yield return (gx + 1, gz);       // 右下
            yield return (gx - 1, gz + 1);   // 左上
            yield return (gx - 1, gz);       // 左下
        }
    }

    /// <summary>
    /// 六边形格数距离（轴向坐标切比雪夫距离）
    /// </summary>
    public static int HexDistance(int gx1, int gz1, int gx2, int gz2)
    {
        // odd-q 偏移转轴向坐标：q = gx，r = gz - (gx - (gx&1)) / 2
        int q1 = gx1;
        int r1 = gz1 - (gx1 - (gx1 & 1)) / 2;
        int q2 = gx2;
        int r2 = gz2 - (gx2 - (gx2 & 1)) / 2;
        int s1 = -q1 - r1;
        int s2 = -q2 - r2;
        return Mathf.Max(Mathf.Abs(q1 - q2), Mathf.Abs(r1 - r2), Mathf.Abs(s1 - s2));
    }

    /// <summary>
    /// 两点世界坐标间的六边形格数距离
    /// </summary>
    public static int WorldDistance(Vector3 pos1, Vector3 pos2)
    {
        var (gx1, gz1) = WorldToGrid(pos1);
        var (gx2, gz2) = WorldToGrid(pos2);
        return HexDistance(gx1, gz1, gx2, gz2);
    }
}
