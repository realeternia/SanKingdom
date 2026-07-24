using System;
using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 势力科技系统：统一管理科技解锁状态查询、加成计算、效果应用
/// 所有科技相关的查询与计算集中在此类，避免散落在 SysFormula/SaveForceData 等位置
/// </summary>
public static class ForceTech
{
    // ============================================================
    // 解锁状态查询
    // ============================================================

    public static bool HasTech(int forceId, int techId)
    {
        var force = GameManager.Instance.GetForce(forceId);
        return force != null && force.HasTech(techId);
    }

    public static List<int> GetUnlockedTechs(int forceId)
    {
        var force = GameManager.Instance.GetForce(forceId);
        return force != null && force.unlockedTechIds != null
            ? force.unlockedTechIds
            : new List<int>();
    }

    /// <summary>
    /// 判断指定科技是否可学习（前置条件满足）：
    /// Level 1 始终可学习；Level N 需同分类下至少一个 Level N-1 科技已解锁。
    /// 已解锁的科技视为不可学习（无需再研究）。
    /// </summary>
    public static bool IsTechLearnable(int forceId, int techId)
    {
        var techCfg = TechConfig.GetConfig(techId);

        // 已解锁则无需再学习
        if (HasTech(forceId, techId))
            return false;

        // Level 1 无前置条件
        if (techCfg.Level <= 1)
            return true;

        // Level N：需同分类下至少一个 Level N-1 科技已解锁
        var unlocked = GetUnlockedTechs(forceId);
        foreach (int unlockedId in unlocked)
        {
            var unlockedCfg = TechConfig.GetConfig(unlockedId);
            if (unlockedCfg.Category == techCfg.Category && unlockedCfg.Level == techCfg.Level - 1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取指定科技的已积累研究值
    /// </summary>
    public static int GetTechProgress(int forceId, int techId)
    {
        var force = GameManager.Instance.GetForce(forceId);
        return force != null ? force.GetTechProgress(techId) : 0;
    }

    // ============================================================
    // 核心查询：获取指定目标上的科技加成总量
    // ============================================================

    /// <summary>
    /// 获取指定势力在指定目标上的科技加成总量
    /// forceId: 势力ID
    /// targetId: TechSkillConfig.Target（CityDevConfig ID 或 ArmsConfig ID）
    /// enhanceType: AmountAdd/AmountMul/CostReduce/SlotAdd/SuccessMul/ArmsAttrAdd
    /// effectAttr: food/gold/soldier/happy/Atk/Def/MoveSpeed/rate/slot 等
    /// </summary>
    public static float GetTechBonus(int forceId, int targetId, string enhanceType, string effectAttr)
    {
        var force = GameManager.Instance.GetForce(forceId);
        if (force == null || force.unlockedTechIds == null || force.unlockedTechIds.Count == 0)
            return 0f;

        float total = 0f;
        foreach (int techId in force.unlockedTechIds)
        {
            var techCfg = TechConfig.GetConfig(techId);
            var skillCfg = TechSkillConfig.GetConfig(techCfg.SkillId);

            if (skillCfg.Target == targetId
                && skillCfg.EnhanceType == enhanceType
                && skillCfg.EffectAttr == effectAttr)
            {
                if (enhanceType == "AmountMul" || enhanceType == "CostReduce" || enhanceType == "SuccessMul")
                    total += techCfg.EffectValue[1];
                else
                    total += techCfg.EffectValue[0];
            }
        }
        return total;
    }

    // ============================================================
    // Dev 行动加成查询
    // ============================================================

    public static float GetDevAmountAdd(int forceId, int devId, string attrName)
    {
        return GetTechBonus(forceId, devId, "AmountAdd", attrName);
    }

    public static float GetDevAmountMul(int forceId, int devId, string attrName)
    {
        return GetTechBonus(forceId, devId, "AmountMul", attrName);
    }

    public static float GetDevCostReduce(int forceId, int devId)
    {
        return GetTechBonus(forceId, devId, "CostReduce", "gold");
    }

    public static int GetDevSlotAdd(int forceId, int devId)
    {
        return (int)GetTechBonus(forceId, devId, "SlotAdd", "slot");
    }

    public static float GetDevSuccessMul(int forceId, int devId)
    {
        return GetTechBonus(forceId, devId, "SuccessMul", "rate");
    }

    // ============================================================
    // KingAction 君令加成查询
    // ============================================================

    public static float GetKingActionSuccessMul(int forceId, int devId)
    {
        return GetTechBonus(forceId, devId, "SuccessMul", "rate");
    }

    public static float GetKingActionCostReduce(int forceId, int devId)
    {
        return GetTechBonus(forceId, devId, "CostReduce", "gold");
    }

    public static float GetKingActionAmountMul(int forceId, int devId, string effectAttr)
    {
        return GetTechBonus(forceId, devId, "AmountMul", effectAttr);
    }

    public static int GetKingActionSlotAdd(int forceId, int devId)
    {
        return (int)GetTechBonus(forceId, devId, "SlotAdd", "slot");
    }

    // ============================================================
    // Arms 兵种加成查询
    // ============================================================

    public static int GetArmsAttrAdd(int forceId, int armsId, string attrName)
    {
        return (int)GetTechBonus(forceId, armsId, "ArmsAttrAdd", attrName);
    }

    // ============================================================
    // 综合查询：Dev/KingAction 通用槽位数（基础HeroCount + 科技SlotAdd）
    // ============================================================

    /// <summary>
    /// 获取Dev行动的有效槽位数（基础HeroCount + 科技SlotAdd加成）
    /// </summary>
    public static int GetEffectiveSlotCount(int forceId, int devId)
    {
        var devCfg = CityDevConfig.GetConfig(devId);
        int baseCount = devCfg.HeroCount > 0 ? devCfg.HeroCount : 1;
        int techBonus = GetDevSlotAdd(forceId, devId) + GetKingActionSlotAdd(forceId, devId);
        return baseCount + techBonus;
    }

    // ============================================================
    // 效果应用工具
    // ============================================================

    /// <summary>
    /// 计算科技加成后的实际消耗
    /// </summary>
    public static int ApplyCostReduce(int baseCost, float reducePercent)
    {
        if (reducePercent <= 0f) return baseCost;
        return Math.Max(0, (int)(baseCost * (1f - reducePercent)));
    }

    /// <summary>
    /// 计算科技加成后的实际成功率
    /// </summary>
    public static int ApplySuccessMul(int baseRate, float mulPercent)
    {
        if (mulPercent <= 0f) return baseRate;
        return Math.Min(100, (int)(baseRate * (1f + mulPercent)));
    }

    /// <summary>
    /// 计算科技加成后的实际效果值
    /// </summary>
    public static float ApplyAmountMul(float baseValue, float mulPercent)
    {
        if (mulPercent <= 0f) return baseValue;
        return baseValue * (1f + mulPercent);
    }
}
