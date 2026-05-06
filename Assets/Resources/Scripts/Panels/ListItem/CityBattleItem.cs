using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityBattleItem : MonoBehaviour
{
    public TMP_Text heroNameText;
    public TMP_Text armsText;
    public TMP_Text targetCityText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text sodierCountText;
    public Image hero1IconImage;
    public Image hero2IconImage;
    public Image hero3IconImage;

    public Button editButton;

    private WarTeamData warTeamData;

    public void Init(WarTeamData data)
    {
        warTeamData = data;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (warTeamData == null) return;

        if (heroNameText != null)
        {
            string heroName = "";
            if (warTeamData.heroId1 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId1);
                heroName = heroConfig.Name;
            }
            heroNameText.text = heroName;
        }

        if (armsText != null)
        {
            if (warTeamData.armsId > 0)
            {
                var armsConfig = ArmsConfig.GetConfig(warTeamData.armsId);
                armsText.text = armsConfig.NameS;
            }
            else
            {
                armsText.text = "";
            }
        }

        if (targetCityText != null)
        {
            if (warTeamData.targetCityId > 0)
            {
                targetCityText.text = ConfigNameHelper.GetCityName(warTeamData.targetCityId);
            }
            else
            {
                targetCityText.text = "准备中";
            }
        }

        if (atkText != null)
        {
            if (warTeamData.armsId > 0)
            {
                var armsConfig = ArmsConfig.GetConfig(warTeamData.armsId);
                atkText.text = armsConfig.Atk.ToString();
            }
            else
            {
                atkText.text = "0";
            }
        }

        if (defText != null)
        {
            if (warTeamData.armsId > 0)
            {
                var armsConfig = ArmsConfig.GetConfig(warTeamData.armsId);
                defText.text = armsConfig.Def.ToString();
            }
            else
            {
                defText.text = "0";
            }
        }

        if (hero1IconImage != null)
        {
            hero1IconImage.gameObject.SetActive(warTeamData.heroId1 > 0);
            if (warTeamData.heroId1 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId1);
                SetHeroIcon(hero1IconImage, heroConfig.Icon);
            }
        }

        if (hero2IconImage != null)
        {
            hero2IconImage.gameObject.SetActive(warTeamData.heroId2 > 0);
            if (warTeamData.heroId2 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId2);
                SetHeroIcon(hero2IconImage, heroConfig.Icon);
            }
        }

        if (hero3IconImage != null)
        {
            hero3IconImage.gameObject.SetActive(warTeamData.heroId3 > 0);
            if (warTeamData.heroId3 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId3);
                SetHeroIcon(hero3IconImage, heroConfig.Icon);
            }
        }
    }

    private void SetHeroIcon(Image iconImage, string iconName)
    {
        if (iconImage == null || string.IsNullOrEmpty(iconName)) return;
        string iconPath = "Textures/Skins/" + iconName;
        Sprite sprite = Resources.Load<Sprite>(iconPath);
        if (sprite != null)
        {
            iconImage.sprite = sprite;
        }
    }

    public WarTeamData GetWarTeamData()
    {
        return warTeamData;
    }
}
