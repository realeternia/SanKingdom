using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityDetail : MonoBehaviour, IPanelEvent
{
    public int cityId;

    public TMP_Text textCityName;
    public TMP_Text textOwnerName;
    public TMP_Text textLevel;
    public TMP_Text textExp;
    public TMP_Text textGold;
    public TMP_Text textFood;
    public TMP_Text textSoldier;
    public TMP_Text textPower;
    public TMP_Text textWall;
    public TMP_Text textLeader;
    public GameObject heroHeadRegion;

    private void SetTextAndColor(TMP_Text textComponent, SaveCityData city, string attrName)
    {
        var val = city.GetAttr(attrName);
        textComponent.text = val.ToString();
        var cfg = CityAttrConfig.GetConfigByname(attrName.ToLower());

        textComponent.color = Color.gray;
        if (cfg.ValLow != 0 && val < cfg.ValLow)
        {
            textComponent.color = Color.red;
        }
    }

    private void AddOverlay(GameObject parent, Color color)
    {
        var overlay = new GameObject("Overlay");
        overlay.transform.SetParent(parent.transform, false);
        
        var rt = overlay.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        
        var img = overlay.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }

    private void AddBorder(GameObject parent, Color color)
    {
        var img = parent.GetComponent<Image>();
        if (img != null)
        {
            var outline = parent.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(3, -3);
        }
    }

    public void SetCityDetail(int cityId)
    {
        if (cityId <= 0)
        {
            return;
        }

        this.cityId = cityId;
        var city = GameManager.Instance.GetCity(cityId);
        var worldCfg = WorldConfig.GetConfig(cityId);
        textCityName.text = worldCfg.Cname;
        textOwnerName.text = ForceConfig.GetConfig(city.forceId).Cname;
        SetTextAndColor(textLevel, city, "level");
        SetTextAndColor(textExp, city, "exp");
        SetTextAndColor(textGold, city, "gold");
        SetTextAndColor(textFood, city, "food");
        SetTextAndColor(textSoldier, city, "soldier");
        SetTextAndColor(textPower, city, "power");
        SetTextAndColor(textWall, city, "wall");
        var owner = city.GetOwner();
        if(owner > 0)
            textLeader.text = HeroConfig.GetConfig(owner).Name;
        else
            textLeader.text = "无";
        textLeader.color = Color.gray;
        
        var heroList = city.GetNormalHeroList();
        var wildList = city.GetHeroList(false, true);
        var catchedList = city.GetCatchedHeroList();
        var currentRound = GameManager.Instance.SaveData.round;
        foreach (Transform child in heroHeadRegion.transform)
            Destroy(child.gameObject);
        for (int i = 0; i < heroList.Count; i++)
        {
            var heroId = heroList[i];
            var hero = GameManager.Instance.GetHero(heroId);
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg != null)
            {
                var heroHead = Instantiate(Resources.Load<GameObject>("Prefabs/CityHeroHead"), heroHeadRegion.transform);
                heroHead.name = "HeroHead_" + i;

                var rt = heroHead.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(70 * (i % 4), -70 * (i / 4));
                
                var img = heroHead.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

                bool hasActed = hero.round >= currentRound;
                if (hasActed)
                {
                    AddOverlay(heroHead, new Color(0, 0, 0, 0.92f));
                }
            }
        }
        int baseIdx = heroList.Count;
        for (int i = 0; i < wildList.Count; i++)
        {
            var heroId = wildList[i];
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg != null)
            {
                int idx = baseIdx + i;
                var heroHead = Instantiate(Resources.Load<GameObject>("Prefabs/CityHeroHead"), heroHeadRegion.transform);
                heroHead.name = "WildHeroHead_" + i;

                var rt = heroHead.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(70 * (idx % 4), -70 * (idx / 4));
                
                var img = heroHead.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

                AddOverlay(heroHead, new Color(0, 0, 0, 0.92f));
                AddBorder(heroHead, Color.yellow);
            }
        }
        baseIdx += wildList.Count;
        for (int i = 0; i < catchedList.Count; i++)
        {
            var heroId = catchedList[i];
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg != null)
            {
                int idx = baseIdx + i;
                var heroHead = Instantiate(Resources.Load<GameObject>("Prefabs/CityHeroHead"), heroHeadRegion.transform);
                heroHead.name = "CatchedHeroHead_" + i;

                var rt = heroHead.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(70 * (idx % 4), -70 * (idx / 4));
                
                var img = heroHead.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

                AddOverlay(heroHead, new Color(0, 0, 0, 0.92f));
                AddBorder(heroHead, Color.red);
            }
        }

    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        if(name == "CityAttrChange")
        {
            SetCityDetail(cityId); //刷新数据
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
