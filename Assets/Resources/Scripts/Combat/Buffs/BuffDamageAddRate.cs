using System;

public class BuffDamageAddRate : Buff
{
    public BuffDamageAddRate(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti,  ref string effect)
    {
        if (damageBase < SystemConst.Battle.BUFF_MIN_DAMAGE_THRESHOLD)
        {
            damageBase = SystemConst.Battle.BUFF_MIN_DAMAGE_VALUE;
        }
        else
        {
            damageMulti += skillCfg.Strength;
        }
    }
}