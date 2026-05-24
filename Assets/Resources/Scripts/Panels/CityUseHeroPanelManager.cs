using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityUseHeroPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    private int forceId;
    private int cityId;
    private int devId;
    private List<int> selectedHeroIds = new List<int>();

    public Button heroButton;
    public TMP_Text attrVal1Text;

    public Button closeButton;
    public Button okButton;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityUseHero();
        });
        heroButton.onClick.AddListener(() =>
        {
            SideHeroSelector.SetContext(cityId, forceId, (newHeroIds) =>
            {
                selectedHeroIds = newHeroIds;
                if (newHeroIds == null || newHeroIds.Count == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var names = newHeroIds.Select(id =>
                    {
                        var cfg = HeroConfig.GetConfig(id);
                        return cfg != null ? cfg.Name : id.ToString();
                    });
                    attrVal1Text.text = string.Join(",", names);
                }
            });
            PanelManager.Instance.ShowSideBar("SideHeroSelector");
        });

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnUseHero);
        }
    }

    void Update()
    {

    }

    public void Init(int forceId, int cityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = cityId;
        this.devId = devId;
        this.selectedHeroIds.Clear();
        attrVal1Text.text = "-";
        CreateHeroHeadItems();
    }

    public bool CanSelectItem()
    {
        return selectedHeroIds.Count > 0;
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

    private void OnUseHero()
    {
        if (selectedHeroIds.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择登用武将");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择执行武将");
            return;
        }

        int[] executorHeroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var devCfg = CityDevConfig.GetConfig(devId);
        var force = GameManager.Instance.GetForce(forceId);

        force.ExecuteCityUseHero(cityId, devId, executorHeroIds, selectedHeroIds.ToArray(), out var allAttrDatas);

        PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, allAttrDatas, () =>
        {
            PanelManager.Instance.HideCityUseHero();
        }, devCfg != null ? devCfg.Mp4 : "", false);
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

        var cities = GameManager.Instance.GetCitiesByForce(forceId);

        List<int> heroList = new List<int>();
        foreach (var city in cities)
        {
            var cityHeroes = city.GetNormalHeroList();
            heroList.AddRange(cityHeroes);
        }

        if (heroList.Count == 0) return;

        int kingHeroId = ForceConfig.GetConfig(forceId).HeroId;
        heroList = heroList.OrderByDescending(h => h == kingHeroId ? 1 : 0)
                           .ThenByDescending(h => GameManager.Instance.GetHero(h).GetAttr("charm"))
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
                itemScript.Init(heroList[i], attText, forceId);
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
