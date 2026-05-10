using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SelectHeroArmyControl : MonoBehaviour
{
    private int cityId;
    private int devId;
    public Image[] heroHeads;
    public int[] heroIds;
    public Button confirmButton;

    public Action onClick;


    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            int[] heroList = GameManager.Instance.GetCity(cityId).GetNormalHeroList().ToArray();
            var devCfg = CityDevConfig.GetConfig(devId);
            string[] attrs = devCfg.Attrs;
            PanelManager.Instance.ShowPopHeroBattleSelectPanel(cityId, devCfg.HeroCount, heroList, devCfg.Prefab == "CityDevMove", heroIds, (selectedHeroIds) =>
            {
                heroIds = selectedHeroIds.ToArray();
                for (int i = 0; i < heroHeads.Length; i++)
                {
                    if (i < selectedHeroIds.Count)
                    {
                        heroHeads[i].gameObject.SetActive(true);
                        var heroCfg = HeroConfig.GetConfig(selectedHeroIds[i]);
                        heroHeads[i].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));

                        var heroData = GameManager.Instance.GetHero(selectedHeroIds[i]);
                        var cityData = GameManager.Instance.GetCity(heroData.cityId);
                        heroHeads[i].GetComponentInChildren<TMP_Text>().text = $"{cityData.soldier}";

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
                onClick?.Invoke();
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
        heroHeads[0].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroDefaultIcon());

        heroIds = new int[0];
    }
}
