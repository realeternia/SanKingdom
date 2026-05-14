public class BuffLock : Buff
{
    public BuffLock(int id, int skillId, Chess caster, Chess target, int lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        var caster = BattleManager.Instance.GetChess(casterId);
        var owner = BattleManager.Instance.GetChess(ownerId);
        var unitList = BattleManager.Instance.GetUnitsInRange(owner.position, skillCfg.Range * 3, caster.forceId, true);
        GameLog.Info("Lock target count: " + unitList.Count);
        foreach (var unit in unitList)
        {
            if (unit.HasBuff(id) && unit != owner)
                unit.DoSkillDamage(caster, skillCfg.Id, (int)(damage * skillCfg.SkillDamageRate));
        }

    }

}