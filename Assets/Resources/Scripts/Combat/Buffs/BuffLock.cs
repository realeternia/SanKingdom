public class BuffLock : Buff
{
    public BuffLock(int id, int skillId, Chess caster, Chess target, int lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        var unitList = BattleManager.Instance.GetUnitsInRange(owner.position, skillCfg.Range * 3, caster.forceId, true);
        UnityEngine.Debug.Log("Lock target count: " + unitList.Count);
        foreach (var unit in unitList)
        {
            if (unit.HasBuff(id) && unit != owner)
                unit.OnSkillDamaged(caster, skillCfg.Id, (int)(damage * skillCfg.SkillDamageRate));
        }

    }

}