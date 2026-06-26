using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class SideArmsItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text NameText;
    public TMP_Text AtkText;
    public TMP_Text DefText;
    public Image IconCost1;
    public Image IconCost2;
    public TMP_Text Cost1Text;
    public TMP_Text Cost2Text;

    private bool isSelected = false;

    public Button button;
    private System.Action<SideArmsItem> onClickCallback;
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

    public void SetData(int armId, SaveTroopsData troop = null)
    {
        armsId = armId;
        ArmsConfig config = ArmsConfig.GetConfig(armId);

        if (troop != null)
        {
            int sodValue = troop.GetSodValue(armId);
            string gradeColored = SysColor.GetColoredTextWithRule(config.Type.ToString(), sodValue);
            NameText.text = $"{config.NameS} ({gradeColored})";
        }
        else
        {
            NameText.text = config.NameS;
        }
        NameText.color = GetColorByLevel(config.Level);
        AtkText.text = config.Atk.ToString();
        DefText.text = config.Def.ToString();

        List<(int attrId, int cost)> costs = new List<(int, int)>();
        
        if (config.HorseCost > 0)
            costs.Add((14, config.HorseCost));
        if (config.SteelCost > 0)
            costs.Add((13, config.SteelCost));
        if (config.WoodCost > 0)
            costs.Add((15, config.WoodCost));
        if (config.StoneCost > 0)
            costs.Add((16, config.StoneCost));

        if (costs.Count > 0)
        {
            IconCost1.gameObject.SetActive(true);
            Cost1Text.gameObject.SetActive(true);
            IconCost1.gameObject.GetComponent<IconLoader>().SetId(costs[0].attrId);
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
            IconCost2.gameObject.GetComponent<IconLoader>().SetId(costs[1].attrId);
            Cost2Text.text = costs[1].cost.ToString();
        }
        else
        {
            IconCost2.gameObject.SetActive(false);
            Cost2Text.gameObject.SetActive(false);
        }

        SetSelected(false);
    }

    public void SetOnClickCallback(System.Action<SideArmsItem> callback)
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
            BG.color = isSelected ? SysColor.UI.MatchColor : SysColor.Theme.CellNormalDark;
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
        return SysColor.GetArmsLevelColor(level);
    }
}
