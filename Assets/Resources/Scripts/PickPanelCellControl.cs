using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PickPanelCellControl : MonoBehaviour
{
    public Image bgImg;
    public Image heroImg;
    public Image checkImg;
    public TMP_Text heroName;
    public int heroId;

    private bool isSelected = false;
    private PickPanelControl parentControl;

    // 设置父控制类引用
    public void SetParentControl(PickPanelControl control)
    {
        parentControl = control;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初始隐藏选中图标
        checkImg.gameObject.SetActive(false);
        
        // 添加点击事件监听
        if (heroImg != null)
        {
            Button button = heroImg.GetComponent<Button>();
            button.onClick.AddListener(OnHeroImgClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 英雄图片点击事件处理
    private void OnHeroImgClick()
    {
        // 切换选中状态
        isSelected = !isSelected;
        
        // 更新选中图标的可见性
        checkImg.gameObject.SetActive(isSelected);
        
        // 如果当前选中，取消其他所有单元格的选中状态
        if (isSelected && parentControl != null)
        {
            parentControl.ClearAllSelectionsExcept(this);
        }
    }

    // 设置选中状态（外部调用）
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        checkImg.gameObject.SetActive(isSelected);
    }

    // 获取选中状态
    public bool IsSelected()
    {
        return isSelected;
    }
}
