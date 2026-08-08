using System;
using CommonConfig;

public class SkillModifySummonTime : BattleSkill
{
    public SkillModifySummonTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckSummonTime(BattleSkillConfig checkSkillCfg, ref float summonTime)
    {
        if (checkSkillCfg.SummonTime <= 0)
            return;
        
        if(checkSkillCfg.SummonTag != skillCfg.SummonTag)
            return;
        
        summonTime += skillCfg.SummonTime;
    }

}
