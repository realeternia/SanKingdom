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
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text sodierCountText;
    public Image hero1IconImage;
    public Image hero2IconImage;
    public Image hero3IconImage;

    public Button editButton;
    public Image bgImage;
    public Button itemButton;

    private SaveTroopsData warTeamData;
    private bool isSelected = false;
    private bool hasActed = false;

    public void Init(SaveTroopsData data, bool hasActed = false)
    {
        warTeamData = data;
        this.hasActed = hasActed;

        if (editButton != null)
        {
            editButton.onClick.RemoveAllListeners();
            editButton.onClick.AddListener(OnEdit);
        }

        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnItemClick);
        }

        isSelected = false;
        UpdateBgColor();
        RefreshUI();
    }

    private void OnItemClick()
    {
        var panelManager = GetComponentInParent<CityBattlePanelManager>();
        if (panelManager == null || !panelManager.CanSelectItem()) return;

        isSelected = !isSelected;
        UpdateBgColor();
    }

    private void UpdateBgColor()
    {
        if (bgImage != null)
        {
            bgImage.color = isSelected ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBgColor();
    }

    private void OnEdit()
    {
        SideArmsSetSod.SetContext(warTeamData.heroId1, (soldier) =>
        {
            CityBattlePanelManager.SetAllocatedSoldier(warTeamData.heroId1, soldier);
            RefreshUI();
        });

        PanelManager.Instance.ShowSideBar("SideArmsSodSet");
    }

    private void RefreshUI()
    {
        if (heroNameText != null)
        {
            string heroName = "";
            if (warTeamData.heroId1 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId1);
                heroName = heroConfig.Name;
            }

            if (hasActed && !string.IsNullOrEmpty(heroName))
            {
                string grayHex = ColorUtility.ToHtmlStringRGB(SysColor.Theme.ActedHeroTextColor);
                heroNameText.text = $"<color=#{grayHex}>{heroName}</color>";
            }
            else
            {
                heroNameText.text = heroName;
            }
        }

        if (armsText != null)
        {
            if (warTeamData.armsId > 0)
            {
                var armsConfig = ArmsConfig.GetConfig(warTeamData.armsId);
                string armsColorHex = ColorUtility.ToHtmlStringRGB(SysColor.GetArmsLevelColor(armsConfig.Level));
                string cityName = "";
                if (warTeamData.cityId > 0)
                {
                    var cityConfig = WorldConfig.GetConfig(warTeamData.cityId);
                    cityName = cityConfig.Cname;
                }
                armsText.text = $"<color=green>{cityName}</color>--<color=#{armsColorHex}>{armsConfig.NameS}</color>";
            }
            else
            {
                armsText.text = "";
            }
        }

        if (atkText != null || defText != null)
        {
            if (warTeamData.heroId1 > 0 && warTeamData.armsId > 0)
            {
                var (atk, def) = SysFormula.Battle.CalculateCombatAttrForTroop(warTeamData);
                if (atkText != null) atkText.text = atk.ToString();
                if (defText != null) defText.text = def.ToString();
            }
            else
            {
                if (atkText != null) atkText.text = "0";
                if (defText != null) defText.text = "0";
            }
        }

        if (sodierCountText != null)
        {
            sodierCountText.text = CityBattlePanelManager.GetAllocatedSoldier(warTeamData.heroId1).ToString();
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
        string iconPath = ResPath.Texture.HeroIcon(iconName);
        Sprite sprite = ResourceCache.LoadSpriteUI(iconPath);
        if (sprite != null)
        {
            iconImage.sprite = sprite;
        }
    }

    public SaveTroopsData GetWarTeamData()
    {
        return warTeamData;
    }
}
