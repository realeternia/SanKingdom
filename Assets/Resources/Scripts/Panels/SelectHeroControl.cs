using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using Controls.Utils;

public class SelectHeroControl : MonoBehaviour
{
    private int cityId;
    private int devId;
    private int mode;
    
    public Image[] heroHeads;
    public int[] heroIds;
    public Button confirmButton;
    
    public System.Action<int> OnHeroCountChange;

    void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            OnSelectHeroClick();
        });
    }

    void Update()
    {
    }

    private void OnSelectHeroClick()
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        int[] heroList;
        int maxHeroCount;
        string[] attrs;
        bool ignoreActionCheck = false;

        if (mode == 1 || mode == 2)
        {
            GameLog.Info("OnSelectHeroClick, cityId " + cityId);
            heroList = GameManager.Instance.GetPraiseableHeroList(cityData.forceId).ToArray();
            
            if(heroList.Length == 0)
            {
                SystemTip.Instance.ShowTip("没有忠心度低于100的武将");
                return;
            }

            maxHeroCount = 10;
            
            if(mode == 2)
            {
                int cityGold = cityData.GetAttr("gold");
                maxHeroCount = System.Math.Min(maxHeroCount, cityGold / 100);
                if(maxHeroCount == 0)
                {
                    SystemTip.Instance.ShowTip("金币不足,无法选择英雄");
                    return;
                }
            }
            
            attrs = new string[]{"Str", "Inte"};
            ignoreActionCheck = mode == 2;
        }
        else
        {
            heroList = cityData.GetNormalHeroList().ToArray();
            var devCfg = CityDevConfig.GetConfig(devId);
            attrs = devCfg.Attrs;
            
            int cityGold = cityData.GetAttr("gold");
            int singleCost = devCfg.GoldCost;
            maxHeroCount = devCfg.HeroCount;
            if (singleCost > 0)
            {
                maxHeroCount = System.Math.Min(maxHeroCount, cityGold / singleCost);
            }

            if (maxHeroCount == 0)
            {
                SystemTip.Instance.ShowTip("金币不足,无法选择英雄");
                return;
            }
        }

        PanelManager.Instance.ShowPopHeroSelectPanel(cityId, maxHeroCount, heroList, heroIds, attrs, (selectedHeroIds) =>
        {
            heroIds = selectedHeroIds.ToArray();
            for (int i = 0; i < heroHeads.Length; i++)
            {
                if (i < selectedHeroIds.Count)
                {
                    heroHeads[i].gameObject.SetActive(true);
                    var heroCfg = HeroConfig.GetConfig(selectedHeroIds[i]);
                    heroHeads[i].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));
                }
                else
                {
                    heroHeads[i].gameObject.SetActive(false);
                }
            }
            if (heroIds.Length == 0)
            {
                heroHeads[0].gameObject.SetActive(true);
                heroHeads[0].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroDefaultIcon());
            }
            
            if (OnHeroCountChange != null)
            {
                OnHeroCountChange(heroIds.Length);
            }
        }, ignoreActionCheck);
    }

    public void SetDevId(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        this.mode = 0;

        ResetHeroHeads();
    }

    public void SetMode(int mode)
    {
        this.mode = mode;

        ResetHeroHeads();
    }

    private void ResetHeroHeads()
    {
        for (int i = 0; i < heroHeads.Length; i++)
        {
            heroHeads[i].gameObject.SetActive(false);
        }
        heroHeads[0].gameObject.SetActive(true);
        heroHeads[0].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroDefaultIcon());

        heroIds = new int[0];
        
        if (OnHeroCountChange != null)
        {
            OnHeroCountChange(heroIds.Length);
        }
    }
}
