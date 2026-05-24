using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityTroopsItem : MonoBehaviour
{
    public TMP_Text heroNameText;
    public TMP_Text armsText;
    public TMP_Text cityText;

    public TMP_Text atkText;
    public TMP_Text defText;
    public Image hero1IconImage;
    public Image hero2IconImage;
    public Image hero3IconImage;
    public Image bgImage;

    public Button editButton;
    public Button dismissButton;

    public Button newTroopsButton;
    public GameObject coverNode;

    private SaveTroopsData warTeamData;
    private bool isCreateMode = false;
    private bool isViewOnly = false;
    private CityPanelManager cityPanelManager;
    private TroopsHeroSlot slot1;
    private TroopsHeroSlot slot2;
    private TroopsHeroSlot slot3;

    public void SetCityPanelManager(CityPanelManager manager)
    {
        cityPanelManager = manager;
    }

    public void SetViewOnly(bool viewOnly)
    {
        isViewOnly = viewOnly;
    }

    private void UpdateBgColor()
    {
        if (bgImage == null) return;
        if (warTeamData != null && warTeamData.cityId > 0 && cityPanelManager != null && warTeamData.cityId == cityPanelManager.cityId)
        {
            bgImage.color = SysColor.Theme.CellSelected;
        }
        else
        {
            bgImage.color = SysColor.Theme.CellNormal;
        }
    }

    public void Init(SaveTroopsData data)
    {
        warTeamData = data;
        SetupSlots();
        RefreshUI();

        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(OnDismiss);
        }

        if (editButton != null)
        {
            editButton.onClick.AddListener(OnEdit);
        }

        if (newTroopsButton != null)
        {
            newTroopsButton.onClick.AddListener(OnNewTroops);
        }
    }

    private void SetupSlots()
    {
        slot1 = AddSlotToImage(hero1IconImage, 0);
        slot2 = AddSlotToImage(hero2IconImage, 1);
        slot3 = AddSlotToImage(hero3IconImage, 2);
    }

    private TroopsHeroSlot AddSlotToImage(Image iconImage, int index)
    {
        if (iconImage == null) return null;

        iconImage.raycastTarget = true;

        var existingSlot = iconImage.GetComponent<TroopsHeroSlot>();
        if (existingSlot != null)
        {
            existingSlot.slotIndex = index;
            existingSlot.troopsItem = this;
            return existingSlot;
        }

        var slot = iconImage.gameObject.AddComponent<TroopsHeroSlot>();
        slot.slotIndex = index;
        slot.troopsItem = this;
        return slot;
    }

    public void OnHeroDropped(int heroId, int slotIndex)
    {
        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return;
        }

        if (isCreateMode)
        {
            warTeamData = new SaveTroopsData();
            SetHeroIdBySlot(warTeamData, slotIndex, heroId);

            SaveTroopsData.AddTroopToCity(warTeamData, cityPanelManager.cityId);

            EnsureCommanderHasHighestLeadship(warTeamData);
            UpdateTroopCityId(warTeamData);
            cityPanelManager.CreateTroopsItems();
            cityPanelManager.UpdateAllHeroWorkState();
            return;
        }

        if (warTeamData == null) return;

        if (slotIndex > 0 && warTeamData.heroId1 <= 0)
        {
            SystemTip.Instance.ShowTip("请先设置主将");
            return;
        }

        if (slotIndex == 0 && (warTeamData.heroId2 > 0 || warTeamData.heroId3 > 0))
        {
            var newHero = GameManager.Instance.GetHero(heroId);
            if (newHero != null && newHero.cityId != warTeamData.cityId)
            {
                SystemTip.Instance.ShowTip("有副将时不能换其他城市的主将");
                return;
            }
        }

        if (slotIndex > 0)
        {
            var viceHero = GameManager.Instance.GetHero(heroId);
            if (viceHero != null && viceHero.cityId != warTeamData.cityId)
            {
                SystemTip.Instance.ShowTip("副将必须来自同一城市");
                return;
            }
        }

        int existingSlot = FindHeroSlot(warTeamData, heroId);
        if (existingSlot >= 0)
        {
            SetHeroIdBySlot(warTeamData, existingSlot, 0);
        }

        SetHeroIdBySlot(warTeamData, slotIndex, heroId);

        EnsureCommanderHasHighestLeadship(warTeamData);
        UpdateTroopCityId(warTeamData);
        RefreshUI();
        cityPanelManager.UpdateAllHeroWorkState();
    }

    private void UpdateTroopCityId(SaveTroopsData data)
    {
        if (data.heroId1 <= 0) return;
        var hero = GameManager.Instance.GetHero(data.heroId1);
        if (hero != null)
        {
            data.cityId = hero.cityId;
        }
    }

    private void EnsureCommanderHasHighestLeadship(SaveTroopsData data)
    {
        if (data.heroId1 <= 0) return;

        var commander = GameManager.Instance.GetHero(data.heroId1);
        if (commander == null) return;
        int maxLeadship = commander.GetAttr("leadship");
        int bestSlot = 0;

        if (data.heroId2 > 0)
        {
            var hero2 = GameManager.Instance.GetHero(data.heroId2);
            if (hero2 != null && hero2.GetAttr("leadship") > maxLeadship)
            {
                maxLeadship = hero2.GetAttr("leadship");
                bestSlot = 1;
            }
        }

        if (data.heroId3 > 0)
        {
            var hero3 = GameManager.Instance.GetHero(data.heroId3);
            if (hero3 != null && hero3.GetAttr("leadship") > maxLeadship)
            {
                bestSlot = 2;
            }
        }

        if (bestSlot > 0)
        {
            int swapHeroId = bestSlot == 1 ? data.heroId2 : data.heroId3;
            SetHeroIdBySlot(data, bestSlot, data.heroId1);
            data.heroId1 = swapHeroId;
        }
    }

    private void RemoveHeroFromDev(int heroId)
    {
        var cityData = GameManager.Instance.GetCity(cityPanelManager.cityId);
        if (cityData == null) return;

        var devId = cityData.GetDevIdByHeroId(heroId);
        if (devId.HasValue)
        {
            cityData.RemoveDevAssignment(heroId);
        }
    }

    private int FindHeroSlot(SaveTroopsData data, int heroId)
    {
        if (data.heroId1 == heroId) return 0;
        if (data.heroId2 == heroId) return 1;
        if (data.heroId3 == heroId) return 2;
        return -1;
    }

    private void SetHeroIdBySlot(SaveTroopsData data, int slotIndex, int heroId)
    {
        switch (slotIndex)
        {
            case 0: data.heroId1 = heroId; break;
            case 1: data.heroId2 = heroId; break;
            case 2: data.heroId3 = heroId; break;
        }
    }

    private void OnDismiss()
    {
        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return;
        }

        if (warTeamData == null || cityPanelManager == null) return;

        SaveTroopsData.RemoveTroopFromCity(warTeamData);

        warTeamData.ReleaseResources();

        cityPanelManager.CreateTroopsItems();
        cityPanelManager.UpdateAllHeroWorkState();
    }

    private void OnEdit()
    {
        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return;
        }

        SideArmysSelector.SetContextForTroop(warTeamData.armsId, (newArmsId) =>
        {
            warTeamData.SetArmsId(newArmsId);
            RefreshUI();
        });
        PanelManager.Instance.ShowSideBar("SideArmsSelector");
    }

    private void OnNewTroops()
    {
        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return;
        }

        if (cityPanelManager == null) return;

        var newTroop = new SaveTroopsData();
        SaveTroopsData.AddTroopToCity(newTroop, cityPanelManager.cityId);

        cityPanelManager.CreateTroopsItems();
    }

    public void SetCreateMode(bool isCreate)
    {
        isCreateMode = isCreate;
        if (newTroopsButton != null)
        {
            newTroopsButton.gameObject.SetActive(isCreate && !isViewOnly);
        }
        if (coverNode != null)
        {
            coverNode.SetActive(isCreate);
        }
        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        bool hasCommander = warTeamData != null && warTeamData.heroId1 > 0;
        
        if (editButton != null)
        {
            editButton.gameObject.SetActive(!isCreateMode && !isViewOnly && hasCommander);
        }
        if (dismissButton != null)
        {
            dismissButton.gameObject.SetActive(!isCreateMode && !isViewOnly && hasCommander);
        }
    }

    private void RefreshUI()
    {
        if (warTeamData == null)
        {
            if (hero1IconImage != null)
            {
                hero1IconImage.gameObject.SetActive(true);
                hero1IconImage.sprite = null;
                hero1IconImage.color = Color.white;
            }
            if (hero2IconImage != null)
            {
                hero2IconImage.gameObject.SetActive(true);
                hero2IconImage.sprite = null;
                hero2IconImage.color = Color.white;
            }
            if (hero3IconImage != null)
            {
                hero3IconImage.gameObject.SetActive(true);
                hero3IconImage.sprite = null;
                hero3IconImage.color = Color.white;
            }
            UpdateButtonsState();
            return;
        }

        if (heroNameText != null)
        {
            string heroName = "";
            if (warTeamData.heroId1 > 0)
            {
                var heroConfig = HeroConfig.GetConfig(warTeamData.heroId1);
                heroName = heroConfig.Name + "队";
            }
            heroNameText.text = heroName;
        }

        if (cityText != null)
        {
            if (warTeamData.cityId > 0)
            {
                var cityCfg = WorldConfig.GetConfig(warTeamData.cityId);
                cityText.text = cityCfg.Cname;
            }
            else
            {
                cityText.text = "";
            }
        }

        if (armsText != null)
        {
            if (warTeamData.armsId > 0)
            {
                var armsConfig = ArmsConfig.GetConfig(warTeamData.armsId);
                string armsColorHex = ColorUtility.ToHtmlStringRGB(SysColor.GetArmsLevelColor(armsConfig.Level));
                armsText.text = $"<color=#{armsColorHex}>{armsConfig.NameS}</color>";
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

        RefreshHeroSlot(hero1IconImage, warTeamData.heroId1);
        RefreshHeroSlot(hero2IconImage, warTeamData.heroId2);
        RefreshHeroSlot(hero3IconImage, warTeamData.heroId3);
        
        UpdateBgColor();
        UpdateButtonsState();
    }

    private void RefreshHeroSlot(Image iconImage, int heroId)
    {
        if (iconImage == null) return;

        iconImage.gameObject.SetActive(true);
        iconImage.color = Color.white;

        if (heroId > 0)
        {
            var heroConfig = HeroConfig.GetConfig(heroId);
            SetHeroIcon(iconImage, heroConfig.Icon);
        }
        else
        {
            iconImage.sprite = null;
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
