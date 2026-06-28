using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityMovePanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    private int forceId;
    private int sourceCityId;
    private int selectedDestCityId;
    private int moveHeroCount;
    private int movedHeroCount;

    public Button destButton;
    public TMP_Text attrVal1Text;
    public ResCheckItem resCheckItem;

    public Button closeButton;
    public Button okButton;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityMove();
        });
        destButton.onClick.AddListener(() =>
        {
            var cityIds = MapTool.GetOwnCityIds(forceId);
            cityIds.Remove(sourceCityId);
            SideCitySelector.SetContext(selectedDestCityId, cityIds, (newCityId) =>
            {
                selectedDestCityId = newCityId;
                if (newCityId == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var cityCfg = WorldConfig.GetConfig(newCityId);
                    attrVal1Text.text = cityCfg.Cname;
                }
                //ClearAllSelections();
            });
            PanelManager.Instance.ShowSideBar("SideCitySelector");
        });

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnMove);
        }
    }

    void Update()
    {

    }

    public void Init(int forceId, int sourceCityId)
    {
        this.forceId = forceId;
        this.sourceCityId = sourceCityId;
        this.selectedDestCityId = 0;

        var moveDevCfg = CityDevConfig.GetConfig(SystemConst.CityDev.MOVE_DEV_ID);
        moveHeroCount = moveDevCfg != null ? moveDevCfg.HeroCount : 0;

        var force = GameManager.Instance.GetForce(forceId);
        movedHeroCount = force != null ? force.GetKingActionCount(SystemConst.CityDev.MOVE_DEV_ID) : 0;

        if (resCheckItem != null && moveDevCfg != null)
        {
            resCheckItem.Init(ResPath.Texture.AttrIcon(moveDevCfg.Icon), $"{movedHeroCount}/{moveHeroCount}");
        }

        attrVal1Text.text = "-";
        CreateHeroHeadItems();
    }

    public bool CanSelectItem()
    {
        return selectedDestCityId > 0;
    }

    private bool CanSelectHero()
    {
        return movedHeroCount + GetSelectedCount() < moveHeroCount;
    }

    private void OnHeroSelectionChanged()
    {
        UpdateResCheckDisplay();
    }

    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
                count++;
        }
        return count;
    }

    private void UpdateResCheckDisplay()
    {
        if (resCheckItem == null) return;

        int totalCount = movedHeroCount + GetSelectedCount();
        if (totalCount >= moveHeroCount)
        {
            resCheckItem.UpdateDisplay("<color=red>已满</color>");
        }
        else
        {
            resCheckItem.UpdateDisplay($"{totalCount}/{moveHeroCount}");
        }
    }

    private void ClearAllSelections()
    {
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                itemScript.SetSelected(false);
            }
        }
    }

    private List<HeroHeadItem> GetSelectedItems()
    {
        List<HeroHeadItem> selected = new List<HeroHeadItem>();
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selected.Add(itemScript);
            }
        }
        return selected;
    }

    private void OnMove()
    {
        if (selectedDestCityId <= 0)
        {
            SystemTip.Instance.ShowTip("请选择目标城市");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择移动武将");
            return;
        }

        List<int> heroIds = new List<int>();
        foreach (var item in selectedItems)
        {
            heroIds.Add(item.GetHeroId());
        }

        var moveDevCfg = CityDevConfig.GetConfig(SystemConst.CityDev.MOVE_DEV_ID);
        int devId = SystemConst.CityDev.MOVE_DEV_ID;
        PanelManager.Instance.ShowPopResultPanel("移动", new List<PopResultPanelManager.AttrData>(), () =>
        {
            var force = GameManager.Instance.GetForce(forceId);

            var heroGroups = heroIds
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null)
                .GroupBy(h => h.cityId);

            foreach (var group in heroGroups)
            {
                int srcCityId = group.Key;
                int[] heroesToMove = group.Select(h => h.heroId).ToArray();
                force.MoveHeroToCity(srcCityId, selectedDestCityId, heroesToMove);
            }

            force.AddKingActionCount(devId, heroIds.Count);

            PanelManager.Instance.HideCityMove();
        }, moveDevCfg != null ? moveDevCfg.Mp4 : "");
    }

    private void CreateHeroHeadItems()
    {
        foreach (var item in heroHeadItems)
        {
            if (item != null)
                Destroy(item);
        }
        heroHeadItems.Clear();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        var kingCity = force.GetKingCity();
        int kingCityId = kingCity != null ? kingCity.cityId : 0;

        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        cities = cities.OrderByDescending(c => c.cityId == kingCityId).ToList();

        List<int> heroList = new List<int>();
        foreach (var city in cities)
        {
            var cityHeroes = city.GetNormalHeroList();
            heroList.AddRange(cityHeroes);
        }

        if (heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        // 已行动武将排到末尾，再按君主城市优先
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h => GameManager.Instance.GetHero(h).cityId == kingCityId ? 1 : 0)
            .ToList();

        var itemPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelGismo("HeroHeadItem"));

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 156f;
        float itemHeight = 185f;
        float spacing = 10f;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((containerRect.rect.width + spacing) / (itemWidth + spacing)));

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        for (int i = 0; i < heroList.Count; i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            float posX = startX + col * (itemWidth + spacing);
            float posY = -row * (itemHeight + spacing);

            GameObject itemObj = Instantiate(itemPrefab, itemRegionMain.transform);
            itemObj.transform.localScale = Vector3.one;

            RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(itemWidth, itemHeight);
            }

            HeroHeadItem itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                string attText = GetHeroAttText(heroList[i]);
                var heroData = GameManager.Instance.GetHero(heroList[i]);
                bool hasActed = heroData != null && heroData.round >= GameManager.Instance.SaveData.round;
                itemScript.Init(heroList[i], attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);
            }

            heroHeadItems.Add(itemObj);
        }

        int totalRows = (heroList.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    private string GetHeroAttText(int heroId)
    {
        var heroData = GameManager.Instance.GetHero(heroId);

        var cityCfg = WorldConfig.GetConfig(heroData.cityId);
        return cityCfg != null ? cityCfg.Cname : "";
    }
}
