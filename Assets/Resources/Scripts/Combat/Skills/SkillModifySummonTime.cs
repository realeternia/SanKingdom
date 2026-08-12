using System;
using CommonConfig;

public class SkillModifySummonTime : BattleSkill
{
    public SkillModifySummonTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckSummonRoundCount(BattleSkillConfig checkSkillCfg, ref int roundCount)
    {
        if (checkSkillCfg.SummonRoundCount <= 0)
            return;

        if(checkSkillCfg.SummonTag != skillCfg.SummonTag)
            return;

        roundCount += skillCfg.SummonRoundCount;
    }

}
