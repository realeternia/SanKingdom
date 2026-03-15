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
            int[] heroList = GameManager.Instance.GetCity(cityId).GetHeroList(true, false).ToArray(); // 不包含在野英雄，因为他们无法参与战斗
            var devCfg = CityDevConfig.GetConfig(devId);
            string[] attrs = devCfg.Attrs;
            PanelManager.Instance.ShowPopHeroBattleSelectPanel(cityId, devCfg.HeroCount, heroList, !devCfg.FindEnemy, heroIds, (selectedHeroIds) =>
            {
                heroIds = selectedHeroIds.ToArray();
                for (int i = 0; i < heroHeads.Length; i++)
                {
                    if (i < selectedHeroIds.Count)
                    {
                        heroHeads[i].gameObject.SetActive(true);
                        var heroCfg = HeroConfig.GetConfig(selectedHeroIds[i]);
                        heroHeads[i].sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);

                        var heroData = GameManager.Instance.GetHero(selectedHeroIds[i]);
                        heroHeads[i].GetComponentInChildren<TMP_Text>().text = $"{heroData.soldier}";

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
        heroHeads[0].sprite = Resources.Load<Sprite>("Skins/moren");

        heroIds = new int[0];
    }
}
