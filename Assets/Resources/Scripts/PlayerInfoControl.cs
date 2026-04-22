using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerInfoControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    
    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public string imgPath;
    public TMP_Text cityText;
    public Image playerBgImg;

    public SaveForceData force;

    void Start()
    {
    }

    public void Init(int forceId)
    {
        force = GameManager.Instance.GetForce(forceId);
        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);
        imgPath = "Textures/Skins/" + heroCfg.Icon;

        playerNameText.text = force.Name;

        playerImage.sprite = Resources.Load<Sprite>(force.IconPath);
        cityText.text = GameManager.Instance.GetPlayerCityCount(forceId).ToString();
        playerBgImg.color = force.LineColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        PanelManager.Instance.SendSignal("SelectPlayer", "", force.forceId);
    }
    
    void Update()
    {

    }
}
