using System;
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
    public TMP_Text cityAttrText;
    public Image cityImage;

    public TMP_Text textExp;
    public TMP_Text textGold;
    public TMP_Text textFood;
    public TMP_Text textSoldier;
    public TMP_Text textWall;

    public ScrollRect scrollRectCity;
    public GameObject rankRegionCity;
    public GameObject rankCellCityPrefab;

    public ScrollRect scrollRectHero;
    public GameObject rankRegionHero;
    public GameObject rankCellHeroPrefab;

    public RectTransform topNode;

    public RectTransform devList;

    private float devItemWidth = 300f;
    private float devItemHeight = 200f;
    private float devItemSpacing = 10f;

    private CityCellCity lastSelectedCity;
    private CityCellHero lastSelectedHero;
    private CityDevItem lastSelectedDevNode;

    private Dictionary<int, CityDevItem> heroToDevNodeMap = new Dictionary<int, CityDevItem>();
    private List<CityDevItem> allDevNodes = new List<CityDevItem>();
    private Dictionary<string, ResItem> resItemDict = new Dictionary<string, ResItem>();
    
    private bool isViewOnly = false;
    private int viewForceId = 0;

    private bool IsKingCity()
    {
        return cityId == SystemConst.City.KING_CITY_ID;
    }

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCity();
        });

        LoadCityCells();
    }

    private void LoadCityCells()
    {
        if (rankRegionCity.transform.childCount == 0)
        {
            var cityData = GameManager.Instance.GetCity(cityId);
            if (cityData == null) return;
            
            int targetForceId = cityData.forceId;
            var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
            
            if (SysSwitch.CanViewOtherForceCity && targetForceId != playerForce?.forceId)
            {
                isViewOnly = true;
                viewForceId = targetForceId;
            }
            else
            {
                isViewOnly = false;
                viewForceId = playerForce?.forceId ?? 0;
            }
            
            var cities = GameManager.Instance.GetCitiesByForce(viewForceId);
            int count = 0;
            CityCellCity currentCityCell = null;

            if (!isViewOnly)
            {
                GameObject kingCell = Instantiate(rankCellCityPrefab, rankRegionCity.transform);
                kingCell.transform.localScale = Vector3.one;
                CityCellCity kingCellCity = kingCell.GetComponent<CityCellCity>();
                kingCellCity.cityPanelManager = this;
                kingCellCity.Init(SystemConst.City.KING_CITY_ID, "王城");
                count++;
            }

            foreach (var city in cities)
            {
                GameObject cell = Instantiate(rankCellCityPrefab, rankRegionCity.transform);
                cell.transform.localScale = Vector3.one;
                CityCellCity cellCity = cell.GetComponent<CityCellCity>();
                cellCity.cityPanelManager = this;
                var cityCfg = WorldConfig.GetConfig(city.cityId);
                cellCity.Init(cityCfg.Cname);
                
                if (city.cityId == cityId)
                {
                    currentCityCell = cellCity;
                }
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

            if (currentCityCell != null)
            {
                OnSelectCity(currentCityCell, true);
            }
            else if (rankRegionCity.transform.childCount > 0)
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

        if (IsKingCity())
        {
            var heroRankRect = rankRegionHero.GetComponent<RectTransform>();
            if (heroRankRect != null)
            {
                heroRankRect.sizeDelta = new Vector2(heroRankRect.sizeDelta.x, 0);
            }
            return;
        }

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
        cityId = cellCity.cityId;

        UpdateCityInfo();
        CreateDevItems();
        LoadHeroCells();
    }

    private void UpdateCityInfo()
    {
        if (IsKingCity())
        {
            cityName.text = "王城";
            if (cityImage != null) cityImage.sprite = null;
            UpdateCityAttrText();
            return;
        }

        var cityCfg = WorldConfig.GetConfig(cityId);
        var cityData = GameManager.Instance.GetCity(cityId);

        if (cityCfg != null)
        {
            int level = cityData != null ? cityData.level : 1;
            cityName.text = $"{cityCfg.Cname}({level}级)";
            cityImage.sprite = Resources.Load<Sprite>("Textures/CityView/" + cityCfg.ViewPrefab);
        }

        UpdateCityAttrText();
    }

    private void UpdateCityAttrText()
    {
        if (IsKingCity())
        {
            if (textExp != null) textExp.text = "";
            if (textGold != null) textGold.text = "";
            if (textFood != null) textFood.text = "";
            if (textSoldier != null) textSoldier.text = "";
            if (textWall != null) textWall.text = "";
            return;
        }

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null) return;

        if (textExp != null) textExp.text = $"{cityData.exp}";
        if (textGold != null) textGold.text = $"{(int)GameManager.Instance.GetForce(cityData.forceId).gold}";
        if (textFood != null) textFood.text = $"{(int)cityData.food}";
        if (textSoldier != null) textSoldier.text = $"{(int)cityData.soldier}";
        if (textWall != null) textWall.text = $"{(int)cityData.wall}";
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

    public void OnSelectDevNode(CityDevItem devNode)
    {
        if (lastSelectedDevNode != null && lastSelectedDevNode != devNode)
        {
            lastSelectedDevNode.SetSelected(false);
        }

        devNode.SetSelected(true);
        lastSelectedDevNode = devNode;

        UpdateAllHeroThumbIcon();
    }

    private void UpdateAllHeroThumbIcon()
    {
        string[] attrs = null;
        if (lastSelectedDevNode != null)
        {
            var devCfg = CityDevConfig.GetConfig(lastSelectedDevNode.GetDevId());
            if (devCfg != null)
            {
                attrs = devCfg.Attrs;
            }
        }

        List<CityCellHero> heroList = new List<CityCellHero>();
        foreach (Transform child in rankRegionHero.transform)
        {
            CityCellHero cellHero = child.GetComponent<CityCellHero>();
            if (cellHero != null)
            {
                cellHero.UpdateThumbIcon(attrs);
                heroList.Add(cellHero);
            }
        }

        if (attrs != null && attrs.Length > 0)
        {
            heroList.Sort((a, b) => b.GetWeightedAttrValue(attrs).CompareTo(a.GetWeightedAttrValue(attrs)));
            
            for (int i = 0; i < heroList.Count; i++)
            {
                heroList[i].transform.SetSiblingIndex(i);
            }
        }

        foreach (var devNode in allDevNodes)
        {
            devNode.UpdateHeroImgBG();
        }
    }

    void CreateDevItems()
    {
        heroToDevNodeMap.Clear();
        allDevNodes.Clear();

        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in devList)
        {
            if (child.GetComponent<CityDevItem>() != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }

        var devPrefab = Resources.Load<GameObject>("Prefabs/Panels/Gismo/CityDevItem");
        if (devPrefab == null) return;

        bool isKing = IsKingCity();

        float listWidth = devList.rect.width;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((listWidth + devItemSpacing) / (devItemWidth + devItemSpacing)));
        int devIndex = 0;

        foreach (var cfg in CityDevConfig.ConfigList)
        {
            if (isKing && !cfg.KingAction) continue;
            if (!isKing && cfg.KingAction) continue;
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

            var devNodeMgr = devNode.GetComponent<CityDevItem>();
            if (devNodeMgr != null)
            {
                devNodeMgr.SetDev(cityId, cfg.Id);
                devNodeMgr.SetCityPanelManager(this);
                allDevNodes.Add(devNodeMgr);
            }

            devIndex++;
        }

        LoadDevAssignmentsFromSave();
    }

    private void LoadDevAssignmentsFromSave()
    {
        if (IsKingCity()) return;

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null) return;

        var assignments = cityData.GetDevAssignments();
        GameLog.Debug($"LoadDevAssignmentsFromSave cityId={cityId} assignments={assignments.Count} allDevNodes={allDevNodes.Count}");
        
        foreach (var assignment in assignments)
        {
            var devNode = allDevNodes.FirstOrDefault(n => n.GetDevId() == assignment.devId);
            GameLog.Debug($"LoadDevAssignmentsFromSave heroId={assignment.heroId} devId={assignment.devId} devNode={devNode != null}");
            
            if (devNode != null)
            {
                var hero = GameManager.Instance.GetHero(assignment.heroId);
                GameLog.Debug($"LoadDevAssignmentsFromSave hero={hero != null} hero.cityId={hero?.cityId} cityId={cityId} hero.state={hero?.state}");
                
                if (hero != null && hero.cityId == cityId && hero.state == HeroState.Normal)
                {
                    devNode.SetHero(assignment.heroId);
                    heroToDevNodeMap[assignment.heroId] = devNode;
                    GameLog.Debug($"LoadDevAssignmentsFromSave set hero {assignment.heroId} to devNode {assignment.devId}");
                }
            }
        }

        UpdateAllHeroWorkState();
        UpdateAllResItemAddons();
    }

    public void OnHeroDragStart(CityCellHero hero)
    {
    }

    public void OnHeroDragEnd(CityCellHero hero, PointerEventData eventData)
    {
    }

    public bool AssignHeroToDevNode(int heroId, CityDevItem targetNode)
    {
        if (IsKingCity())
        {
            SystemTip.Instance.ShowTip("王城不能派遣武将");
            return false;
        }

        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return false;
        }
        
        var currentForce = GameManager.Instance.CurrentForce;
        if (currentForce == null || !currentForce.isPlayer)
        {
            SystemTip.Instance.ShowTip("当前不是你的回合");
            return false;
        }
        
        if (currentForce.phase != TurnPhase.Planning)
        {
            SystemTip.Instance.ShowTip("当前阶段无法派遣英雄");
            return false;
        }
        
        var cityData = GameManager.Instance.GetCity(cityId);
        var devCfg = CityDevConfig.GetConfig(targetNode.GetDevId());
        
        bool isHeroAlreadyAssigned = heroToDevNodeMap.ContainsKey(heroId);
        var nodeHeroIds = targetNode.GetHeroIds();
        bool isTargetNodeFull = nodeHeroIds.Count >= devCfg.HeroCount;

        if (!isHeroAlreadyAssigned && !isTargetNodeFull)
        {
            var levelCfg = CityLevelConfig.GetConfig(cityData.level);
            if (heroToDevNodeMap.Count >= levelCfg.JobCount)
            {
                SystemTip.Instance.ShowTip($"该城市最多只能派遣{levelCfg.JobCount}人工作");
                return false;
            }
        }

        if (heroToDevNodeMap.TryGetValue(heroId, out CityDevItem oldNode))
        {
            if (oldNode == targetNode)
            {
                return true;
            }
            oldNode.RemoveHero(heroId);
        }

        if (isTargetNodeFull && !nodeHeroIds.Contains(heroId))
        {
            int firstHeroId = nodeHeroIds[0];
            targetNode.RemoveHero(firstHeroId);
            heroToDevNodeMap.Remove(firstHeroId);
            cityData.RemoveDevAssignment(firstHeroId);
        }

        targetNode.SetHero(heroId);
        heroToDevNodeMap[heroId] = targetNode;
        cityData.SetDevAssignment(heroId, targetNode.GetDevId());

        UpdateAllHeroWorkState();
        UpdateAllResItemAddons();

        GameLog.Info($"Hero {heroId} assigned to dev node {targetNode.GetDevId()}");
        return true;
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
        if (heroToDevNodeMap.TryGetValue(heroId, out CityDevItem node))
        {
            node.ClearHero();
            heroToDevNodeMap.Remove(heroId);

            var cityData = GameManager.Instance.GetCity(cityId);
            if (cityData != null)
            {
                cityData.RemoveDevAssignment(heroId);
            }
            
            UpdateAllResItemAddons();
        }
    }


    void Update()
    {

    }

    public void SetCityId(int cityId)
    {
        this.cityId = cityId;
        UpdateCityInfo();
        InitTopNodeResItems();
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }

    public void SendSignal(SignalData data)
    {
        if (data.Name == "CityResChange")
        {
            var signal = data as CityResChangeSignal;
            if (signal.CityId == cityId)
            {
                RefreshTopNodeResItem(signal.ResType, signal.Value);
            }
        }
    }

    public void InitTopNodeResItems()
    {
        GameLog.Debug($"InitTopNodeResItems topNode={topNode}");
        
        foreach (Transform child in topNode.transform)
        {
            Destroy(child.gameObject);
        }
        resItemDict.Clear();

        if (IsKingCity()) return;
        
        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null) return;
        
        var viewForce = GameManager.Instance.GetForce(viewForceId);
        
        int index = 0;
        foreach (var attrConfig in CityAttrConfig.ConfigList)
        {
            if (attrConfig.IsForceAttr)
                continue;
            
            GameLog.Debug($"InitTopNodeResItems creating ResItem for {attrConfig.name}");
            var resBasePrefab = Resources.Load<GameObject>("Prefabs/ResBase");
            var resObj = Instantiate(resBasePrefab, topNode.transform);
            resObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(240 * index, 0);
            var resItem = resObj.GetComponent<ResItem>();
            resItem.Init(attrConfig.name);
            resItem.UpdateNum(cityData.GetAttr(attrConfig.name));
            resItemDict[attrConfig.name] = resItem;
            index++;
        }
        
        GameLog.Debug($"InitTopNodeResItems resItemDict.Count={resItemDict.Count}");
        UpdateAllResItemAddons();
    }

    private void UpdateAllResItemAddons()
    {
        if (IsKingCity()) return;

        var cityData = GameManager.Instance.GetCity(cityId);
        var viewForce = GameManager.Instance.GetForce(viewForceId);
        
        var cityAttrAddons = cityData.CalculateDevAttrAddons();
        var forceAttrAddons = viewForce != null ? viewForce.CalculateForceAttrAddons() : new Dictionary<string, float>();
        
        GameLog.Debug($"UpdateAllResItemAddons cityId={cityId} resItemDict={resItemDict.Count} cityAttrAddons={cityAttrAddons.Count}");
        
        foreach (var kvp in resItemDict)
        {
            string attrName = kvp.Key;
            var resItem = kvp.Value;
            var attrConfig = CityAttrConfig.GetConfigByname(attrName);
            if (attrConfig.IsPosRes)
                continue;
            
            float addonFloat = 0;
            if (attrConfig.IsForceAttr)
            {
                forceAttrAddons.TryGetValue(attrName.ToLower(), out addonFloat);
            }
            else
            {
                cityAttrAddons.TryGetValue(attrName.ToLower(), out addonFloat);
            }
            int addon = (int)Math.Floor(addonFloat);
            GameLog.Debug($"UpdateAllResItemAddons attrName={attrName} addon={addon}");
            resItem.UpdateAddon(addon);
        }
        
        if (!isViewOnly)
        {
            PanelManager.Instance.UpdateForceResItemAddons();
        }
    }

    private void RefreshTopNodeResItem(string attrName, int value)
    {
        if (topNode == null) return;
        foreach (Transform child in topNode.transform)
        {
            var resItem = child.GetComponent<ResItem>();
            if (resItem != null && resItem.attrName == attrName)
            {
                resItem.UpdateNum(value);
                return;
            }
        }
    } 
}
