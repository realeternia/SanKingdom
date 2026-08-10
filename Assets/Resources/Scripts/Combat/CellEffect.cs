using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 地图格子上的持续效果（如火墙、雷电阵），每回合对格子内的敌方单位结算伤害，自管理过期与视觉特效生命周期
/// </summary>
[Serializable]
public class CellEffect
{
    public int cellId; // 所属格子ID，由 MapCell.AddEffect 在加入格子时赋值
    public int skillId;
    public int casterId;
    public int forceId;
    public string attr;
    public float damageRate;
    public int endRound;

    /// <summary>
    /// 视觉特效对象，[NonSerialized] 回放时由 AddCellEffectAction 重建
    /// </summary>
    [NonSerialized]
    public GameObject viewEffect;

    /// <summary>
    /// 是否已过期需从格子移除
    /// </summary>
    public bool IsExpired(int round)
    {
        return round > endRound;
    }

    /// <summary>
    /// 每回合结算：若格子被敌方单位占用则对其造成伤害
    /// </summary>
    public void Trigger()
    {
        var caster = BattleManager.Instance.GetChess(casterId);
        if (caster == null || caster.hp <= 0) return;

        var cell = BattleManager.Instance.GetMapCellById(cellId);
        if (cell == null || !cell.IsOccupied()) return;

        var target = BattleManager.Instance.GetChess(cell.chessId);
        if (target == null || target.forceId == forceId || target.hp <= 0) return;

        var damage = (int)(caster.GetAttr(attr) * damageRate);
        target.DoSkillDamage(caster, skillId, damage, false, 0);
    }

    /// <summary>
    /// 创建持久视觉特效，生命周期与 CellEffect 一致；quickMode 或已存在则跳过
    /// </summary>
    public void CreateView()
    {
        if (viewEffect != null) return;
        var cell = BattleManager.Instance.GetMapCellById(cellId);
        if (cell == null) return;
        var cfg = BattleSkillConfig.GetConfig(skillId);
        var worldPos = BattleManager.Instance.GridCoordToWorld(cell.gridX, cell.gridZ);
        viewEffect = EffectManager.PlayPosSkillEffect(null, worldPos, cfg.EffectSize, cfg.EffectArea, 0f);
    }

    /// <summary>
    /// 移除视觉特效，供 CellEffect 过期或地图重置时调用
    /// </summary>
    public void DestroyView()
    {
        if (viewEffect != null)
        {
            UnityEngine.Object.Destroy(viewEffect);
            viewEffect = null;
        }
    }
}
