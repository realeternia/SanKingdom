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

    public RectTransform devList;

    private float devItemWidth = 300f;
    private float devItemHeight = 200f;
    private float devItemSpacing = 10f;

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCity();
        });

        CreateDevItems();
    }

    void CreateDevItems()
    {
        var devPrefab = Resources.Load<GameObject>("Prefabs/Panels/CityDevNew");
        if (devPrefab == null) return;

        float listWidth = devList.rect.width;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((listWidth + devItemSpacing) / (devItemWidth + devItemSpacing)));
        int devIndex = 0;

        foreach (var cfg in CityDevConfig.ConfigList)
        {
            int row = devIndex / itemsPerRow;
            int col = devIndex % itemsPerRow;

            float posX = col * (devItemWidth + devItemSpacing);
            float posY = -row * (devItemHeight + devItemSpacing);

            var devNode = Instantiate(devPrefab, devList);
            var rectTransform = devNode.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            var devNodeMgr = devNode.GetComponent<CityDevNodeNew>();
            if (devNodeMgr != null)
            {
                devNodeMgr.SetDev(cityId, cfg.Id);
            }

            devIndex++;
        }
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
