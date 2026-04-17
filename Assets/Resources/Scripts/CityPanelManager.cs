using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using Controls.Utils;

public class CityPanelManager : MonoBehaviour, IPanelEvent
{
    public int cityId;
    public Button closeBtn;
    public TMP_Text cityName;
    public Image cityImage;
    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCity();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCityId(int cityId)
    {
        this.cityId = cityId;
        var cityCfg = WorldConfig.GetConfig(cityId);
        cityName.text = cityCfg.Cname;
        cityImage.sprite = Resources.Load<Sprite>("Textures/CityView/" + cityCfg.ViewPrefab);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
    } 
}
