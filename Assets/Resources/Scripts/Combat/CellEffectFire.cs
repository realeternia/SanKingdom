using System;

/// <summary>
/// 火系持续效果（火计/火墙/火矢生成的灼烧格）。
/// 火墙敌我不分：无论敌我，谁在格子里都会受到灼烧伤害。
/// </summary>
[Serializable]
public class CellEffectFire : CellEffect
{
    public override void Trigger()
    {
        var caster = BattleManager.Instance.GetChess(casterId);
        if (caster == null || caster.hp <= 0) return;

        var target = BattleManager.Instance.GetChessOnCell(cellId);
        if (target == null || target.hp <= 0) return;

        var damage = (int)(caster.GetAttr(attr) * damageRate);
        target.DoSkillDamage(caster, skillId, damage, false, 0);
    }
}
