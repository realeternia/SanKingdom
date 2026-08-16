using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 地图格子上的持续效果基类（虚基类），管理格子归属、过期与视觉特效生命周期。
/// 具体效果通过派生类定制触发与伤害逻辑（如火墙 CellEffectFire），创建统一走静态工厂 Create。
/// </summary>
[Serializable]
public class CellEffect
{
    public int cellId; // 所属格子ID，由 MapCell.AddEffect 在加入格子时赋值
    public int skillId;
    public int casterId;
    public int forceId;
    public string attr;      // 伤害属性名（如 inte/str），供伤害计算使用
    public float damageRate; // 属性伤害倍率
    public int endRound;

    /// <summary>
    /// 视觉特效对象，[NonSerialized] 回放时由 AddCellEffectAction 重建
    /// </summary>
    [NonSerialized]
    public GameObject viewEffect;

    /// <summary>
    /// 静态工厂：按类型名创建对应的格子效果实例并填充公共字段。
    /// 传 "Fire" 创建 CellEffectFire（火计/火墙/火矢，敌我不分），传空串或其他类型使用通用基类。
    /// </summary>
    public static CellEffect Create(string type, BattleSkillConfig skillCfg, Chess caster, int endRound)
    {
        var effect = type == "Fire" ? new CellEffectFire() : new CellEffect();
        effect.skillId = skillCfg.Id;
        effect.casterId = caster.id;
        effect.forceId = caster.forceId;
        effect.attr = skillCfg.Attr;
        effect.damageRate = skillCfg.SkillDamageAttrRate;
        effect.endRound = endRound;
        return effect;
    }

    /// <summary>
    /// 是否已过期需从格子移除
    /// </summary>
    public virtual bool IsExpired(int round)
    {
        return round > endRound;
    }

    /// <summary>
    /// 每回合结算：若格子被敌方单位占用则对其造成伤害（含不占格的城门/箭塔/城墙）。
    /// 默认只伤害敌方单位，派生类可覆盖为敌我不分（如 CellEffectFire 的火墙灼烧）。
    /// </summary>
    public virtual void Trigger()
    {
        var caster = BattleManager.Instance.GetChess(casterId);
        if (caster == null || caster.hp <= 0) return;

        var target = BattleManager.Instance.GetChessOnCell(cellId);
        if (target == null || target.forceId == forceId || target.hp <= 0) return;

        var damage = (int)(caster.GetAttr(attr) * damageRate);
        target.DoSkillDamage(caster, skillId, damage, false, 0);
    }

    /// <summary>
    /// 创建持久视觉特效，生命周期与效果一致；已存在则跳过
    /// </summary>
    public virtual void CreateView()
    {
        if (viewEffect != null) return;
        var cell = BattleManager.Instance.GetMapCellById(cellId);
        if (cell == null) return;
        var cfg = BattleSkillConfig.GetConfig(skillId);
        var worldPos = BattleManager.Instance.GridCoordToWorld(cell.gridX, cell.gridZ);
        viewEffect = EffectManager.PlayPosSkillEffect(null, worldPos, cfg.EffectSize, cfg.EffectArea, 0f);
    }

    /// <summary>
    /// 移除视觉特效，供效果过期或地图重置时调用
    /// </summary>
    public virtual void DestroyView()
    {
        if (viewEffect != null)
        {
            UnityEngine.Object.Destroy(viewEffect);
            viewEffect = null;
        }
    }
}
