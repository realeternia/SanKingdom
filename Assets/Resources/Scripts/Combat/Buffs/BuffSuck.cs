public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        GameLog.Info("Suck " + damage.ToString());
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.AddHp((int)(damage * skillCfg.SkillDamageRate));
        EffectManager.PlaySkillEffect(owner, skillCfg.EffectHit);
    }
}