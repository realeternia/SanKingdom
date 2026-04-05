using CommonConfig;
using Controls.Utils;

public static class BuffManager
{
    public static void AddBuff(Chess target, Chess caster, int skillId, int buffId, float lastTime)
    {
        SkillManager.OnAddBuff(target, caster, ref buffId, skillId, ref lastTime);

        if(lastTime == 0) //有的技能会先填0，等待buff
            return;

        var action = new AddBuffAction(target.id, BattleManager.Instance.tickIndex, caster.id, skillId, buffId, lastTime);
        BattleManager.Instance.AddChessAction(action);
    }

    public static void DoAddBuff(Chess target, Chess caster, int skillId, int buffId, float lastTime)
    {
        var endTick = BattleManager.Instance.tickIndex + BattleManager.Instance.GetTickFromTime(lastTime);
        
       // UnityEngine.Debug.Log("AddBuff buffId=" + buffId.ToString() + " skillId=" + skillId.ToString() + " time=" + time.ToString());

        Buff buff = null;
        var buffCfg = BuffConfig.GetConfig(buffId);
        switch (buffCfg.ScriptName)
        {
            case "BuffShield":
                buff = new BuffShield(buffId, skillId, caster, target, endTick);
                break;
            case "BuffShieldValue":
                buff = new BuffShieldValue(buffId, skillId, caster, target, endTick);
                break;
            case "BuffCoolDown":
                buff = new BuffCoolDown(buffId, skillId, caster, target, endTick);
                break; 
            case "BuffNoAction":
                buff = new BuffNoAction(buffId, skillId, caster, target, endTick);
                break;
            case "BuffNoMove":
                buff = new BuffNoMove(buffId, skillId, caster, target, endTick);    
                break;
            case "BuffLock":
                buff = new BuffLock(buffId, skillId, caster, target, endTick);
                break;
            case "BuffSuck":
                buff = new BuffSuck(buffId, skillId, caster, target, endTick);
                break;
            case "BuffDamageAddRate":
                buff = new BuffDamageAddRate(buffId, skillId, caster, target, endTick);
                break;                
            case "BuffDamagedAddRate":
                buff = new BuffDamagedAddRate(buffId, skillId, caster, target, endTick);
                break;
            case "BuffSpeedDown":
                buff = new BuffSpeedDown(buffId, skillId, caster, target, endTick);
                break;
            case "BuffTimeDamage":
                buff = new BuffTimeDamage(buffId, skillId, caster, target, endTick);
                break;

        }

        if (buff == null)
        {
            GameLog.Error("Buff not found");
            return;
        }

        target.AddBuff(buff, caster, endTick);
    }

    public static void RemoveBuff(Chess chess, int buffId)
    {
        var action = new RemoveBuffAction(chess.id, BattleManager.Instance.tickIndex, buffId);
        BattleManager.Instance.AddChessAction(action);
    }

    public static void DoRemoveBuff(Chess chess, int buffId)
    {
        for(int i = 0; i < chess.buffs.Count; i++)
        {
            if(chess.buffs[i].id == buffId)
            {
                chess.buffs[i].OnRemove(chess);
                chess.buffs.RemoveAt(i);
                break;
            }
        }
    }

}