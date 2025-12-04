using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityPanelManager : MonoBehaviour
{
    public int cityId;
    public Button closeBtn;
    public TMP_Text cityName;
    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCity();
            //  CardShopManager.Instance.OnShow();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCityId(int cityId)
    {
        this.cityId = cityId;
        cityName.text = WorldConfig.GetConfig(cityId).Cname;
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
