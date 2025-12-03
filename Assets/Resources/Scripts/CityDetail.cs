using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityDetail : MonoBehaviour
{
    public TMP_Text textCityName;
    public TMP_Text textOwnerName;
    public TMP_Text textArchGold;
    public TMP_Text textArchFood;
    public TMP_Text textArchPeople;
    public TMP_Text textGold;
    public TMP_Text textFood;
    public TMP_Text textSoldier;
    public TMP_Text textSecure;
    public TMP_Text textWall;
    public TMP_Text textLeader;

    public void SetCityDetail(int cityId)
    {
        var city = GameManager.Instance.GetCity(cityId);
        var worldCfg = WorldConfig.GetConfig(cityId);
        textCityName.text = worldCfg.Cname;
        textOwnerName.text = ForceConfig.GetConfig(city.forceId).Cname;
        textArchGold.text = city.archGold.ToString();
        textArchFood.text = city.archFood.ToString();
        textArchPeople.text = city.archPeople.ToString();
        textGold.text = city.gold.ToString();
        textFood.text = city.food.ToString();
        textSoldier.text = city.soldier.ToString();
        textSecure.text = city.secure.ToString();
        textWall.text = city.wall.ToString();
        if(city.leader > 0)
            textLeader.text = HeroConfig.GetConfig(city.leader).Name;
        else
            textLeader.text = "无";
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
