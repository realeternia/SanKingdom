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
    public CityDetail cityDetail;
    private GameObject currentCityView; // 当前加载的城市视图
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
        var cityCfg = WorldConfig.GetConfig(cityId);
        cityName.text = cityCfg.Cname;
        
        // 加载城市视图预制件
        LoadCityView(cityCfg.ViewPrefab);
        cityDetail.SetCityDetail(cityId);
    }

    private void LoadCityView(string viewPrefabPath)
    {
        // 先销毁当前的城市视图（如果存在）
        if (currentCityView != null)
        {
            Destroy(currentCityView);
        }

        // 加载chengdu预制件
        GameObject cityViewPrefab = Resources.Load<GameObject>("Prefabs/CityView/" + viewPrefabPath);
        if (cityViewPrefab != null)
        {
            // 实例化预制件并挂载到父对象下
            currentCityView = Instantiate(cityViewPrefab, transform);
            currentCityView.transform.localScale = Vector3.one;
            currentCityView.transform.Find("ButtonArmy").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelManager.Instance.ShowPopCitySelectPanel(cityId);
            });
            currentCityView.transform.Find("ButtonGate").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelManager.Instance.ShowPopResultPanel("fix2.mp4");
            });
            currentCityView.transform.Find("ButtonTian").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelManager.Instance.ShowPopResultPanel("harve.mp4");
            });
            currentCityView.transform.Find("ButtonYa").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelManager.Instance.ShowPopResultPanel("fix2.mp4");
            });

            currentCityView.transform.Find("ButtonJishi").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                PanelManager.Instance.ShowPopHeroSelectPanel(cityId);
                //PanelManager.Instance.ShowPopResultPanel("shop2.mp4");
            });
        }
        else
        {
            Debug.LogError("Failed to load city view prefab or cityViewParent is not assigned.");
        }
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
