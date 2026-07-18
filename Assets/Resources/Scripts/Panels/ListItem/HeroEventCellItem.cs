using System.Collections.Generic;
using CommonConfig;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;

public class HeroEventCellItem : MonoBehaviour, ILoopScrollItem
{
    public TMP_Text text;

    void Start()
    {
        if (text != null) text.raycastTarget = false;
    }

    public void BindData(object data)
    {
        if (data is GameEventData eventData)
        {
            text.text = FormatEvent(eventData);
        }
        else if (data is string str)
        {
            text.text = str;
        }
    }

    private string FormatEvent(GameEventData ev)
    {
        int year = ev.year;
        string seasonName = GetSeasonName(ev.round);
        string timeLabel = $"{year}年{seasonName}";

        switch (ev.eventType)
        {
            case GameEventType.BattleAttack:
                {
                    string target = GetCityName(ev.cityId);
                    string defenders = GetForceName(ev.relatedForceId);
                    return $"<color=#FFD700>{timeLabel}</color>  进攻 <color=#FF6B6B>{defenders}</color> 的 <color=#87CEEB>{target}</color>";
                }
            case GameEventType.BattleDefend:
                {
                    string target = GetCityName(ev.cityId);
                    string attacker = GetForceName(ev.relatedForceId);
                    return $"<color=#FFD700>{timeLabel}</color>  防御 <color=#FF6B6B>{attacker}</color> 来犯于 <color=#87CEEB>{target}</color>";
                }
            case GameEventType.BattleResult:
                {
                    string target = GetCityName(ev.cityId);
                    bool win = ev.intParam == 1;
                    string result = win ? "<color=#7CFC00>胜</color>" : "<color=#FF6B6B>败</color>";
                    return $"<color=#FFD700>{timeLabel}</color>  战斗{result}于 <color=#87CEEB>{target}</color>";
                }
            case GameEventType.Dev:
                {
                    string action = ev.intParam == 0 ? "就任" : ev.intParam == 1 ? "卸任" : "调任";
                    string devName = GetDevName(ev.devId);
                    return $"<color=#FFD700>{timeLabel}</color>  {action} <color=#B0C4DE>{devName}</color>";
                }
            case GameEventType.KingActionMove:
                {
                    string dest = GetCityName(ev.intParam);
                    return $"<color=#FFD700>{timeLabel}</color>  移动至 <color=#87CEEB>{dest}</color>";
                }
            case GameEventType.KingActionTrade:
                {
                    bool buy = ev.intParam == 1;
                    string gain = ev.effectValue > 0 ? $" <color=#7CFC00>+{ev.effectValue}</color>" : "";
                    return $"<color=#FFD700>{timeLabel}</color>  交易 <color=#B0C4DE>{(buy ? "买兵" : "卖粮")}</color>{gain}";
                }
            case GameEventType.KingActionSearch:
                {
                    string cityName = GetCityName(ev.cityId);
                    string resultDesc = ev.effectValue switch
                    {
                        1 => $" <color=#7CFC00>发现城市资源+{ev.effectValue2}</color>",
                        2 => $" <color=#7CFC00>发现势力资源+{ev.effectValue2}</color>",
                        3 => $" <color=#7CFC00>发现{GetHeroNames(ev.relatedHeroIds)}</color>",
                        4 => $" <color=#7CFC00>发现名将{GetHeroNames(ev.relatedHeroIds)}</color>",
                        _ => ""
                    };
                    return $"<color=#FFD700>{timeLabel}</color>  于 <color=#87CEEB>{cityName}</color> 搜索{resultDesc}";
                }
            case GameEventType.KingActionRecruit:
                {
                    bool success = ev.intParam == 1;
                    string result = success ? "<color=#7CFC00>成功</color>" : "<color=#FF6B6B>失败</color>";
                    return $"<color=#FFD700>{timeLabel}</color>  登庸{result}";
                }
            case GameEventType.KingActionPraise:
                {
                    string method = ev.intParam == 2 ? "奖赏" : "褒奖";
                    string loyalty = ev.effectValue != 0 ? $" <color=#7CFC00>忠心+{ev.effectValue}</color>" : "";
                    return $"<color=#FFD700>{timeLabel}</color>  受{method}{loyalty}";
                }
            case GameEventType.KingActionDestroy:
                {
                    string target = GetCityName(ev.intParam);
                    string wall = ev.effectValue != 0 ? $" <color=#FF6B6B>城防-{ev.effectValue}</color>" : "";
                    return $"<color=#FFD700>{timeLabel}</color>  破坏 <color=#87CEEB>{target}</color>{wall}";
                }
            case GameEventType.KingActionDisturb:
                {
                    string target = GetCityName(ev.intParam);
                    string happy = ev.effectValue != 0 ? $" <color=#FF6B6B>民心-{ev.effectValue}</color>" : "";
                    string loyalty = ev.effectValue2 != 0 ? $" <color=#FF6B6B>忠心-{ev.effectValue2}</color>" : "";
                    return $"<color=#FFD700>{timeLabel}</color>  扰乱 <color=#87CEEB>{target}</color>{happy}{loyalty}";
                }
            case GameEventType.LoyaltyChange:
                {
                    string reason = ev.intParam == 0 ? "被扰乱" : "被俘虏";
                    string loyalty = ev.effectValue < 0
                        ? $" <color=#FF6B6B>忠心{ev.effectValue}</color>"
                        : $" <color=#7CFC00>忠心+{ev.effectValue}</color>";
                    return $"<color=#FFD700>{timeLabel}</color>  {reason}{loyalty}";
                }
            case GameEventType.Capture:
                {
                    string captor = GetForceName(ev.forceId);
                    return $"<color=#FFD700>{timeLabel}</color>  被 <color=#FF6B6B>{captor}</color> 俘虏";
                }
            case GameEventType.Wild:
                {
                    string cityName = GetCityName(ev.cityId);
                    return $"<color=#FFD700>{timeLabel}</color>  于 <color=#87CEEB>{cityName}</color> 被发现（在野）";
                }
            case GameEventType.Escape:
                {
                    string dest = GetCityName(ev.intParam);
                    return $"<color=#FFD700>{timeLabel}</color>  逃脱至 <color=#87CEEB>{dest}</color>";
                }
            case GameEventType.RecruitSuccess:
                {
                    string oldForce = GetForceName(ev.relatedForceId);
                    string newForce = GetForceName(ev.forceId);
                    return $"<color=#FFD700>{timeLabel}</color>  由 <color=#B0C4DE>{oldForce}</color> 投效 <color=#7CFC00>{newForce}</color>";
                }
            default:
                return $"<color=#FFD700>{timeLabel}</color>  未知事件";
        }
    }

    private string GetSeasonName(int round)
    {
        int seasonId = (int)SysFormula.Game.CalculateSeasonId(round);
        var seasonCfg = SeasonConfig.GetConfig(seasonId);
        return seasonCfg != null ? seasonCfg.Name : "";
    }

    private string GetCityName(int cityId)
    {
        if (cityId <= 0) return "未知";
        var cfg = WorldConfig.GetConfig(cityId);
        return cfg != null ? cfg.Cname : "未知";
    }

    private string GetForceName(int forceId)
    {
        if (forceId <= 0) return "在野";
        var cfg = ForceConfig.GetConfig(forceId);
        return cfg != null ? cfg.Cname : "未知";
    }

    private string GetDevName(int devId)
    {
        if (devId <= 0) return "未知职务";
        var cfg = CityDevConfig.GetConfig(devId);
        return cfg != null ? cfg.Des : "未知职务";
    }

    private string GetHeroNames(List<int> heroIds)
    {
        if (heroIds == null || heroIds.Count == 0) return "";
        var names = new List<string>();
        foreach (int id in heroIds)
        {
            var cfg = HeroConfig.GetConfig(id);
            if (cfg != null) names.Add(cfg.Name);
        }
        return string.Join("、", names);
    }

    public void OnReturnToPool()
    {
    }
}
