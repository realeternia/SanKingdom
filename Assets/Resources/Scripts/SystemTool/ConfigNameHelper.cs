using System.Linq;
using CommonConfig;

public static class ConfigNameHelper
{
    public static string GetForceName(int forceId)
    {
        var cfg = ForceConfig.GetConfig(forceId);
        return cfg != null ? cfg.Cname : forceId.ToString();
    }

    public static string GetHeroName(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        return cfg != null ? cfg.Name : heroId.ToString();
    }

    public static string GetCityName(int cityId)
    {
        var cfg = WorldConfig.GetConfig(cityId);
        return cfg != null ? cfg.Cname : cityId.ToString();
    }

    public static string GetHeroNames(int[] heroIds)
    {
        return string.Join(",", heroIds.Select(GetHeroName));
    }
}
