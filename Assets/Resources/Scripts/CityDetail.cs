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
    public TMP_Text textArchGold;
    public TMP_Text textArchFood;
    public TMP_Text textArchPeople;
    public TMP_Text textGold;
    public TMP_Text textFood;
    public TMP_Text textSoldier;
    public TMP_Text textSecure;
    public TMP_Text textPower;
    public TMP_Text textWall;
    public TMP_Text textLeader;
    public GameObject heroHeadRegion;

    public void SetCityDetail(int cityId)
    {
        this.cityId = cityId;
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
        textPower.text = city.power.ToString();
        textWall.text = city.wall.ToString();
        var owner = city.GetOwner();
        if(owner > 0)
            textLeader.text = HeroConfig.GetConfig(owner).Name;
        else
            textLeader.text = "无";
        
        var heroList = city.GetHeroList();
        //todo 清理一下heroHeadRegion.transform下所有对象
        foreach (Transform child in heroHeadRegion.transform)
            Destroy(child.gameObject);
        for (int i = 0; i < heroList.Count; i++)
        {
            var hero = heroList[i]; // 恢复这一行，定义hero变量
            var heroCfg = HeroConfig.GetConfig(hero);
            if (heroCfg != null)
            {
                var heroHead = Instantiate(Resources.Load<GameObject>("Prefabs/CityHeroHead"), heroHeadRegion.transform);
                heroHead.name = "HeroHead_" + i;

                var rt = heroHead.GetComponent<RectTransform>();
                // 设置头像位置偏移（水平排列）
                rt.anchoredPosition = new Vector2(70 * (i % 4), -70 * (i / 4)); // 70为每个头像的水平间距
                
                // 新建Image组件并设置头像
                var img = heroHead.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
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
