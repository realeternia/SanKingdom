using System;
using CommonConfig;

[Serializable]
public class WarTroopsData
{
    public int heroId1;
    public int heroId2;
    public int heroId3;
    public int armsId;
    public int cityId;
    public int soldierCount;

    public WarTroopsData()
    {
        heroId1 = 0;
        heroId2 = 0;
        heroId3 = 0;
        armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
        cityId = 0;
        soldierCount = 0;
    }

    public WarTroopsData(int heroId1, int heroId2, int heroId3, int armsId, int cityId)
    {
        this.heroId1 = heroId1;
        this.heroId2 = heroId2;
        this.heroId3 = heroId3;
        this.armsId = armsId;
        this.cityId = cityId;
        soldierCount = 0;
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
        if (force == null) return false;

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
        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData != null) return cityData.forceId;

        if (heroId1 > 0)
        {
            var hero = GameManager.Instance.GetHero(heroId1);
            if (hero != null) return hero.forceId;
        }
        return 0;
    }
}
