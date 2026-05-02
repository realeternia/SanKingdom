using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;


public class RankCellInfoCity : MonoBehaviour, IRankDetailInfo, IRankDetailInfoHeader
{
    public RankPanelManager rankPanelManager;

    public TMP_Text cityName;

    public TMP_Text cityLevel;
    public TMP_Text cityExp;
    public TMP_Text citySoldier;
    public TMP_Text cityWall;

    public Button btnLevel;
    public Button btnExp;
    public Button btnSoldier;
    public Button btnWall;    

    public GameObject nodeHeader;
    public GameObject nodeRow;

    public int cityId;

    // Start is called before the first frame update
    void Start()
    {
        cityName.raycastTarget = false;
        cityLevel.raycastTarget = false;
        cityExp.raycastTarget = false;
        citySoldier.raycastTarget = false;
        cityWall.raycastTarget = false;
    }

    public void SetManager(RankPanelManager rankPanelManager)
    {
        this.rankPanelManager = rankPanelManager;
    }    

    public void SetMode(bool isHeader)
    {
        if(isHeader)
        {
            nodeHeader.SetActive(true);
            nodeRow.SetActive(false);
            btnLevel.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Level");
            });
            btnExp.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Exp");
            });
            btnSoldier.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Soldier");
            });
            btnWall.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Wall");
            });
        }
        else
        {
            nodeHeader.SetActive(false);
            nodeRow.SetActive(true);
        }
    }

    public int GetValInt(string key)
    {
        switch (key)
        {
            case "Level":
                return int.Parse(cityLevel.text);
            case "Exp":
                return int.Parse(cityExp.text);
            case "Soldier":
                return int.Parse(citySoldier.text);
            case "Wall":
                return int.Parse(cityWall.text);
            default:
                return 0;
        }
    }

    public void Init(int cityId)
    {
        this.cityId = cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        cityName.text = WorldConfig.GetConfig(cityId).Cname;
        cityLevel.text = cityData.level.ToString();
        cityExp.text = cityData.exp.ToString();
        citySoldier.text = cityData.soldier.ToString();
        cityWall.text = cityData.wall.ToString();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnSelectHero(bool isSelected)
    {

    }
}
