using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;
using Controls.Utils;

public class CityDevPanelManager : MonoBehaviour
{
    private int cityId;
    private int devId;

    public Button closeButton;
    public TMP_Text buildingText;

    public GameObject devDetailParent;

    private GameObject detailObj;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityDev();
        });
    }

    void Update()
    {
        
    }

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;

        if (detailObj != null)
        {
            Destroy(detailObj);
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        buildingText.text = devCfg.Cname;

        if (!string.IsNullOrEmpty(devCfg.Prefab))
        {
            detailObj = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel(devCfg.Prefab)), devDetailParent.transform);
            detailObj.SetActive(true);
            detailObj.GetComponent<ICityDevNode>().SetDev(cityId, devId);
        }
    }
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
