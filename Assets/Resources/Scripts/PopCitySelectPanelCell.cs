using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class PopCitySelectPanelCell : MonoBehaviour
{
    public TMP_Text cityName;
    public TMP_Text textOwner;
    public TMP_Text textSoldier;
    public TMP_Text textHeroCount;

    // Start is called before the first frame update
    void Start()
    {
        cityName.raycastTarget = false;
        textOwner.raycastTarget = false;
        textSoldier.raycastTarget = false;
        textHeroCount.raycastTarget = false;
    }

    public void Init(int cityId)
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        cityName.text = cityCfg.Cname;
        textOwner.text = ForceConfig.GetConfig(cityData.forceId).Cname;
        textSoldier.text = cityData.soldier.ToString();
        textHeroCount.text = cityData.GetHeroCount().ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
