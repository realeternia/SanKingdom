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

    public TMP_Text cityArchGold;
    public TMP_Text cityArchFood;
    public TMP_Text cityPeople;
    public TMP_Text citySoldier;
    public TMP_Text citySecure;
    public TMP_Text cityWall;

    public Button btnArchGold;
    public Button btnArchFood;
    public Button btnPeople;
    public Button btnSoldier;
    public Button btnSecure;
    public Button btnWall;    

    public GameObject nodeHeader;
    public GameObject nodeRow;

    public int cityId;

    // Start is called before the first frame update
    void Start()
    {
        cityName.raycastTarget = false;
        cityArchGold.raycastTarget = false;
        cityArchFood.raycastTarget = false;
        cityPeople.raycastTarget = false;
        citySoldier.raycastTarget = false;
        citySecure.raycastTarget = false;
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
            btnArchGold.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("ArchGold");
            });
            btnArchFood.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("ArchFood");
            });
            btnPeople.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("People");
            });
            btnSoldier.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Soldier");
            });
            btnSecure.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Secure");
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
            case "ArchGold":
                return int.Parse(cityArchGold.text);
            case "ArchFood":
                return int.Parse(cityArchFood.text);
            case "People":
                return int.Parse(cityPeople.text);
            case "Soldier":
                return int.Parse(citySoldier.text);
            case "Secure":
                return int.Parse(citySecure.text);
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
        cityArchGold.text = cityData.archGold.ToString();
        cityArchFood.text = cityData.archFood.ToString();
        cityPeople.text = cityData.archPeople.ToString();
        citySoldier.text = cityData.soldier.ToString();
        citySecure.text = cityData.secure.ToString();
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
