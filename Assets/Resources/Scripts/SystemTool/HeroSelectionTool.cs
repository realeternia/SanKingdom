using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

// 定义一个单独的工具类
public static class HeroSelectionTool
{
    private static int[] cardHeroExp = new int[] { 0, 2, 4, 7, 11, 16, 22, 29, 37, 46, 56, 66, 76, 86, 96, 106, 116, 126, 136, 146, 156, 166, 176, 186, 196, 206, 216, 226, 236, 246, 256, 266, 276, 999 };
    private static int[] cardItemExp = new int[] { 0, 2, 4, 6, 9, 12, 15, 19, 23, 27, 31, 36, 41, 46, 51, 56, 62, 68, 74, 80, 86, 92, 98, 104, 110, 116, 122, 128, 136, 142, 148, 154, 160, 166, 172, 178, 184, 190, 196, 202, 999 }; //生成后续数据
    public static int GetCardLevel(int exp, bool isHero)
    {
        if(isHero)
        {
            for(int i = 0; i < cardHeroExp.Length; i++)
            {
                if(exp < cardHeroExp[i])
                    return i;
            }
            return cardHeroExp.Length;
        }
        else
        {
            for(int i = 0; i < cardItemExp.Length; i++)
            {
                if(exp < cardItemExp[i])
                    return i;
            }
            return cardItemExp.Length;
        }
    }

    public static float GetExpRate(int exp, bool isHero)
    {
        int level = GetCardLevel(exp, isHero);
        if(level >= cardHeroExp.Length)
            return 1f;
        if(level <= 1)
            return 0;
        if(isHero)
            return (float)(exp - cardHeroExp[level - 1]) / (cardHeroExp[level] - cardHeroExp[level - 1]);
        else
            return (float)(exp - cardItemExp[level - 1]) / (cardItemExp[level] - cardItemExp[level - 1]);
    }

    public static AttrInfo GetCardAttr(int cardId, int lv)
    {
        var attrInfo = new AttrInfo();
        if (ConfigManager.IsHeroCard(cardId))
        {
            var heroConfig = HeroConfig.GetConfig(cardId);
            var heroData = GameManager.Instance?.GetHero(cardId);
            int baseStr = heroData != null && heroData.str > 0 ? heroData.str : heroConfig.Str;
            int baseInte = heroData != null && heroData.inte > 0 ? heroData.inte : heroConfig.Inte;
            int baseLead = heroData != null && heroData.leadShip > 0 ? heroData.leadShip : heroConfig.LeadShip;

            attrInfo.Inte = baseInte + System.Math.Max(SystemConst.Hero.MIN_ATTR_PER_LEVEL * (lv - 1), baseInte * (lv - 1) / SystemConst.Hero.ATTR_GROWTH_DIVISOR);
            attrInfo.Str = baseStr + System.Math.Max(SystemConst.Hero.MIN_ATTR_PER_LEVEL * (lv - 1), baseStr * (lv - 1) / SystemConst.Hero.ATTR_GROWTH_DIVISOR);
            attrInfo.Lead = baseLead + System.Math.Max(SystemConst.Hero.MIN_ATTR_PER_LEVEL * (lv - 1), baseLead * (lv - 1) / SystemConst.Hero.ATTR_GROWTH_DIVISOR);
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(cardId);
            if (itemConfig.Attr1 == "str")
            {
                attrInfo.Str = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "inte")
            {
                attrInfo.Inte = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "lead")
            {
                attrInfo.Lead = itemConfig.Attr1Val;
            }

            if (itemConfig.Attr2 == "str")
            {
                attrInfo.Str = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "inte")
            {
                attrInfo.Inte = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "lead")
            {
                attrInfo.Lead = itemConfig.Attr2Val;
            }

            attrInfo.Inte = attrInfo.Inte * lv;
            attrInfo.Str = attrInfo.Str * lv;
            attrInfo.Lead = attrInfo.Lead * lv;
        }

        return attrInfo;

    }
 
}
