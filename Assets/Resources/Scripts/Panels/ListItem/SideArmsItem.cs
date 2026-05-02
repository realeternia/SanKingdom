using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class SelectArmsItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text NameText;
    public TMP_Text AtkText;
    public TMP_Text DefText;
    public Image IconCost1;
    public Image IconCost2;
    public TMP_Text Cost1Text;
    public TMP_Text Cost2Text;

    private Color normalColor = Color.black;
    private Color selectedColor = new Color(0.3f, 0.7f, 0.3f, 1);
    private bool isSelected = false;

    public Button button;
    private System.Action<SelectArmsItem> onClickCallback;
    private int armsId;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnItemClick);
        }
    }

    void Update()
    {
        
    }

    public void SetData(int armId)
    {
        armsId = armId;
        ArmsConfig config = ArmsConfig.GetConfig(armId);
        NameText.text = config.NameS;
        NameText.color = GetColorByLevel(config.Level);
        AtkText.text = config.Atk.ToString();
        DefText.text = config.Def.ToString();

        List<(string icon, int cost)> costs = new List<(string, int)>();
        
        if (config.HorseCost > 0)
            costs.Add(("cityhorse", config.HorseCost));
        if (config.SteelCost > 0)
            costs.Add(("citysteel", config.SteelCost));
        if (config.WoodCost > 0)
            costs.Add(("citywood", config.WoodCost));
        if (config.StoneCost > 0)
            costs.Add(("citystone", config.StoneCost));

        if (costs.Count > 0)
        {
            IconCost1.gameObject.SetActive(true);
            Cost1Text.gameObject.SetActive(true);
            Sprite sprite1 = Resources.Load<Sprite>($"Textures/Icons/{costs[0].icon}");
            if (sprite1 != null)
                IconCost1.sprite = sprite1;
            Cost1Text.text = costs[0].cost.ToString();
        }
        else
        {
            IconCost1.gameObject.SetActive(false);
            Cost1Text.gameObject.SetActive(false);
        }

        if (costs.Count > 1)
        {
            IconCost2.gameObject.SetActive(true);
            Cost2Text.gameObject.SetActive(true);
            Sprite sprite2 = Resources.Load<Sprite>($"Textures/Icons/{costs[1].icon}");
            if (sprite2 != null)
                IconCost2.sprite = sprite2;
            Cost2Text.text = costs[1].cost.ToString();
        }
        else
        {
            IconCost2.gameObject.SetActive(false);
            Cost2Text.gameObject.SetActive(false);
        }

        SetSelected(false);
    }

    public void SetOnClickCallback(System.Action<SelectArmsItem> callback)
    {
        onClickCallback = callback;
    }

    public void OnItemClick()
    {
        onClickCallback?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (BG != null)
        {
            BG.color = isSelected ? selectedColor : normalColor;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public int GetArmsId()
    {
        return armsId;
    }

    private static Color GetColorByLevel(int level)
    {
        return SystemConst.Arms.GetColorByLevel(level);
    }
}
