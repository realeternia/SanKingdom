using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;

[Serializable]
public class SaveTroopsData
{
    public int cityId;
    public int heroId1;
    public int heroId2;
    public int heroId3;
    public int armsId;

    public SaveTroopsData()
    {
        cityId = 0;
        heroId1 = 0;
        heroId2 = 0;
        heroId3 = 0;
        armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
    }

    public SaveTroopsData(int heroId1, int heroId2, int heroId3, int armsId)
    {
        this.cityId = 0;
        this.heroId1 = heroId1;
        this.heroId2 = heroId2;
        this.heroId3 = heroId3;
        this.armsId = armsId;
    }

    public bool SetArmsId(int newArmsId)
    {
        if (heroId1 <= 0)
        {
            SystemTip.Instance.ShowTip("请先设置主将");
            return false;
        }

        int forceId = GetForceId();
        if (forceId <= 0) return false;

        var force = GameManager.Instance.GetForce(forceId);
        if (!force.CanAffordArms(newArmsId, this))
        {
            SystemTip.Instance.ShowTip("资源不足");
            return false;
        }

        int oldArmsId = armsId;
        var oldArmsConfig = oldArmsId > 0 ? ArmsConfig.GetConfig(oldArmsId) : null;
        var newArmsConfig = ArmsConfig.GetConfig(newArmsId);

        armsId = newArmsId;
        force.RecalculateResUsed();

        if (oldArmsConfig == null || oldArmsConfig.HorseCost != newArmsConfig.HorseCost)
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "horse", Value = force.GetAttr("horse"), Used = force.GetResUsed("horse") });
        if (oldArmsConfig == null || oldArmsConfig.SteelCost != newArmsConfig.SteelCost)
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "steel", Value = force.GetAttr("steel"), Used = force.GetResUsed("steel") });
        if (oldArmsConfig == null || oldArmsConfig.WoodCost != newArmsConfig.WoodCost)
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "wood", Value = force.GetAttr("wood"), Used = force.GetResUsed("wood") });
        if (oldArmsConfig == null || oldArmsConfig.StoneCost != newArmsConfig.StoneCost)
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "stone", Value = force.GetAttr("stone"), Used = force.GetResUsed("stone") });

        return true;
    }

    public void ReleaseResources()
    {
        int forceId = GetForceId();
        if (forceId <= 0) return;

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        force.RecalculateResUsed();

        PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "horse", Value = force.GetAttr("horse"), Used = force.GetResUsed("horse") });
        PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "steel", Value = force.GetAttr("steel"), Used = force.GetResUsed("steel") });
        PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "wood", Value = force.GetAttr("wood"), Used = force.GetResUsed("wood") });
        PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = "stone", Value = force.GetAttr("stone"), Used = force.GetResUsed("stone") });
    }

    private int GetForceId()
    {
        var hero = GameManager.Instance.GetHero(heroId1);
        return hero.forceId;
    }

    public static SaveTroopsData FindByHeroId(int heroId)
    {
        return GameManager.Instance.SaveData.troops.FirstOrDefault(t =>
            t.heroId1 == heroId || t.heroId2 == heroId || t.heroId3 == heroId);
    }

    public static List<SaveTroopsData> GetTroopsByCity(int cityId)
    {
        return GameManager.Instance.SaveData.troops.Where(t => t.cityId == cityId).ToList();
    }

    public static void AddTroopToCity(SaveTroopsData troop, int cityId)
    {
        troop.cityId = cityId;
        GameManager.Instance.SaveData.troops.Add(troop);
    }

    public static void RemoveTroopFromCity(SaveTroopsData troop)
    {
        GameManager.Instance.SaveData.troops.Remove(troop);
    }

    public static void RemoveAllTroopsByCity(int cityId)
    {
        GameManager.Instance.SaveData.troops.RemoveAll(t => t.cityId == cityId);
    }

    public static int GetTroopsCountByCity(int cityId)
    {
        return GameManager.Instance.SaveData.troops.Count(t => t.cityId == cityId);
    }

    public static void MoveTroopsToCity(List<SaveTroopsData> troopsToMove, int destCityId)
    {
        foreach (var troop in troopsToMove)
        {
            troop.cityId = destCityId;
        }
    }

    public static bool IsHeroCommander(int heroId, int cityId)
    {
        return GameManager.Instance.SaveData.troops.Any(t => t.cityId == cityId && t.heroId1 == heroId);
    }

    public static bool IsHeroViceCommander(int heroId, int cityId)
    {
        return GameManager.Instance.SaveData.troops.Any(t => t.cityId == cityId && (t.heroId2 == heroId || t.heroId3 == heroId));
    }

    public static bool IsHeroInTroop(int heroId, int cityId)
    {
        return GameManager.Instance.SaveData.troops.Any(t =>
            t.cityId == cityId && (t.heroId1 == heroId || t.heroId2 == heroId || t.heroId3 == heroId));
    }
}
