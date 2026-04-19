using System;

public class BuffDamagedAddRate : Buff
{
    public BuffDamagedAddRate(int id, int skillId, Chess caster, Chess target, int lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
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