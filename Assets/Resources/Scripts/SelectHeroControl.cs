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
    public Button confirmButton;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < heroHeads.Length; i++)
        {
            heroHeads[i].gameObject.SetActive(false);
        }
        heroHeads[0].gameObject.SetActive(true);
        heroHeads[0].sprite = Resources.Load<Sprite>("Skins/moren");

        confirmButton.onClick.AddListener(() =>
        {
            int[] heroList = GameManager.Instance.GetCity(cityId).GetHeroList().ToArray();
            var devCfg = CityDevConfig.GetConfig(devId);
            string[] attrs = devCfg.Attrs;
            PanelManager.Instance.ShowPopHeroSelectPanel(heroList, attrs, (selectedHeroIds) =>
            {
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
            });
          //  PanelManager.Instance.ShowPopResultPanel(devCfg.Mp4);
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
    }
}
