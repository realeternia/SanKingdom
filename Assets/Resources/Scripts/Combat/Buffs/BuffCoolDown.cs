using System;

public class BuffCoolDown : Buff
{
    public BuffCoolDown(int id, int skillId, Chess caster, Chess target, int lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.Cooldown((int)(2 * skillCfg.Strength));
    }
}