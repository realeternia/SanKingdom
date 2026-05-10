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

    private static Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private static Color selectedColor = new Color(0.3f, 0.7f, 0.4f, 1f);

    public void Init(SaveTroopsData data)
    {
        warTeamData = data;

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
            bgImage.color = isSelected ? selectedColor : normalColor;
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
        PopArmySetManager.SetSoldierSetCallback((soldier) =>
        {
            warTeamData.soldierCount = soldier;
            RefreshUI();
        });

        if (warTeamData.soldierCount > 0)
        {
            CityBattlePanelManager.SetAllocatedSoldier(warTeamData.heroId1, warTeamData.soldierCount);
        }

        PanelManager.Instance.ShowPopArmySetPanel(warTeamData.heroId1);
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

        if (atkText != null || defText != null)
        {
            if (warTeamData.heroId1 > 0 && warTeamData.armsId > 0)
            {
                var heroData = GameManager.Instance.GetHero(warTeamData.heroId1);
                if (heroData != null)
                {
                    var (atk, def) = SysFormula.Battle.CalculateCombatAttr(heroData, warTeamData.armsId);
                    if (atkText != null) atkText.text = atk.ToString();
                    if (defText != null) defText.text = def.ToString();
                }
                else
                {
                    if (atkText != null) atkText.text = "0";
                    if (defText != null) defText.text = "0";
                }
            }
            else
            {
                if (atkText != null) atkText.text = "0";
                if (defText != null) defText.text = "0";
            }
        }

        if (sodierCountText != null)
        {
            sodierCountText.text = warTeamData.soldierCount.ToString();
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
