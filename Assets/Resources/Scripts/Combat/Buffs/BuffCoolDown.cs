using System;

public class BuffCoolDown : Buff
{
    public BuffCoolDown(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.Cooldown((int)(2 * skillCfg.Strength));
    }
}