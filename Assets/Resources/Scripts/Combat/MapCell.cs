using System;
using System.Collections.Generic;

/// <summary>
/// 战斗地图格子，封装格子坐标与占用关系，一格最多一棋子
/// </summary>
[Serializable]
public class MapCell
{
    public int gridX;
    public int gridZ;
    public int chessId; // 占用该格的棋子ID，0表示空

    [NonSerialized]
    public List<CellEffect> effects = new List<CellEffect>();

    public void AddEffect(CellEffect effect)
    {
        effects.Add(effect);
        effect.CreateView(gridX, gridZ);
    }

    public MapCell(int gridX, int gridZ)
    {
        this.gridX = gridX;
        this.gridZ = gridZ;
        chessId = 0;
    }

    public bool IsOccupied()
    {
        return chessId != 0;
    }

    public void Occupy(int chessId)
    {
        if (IsOccupied() && this.chessId != chessId)
        {
            GameLog.Warn($"MapCell({gridX},{gridZ}) 已被 chessId={this.chessId} 占用，现被 chessId={chessId} 覆盖");
        }
        this.chessId = chessId;
    }

    public void Release()
    {
        chessId = 0;
    }
}
