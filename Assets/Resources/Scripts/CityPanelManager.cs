using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using Controls.Utils;
using System.Linq;
using UnityEngine.EventSystems;

public class CityPanelManager : MonoBehaviour, IPanelEvent
{
    public int cityId;
    public Button closeBtn;
    public TMP_Text cityName;
    public Image cityImage;

    public ScrollRect scrollRectCity;
    public GameObject rankRegionCity;
    public GameObject rankCellCityPrefab;

    public ScrollRect scrollRectHero;
    public GameObject rankRegionHero;
    public GameObject rankCellHeroPrefab;

    public RectTransform devList;

    private float devItemWidth = 300f;
    private float devItemHeight = 200f;
    private float devItemSpacing = 10f;

    private CityCellCity lastSelectedCity;
    private CityCellHero lastSelectedHero;

    private Dictionary<int, CityDevNodeNew> heroToDevNodeMap = new Dictionary<int, CityDevNodeNew>();
    private List<CityDevNodeNew> allDevNodes = new List<CityDevNodeNew>();

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCity();
        });

        LoadCityCells();
        LoadHeroCells();
        CreateDevItems();
    }

    private void LoadCityCells()
    {
        if (rankRegionCity.transform.childCount == 0)
        {
            var player = GameManager.Instance.players.Find(p => p.IsPlayer);
            if (player == null) 
                return;
            var cities = GameManager.Instance.GetCitiesByForce(player.forceId);
            int count = 0;
            foreach (var cityData in cities)
            {
                GameObject cell = Instantiate(rankCellCityPrefab, rankRegionCity.transform);
                cell.transform.localScale = Vector3.one;
                CityCellCity cellCity = cell.GetComponent<CityCellCity>();
                cellCity.cityPanelManager = this;
                var cityCfg = WorldConfig.GetConfig(cityData.cityId);
                cellCity.Init(cityCfg.Cname);
                count++;
            }

            RectTransform rankRect = rankRegionCity.GetComponent<RectTransform>();
            RectTransform cellRect = rankCellCityPrefab.GetComponent<RectTransform>();

            if (rankRect != null && cellRect != null)
            {
                int columns = 2;
                int rows = Mathf.CeilToInt((float)count / columns);
                rankRect.sizeDelta = new Vector2(rankRect.sizeDelta.x, cellRect.sizeDelta.y * rows);
            }

            if (scrollRectCity != null)
            {
                scrollRectCity.normalizedPosition = new Vector2(0, 1);
            }

            if (rankRegionCity.transform.childCount > 0)
            {
                CityCellCity firstCity = rankRegionCity.transform.GetChild(0).GetComponent<CityCellCity>();
                if (firstCity != null)
                {
                    OnSelectCity(firstCity, true);
                }
            }
        }
    }

    private void LoadHeroCells()
    {
        foreach (Transform child in rankRegionHero.transform)
            Destroy(child.gameObject);

        if (lastSelectedCity == null) return;

        var heroes = GameManager.Instance.SaveData.heros.Where(h => h.cityId == lastSelectedCity.cityId).ToList();
        int count = 0;
        foreach (var heroData in heroes)
        {
            if (heroData.state != HeroState.Normal) continue;

            GameObject cell = Instantiate(rankCellHeroPrefab, rankRegionHero.transform);
            cell.transform.localScale = Vector3.one;
            CityCellHero cellHero = cell.GetComponent<CityCellHero>();
            cellHero.cityPanelManager = this;
            var heroCfg = HeroConfig.GetConfig(heroData.heroId);
            cellHero.Init(heroCfg.Id);
            count++;
        }

        RectTransform rankRect = rankRegionHero.GetComponent<RectTransform>();
        RectTransform cellRect = rankCellHeroPrefab.GetComponent<RectTransform>();

        if (rankRect != null && cellRect != null)
        {
            rankRect.sizeDelta = new Vector2(rankRect.sizeDelta.x, cellRect.sizeDelta.y * count);
        }
        GameLog.Info($"LoadHeroCells, count = {cellRect.sizeDelta.y}");

        if (scrollRectHero != null)
        {
            scrollRectHero.normalizedPosition = new Vector2(0, 1);
        }

        if (rankRegionHero.transform.childCount > 0)
        {
            CityCellHero firstHero = rankRegionHero.transform.GetChild(0).GetComponent<CityCellHero>();
            if (firstHero != null)
            {
                OnSelectHero(firstHero, true);
            }
        }
    }

    public void OnSelectCity(CityCellCity cellCity, bool init = false)
    {
        if (lastSelectedCity != null && lastSelectedCity != cellCity)
        {
            lastSelectedCity.SetSelected(false);
        }

        cellCity.SetSelected(true);
        lastSelectedCity = cellCity;

        ClearAllDevNodeHeroes();

        if (!init)
            LoadHeroCells();
    }

    private void ClearAllDevNodeHeroes()
    {
        foreach (var node in allDevNodes)
        {
            node.ClearHero();
        }
        heroToDevNodeMap.Clear();
    }

    public void OnSelectHero(CityCellHero cellHero, bool init = false)
    {
        if (lastSelectedHero != null && lastSelectedHero != cellHero)
        {
            lastSelectedHero.SetSelected(false);
        }

        cellHero.SetSelected(true);
        lastSelectedHero = cellHero;
    }

    void CreateDevItems()
    {
        heroToDevNodeMap.Clear();
        allDevNodes.Clear();

        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in devList)
        {
            if (child.GetComponent<CityDevNodeNew>() != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }

        var devPrefab = Resources.Load<GameObject>("Prefabs/Panels/CityDevNew");
        if (devPrefab == null) return;

        float listWidth = devList.rect.width;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((listWidth + devItemSpacing) / (devItemWidth + devItemSpacing)));
        int devIndex = 0;

        foreach (var cfg in CityDevConfig.ConfigList)
        {
            int row = devIndex / itemsPerRow;
            int col = devIndex % itemsPerRow;

            float posX = col * (devItemWidth + devItemSpacing);
            float posY = -row * (devItemHeight + devItemSpacing);

            var devNode = Instantiate(devPrefab, devList);
            var rectTransform = devNode.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            var devNodeMgr = devNode.GetComponent<CityDevNodeNew>();
            if (devNodeMgr != null)
            {
                devNodeMgr.SetDev(cityId, cfg.Id);
                devNodeMgr.SetCityPanelManager(this);
                allDevNodes.Add(devNodeMgr);
            }

            devIndex++;
        }
    }

    public void OnHeroDragStart(CityCellHero hero)
    {
    }

    public void OnHeroDragEnd(CityCellHero hero, PointerEventData eventData)
    {
    }

    public void AssignHeroToDevNode(int heroId, CityDevNodeNew targetNode)
    {
        if (heroToDevNodeMap.TryGetValue(heroId, out CityDevNodeNew oldNode))
        {
            if (oldNode == targetNode)
            {
                return;
            }
            oldNode.ClearHero();
        }

        int oldHeroId = targetNode.GetCurrentHeroId();
        if (oldHeroId > 0)
        {
            heroToDevNodeMap.Remove(oldHeroId);
        }

        if (heroToDevNodeMap.ContainsValue(targetNode))
        {
            int keyToRemove = -1;
            foreach (var kvp in heroToDevNodeMap)
            {
                if (kvp.Value == targetNode)
                {
                    keyToRemove = kvp.Key;
                    break;
                }
            }
            if (keyToRemove > 0)
            {
                heroToDevNodeMap.Remove(keyToRemove);
            }
        }

        targetNode.SetHero(heroId);
        heroToDevNodeMap[heroId] = targetNode;

        UpdateAllHeroWorkState();

        GameLog.Info($"Hero {heroId} assigned to dev node {targetNode.GetDevId()}");
    }

    private void UpdateAllHeroWorkState()
    {
        foreach (Transform child in rankRegionHero.transform)
        {
            CityCellHero cellHero = child.GetComponent<CityCellHero>();
            if (cellHero != null)
            {
                cellHero.UpdateWorkState();
            }
        }
    }

    public void RemoveHeroFromDevNode(int heroId)
    {
        if (heroToDevNodeMap.TryGetValue(heroId, out CityDevNodeNew node))
        {
            node.ClearHero();
            heroToDevNodeMap.Remove(heroId);
        }
    }

    public CityDevNodeNew GetDevNodeByHero(int heroId)
    {
        if (heroToDevNodeMap.TryGetValue(heroId, out CityDevNodeNew node))
        {
            return node;
        }
        return null;
    }

    void Update()
    {

    }

    public void SetCityId(int cityId)
    {
        this.cityId = cityId;
        var cityCfg = WorldConfig.GetConfig(cityId);
        cityName.text = cityCfg.Cname;
        cityImage.sprite = Resources.Load<Sprite>("Textures/CityView/" + cityCfg.ViewPrefab);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
    } 
}
