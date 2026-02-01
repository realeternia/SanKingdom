using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class SelectHeroControl : MonoBehaviour
{
    private int cityId;
    private int devId;
    public Image[] heroHeads;
    public int[] heroIds;
    public Button confirmButton;
    
    // 英雄选择变化的回调委托
    public System.Action<int> OnHeroCountChange;

    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            int[] heroList = GameManager.Instance.GetCity(cityId).GetHeroList().ToArray();
            var devCfg = CityDevConfig.GetConfig(devId);
            string[] attrs = devCfg.Attrs;
            
            // 计算最大可选择的英雄数量，不超过经济能力
            int cityGold = GameManager.Instance.GetCity(cityId).gold;
            int singleCost = devCfg.GoldCost;
            int maxHeroCount = devCfg.HeroCount;
            if (singleCost > 0)
            {
                maxHeroCount = System.Math.Min(maxHeroCount, cityGold / singleCost);
            }

            if (maxHeroCount == 0)
                return;
            
            PanelManager.Instance.ShowPopHeroSelectPanel(cityId, maxHeroCount, heroList, heroIds, attrs, (selectedHeroIds) =>
            {
                heroIds = selectedHeroIds.ToArray();
                for (int i = 0; i < heroHeads.Length; i++)
                {
                    if (i < selectedHeroIds.Count)
                    {
                        heroHeads[i].gameObject.SetActive(true);
                        var heroCfg = HeroConfig.GetConfig(selectedHeroIds[i]);
                        heroHeads[i].sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
                    }
                    else
                    {
                        heroHeads[i].gameObject.SetActive(false);
                    }
                }
                if (heroIds.Length == 0)
                {
                    heroHeads[0].gameObject.SetActive(true);
                    heroHeads[0].sprite = Resources.Load<Sprite>("Skins/moren");
                }
                
                // 触发英雄数量变化回调
                if (OnHeroCountChange != null)
                {
                    OnHeroCountChange(heroIds.Length);
                }
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetDevId(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;

        for (int i = 0; i < heroHeads.Length; i++)
        {
            heroHeads[i].gameObject.SetActive(false);
        }
        heroHeads[0].gameObject.SetActive(true);
        heroHeads[0].sprite = Resources.Load<Sprite>("Skins/moren");

        heroIds = new int[0];
        
        // 触发英雄数量变化回调
        if (OnHeroCountChange != null)
        {
            OnHeroCountChange(heroIds.Length);
        }
    }
}
