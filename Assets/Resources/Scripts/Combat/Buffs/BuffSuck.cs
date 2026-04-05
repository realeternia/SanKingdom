using Controls.Utils;

public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, int lastTime)
     : base(id, skillId, caster, target, lastTime)
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