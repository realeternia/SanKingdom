using System.Collections.Generic;
using CommonConfig;
using Controls.Utils;

public enum HeroState
{
    Normal, // 正常
    Wild,   // 在野
    Catched // 被俘虏
}

[System.Serializable]
public class SaveHeroData
{
    public int heroId;
    public int exp;

    public int cityId;
    public HeroState state;
    public int loyalty;
    public int forceId;
    public int armsId;

    public int str;
    public int inte;
    public int fair;
    public int charm;
    public int leadShip;

    public void InitAttrsFromConfig()
    {
        var heroConfig = HeroConfig.GetConfig(heroId);
        if (heroConfig == null) 
            return;
        if (str == 0) str = heroConfig.Str;
        if (inte == 0) inte = heroConfig.Inte;
        if (fair == 0) fair = heroConfig.Fair;
        if (charm == 0) charm = heroConfig.Charm;
        if (leadShip == 0) leadShip = heroConfig.LeadShip;
    }

    public int GetAttr(string attr)
    {
        InitAttrsFromConfig();
        switch (attr.ToLower())
        {
            case "str":
                return str;
            case "inte":
                return inte;
            case "fair":
                return fair;
            case "leadship":
                return leadShip;
            case "charm":
                return charm;
            default:
                return 0;
        }
    }

    public int GetLevel()
    {
        return HeroSelectionTool.GetCardLevel(exp, true);
    }

    public bool SetArmsId(int newArmsId)
    {
        if (state != HeroState.Normal)
        {
            GameLog.Warn($"SetArmsId: 英雄 {heroId} 状态不是 Normal，无法设置兵种");
            return false;
        }
        
        if (forceId <= 0)
        {
            GameLog.Warn($"SetArmsId: 英雄 {heroId} 不属于任何势力，无法设置兵种");
            return false;
        }
        
        var force = GameManager.Instance.GetForce(forceId);
        if (force == null)
        {
            GameLog.Warn($"SetArmsId: 英雄 {heroId} 的势力 {forceId} 不存在");
            return false;
        }
        
        if (!force.CanAffordArms(newArmsId, heroId))
        {
            GameLog.Warn($"SetArmsId: 势力 {forceId} 资源不足，无法为英雄 {heroId} 设置兵种 {newArmsId}");
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
        
        GameLog.Info($"SetArmsId: 英雄 {heroId} 成功设置兵种为 {newArmsId}");
        return true;
    }

    public static SaveHeroData CreateWildHero(int heroId, int cityId)
    {
        SaveHeroData newHero = new SaveHeroData();
        newHero.heroId = heroId;
        newHero.exp = 0;
        newHero.cityId = cityId;
        newHero.state = HeroState.Wild;
        newHero.loyalty = SystemConst.Hero.WILD_HERO_DEFAULT_LOYALTY;
        newHero.forceId = SystemConst.Hero.WILD_FORCE_ID;
        newHero.armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
        newHero.InitAttrsFromConfig();
        return newHero;
    }

}