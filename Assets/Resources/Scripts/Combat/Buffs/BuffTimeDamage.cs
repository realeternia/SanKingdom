using UnityEngine;

public class BuffTimeDamage : Buff
{
    public float damage;

    public BuffTimeDamage(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        damage = caster.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate;
        
        // 添加持续伤害状态
        chess.AddDamageOverTimeState(caster.id, skillCfg.Id, damage);
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        
        // 移除持续伤害状态
        chess.RemoveDamageOverTimeState(skillCfg.Id);
    }

}
