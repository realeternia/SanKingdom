using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevNodeNew : MonoBehaviour
{
    private int cityId;
    private int devId;

    public TMP_Text nameText;
    public Image cityImg;
    public Image heroImg;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        var devCfg = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        nameText.text = devCfg.Cname;
        cityImg.sprite = Resources.Load<Sprite>("Textures/Buildings/" + devCfg.Icon);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
