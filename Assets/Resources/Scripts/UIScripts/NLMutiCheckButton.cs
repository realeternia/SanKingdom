using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NLMutiCheckButton : MonoBehaviour
{
    public Button button1;

    private List<Button> buttons = new List<Button>();
    private int selectedIndex = -1;
    private Vector3 baseLocalPos;

    public System.Action<int> SelectIndexChange;

    public void Init(string[] names)
    {
        ClearButtons();

        if (names == null || names.Length == 0)
        {
            GameLog.Warn("NLMutiCheckButton Init: names为空");
            return;
        }

        if (button1 == null)
        {
            GameLog.Error("NLMutiCheckButton Init: button1未设置");
            return;
        }

        baseLocalPos = button1.transform.localPosition;
        button1.gameObject.SetActive(true);
        SetButtonText(button1, names[0]);
        buttons.Add(button1);

        for (int i = 1; i < names.Length; i++)
        {
            Button newBtn = Instantiate(button1, transform);
            newBtn.name = "btn" + (i + 1);
            SetButtonText(newBtn, names[i]);
            buttons.Add(newBtn);
        }

        LayoutButtons();

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        SetSelectedIndex(0);
    }

    private void LayoutButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = new Vector3(baseLocalPos.x + i * 150, baseLocalPos.y, baseLocalPos.z);
        }
    }

    private void SetButtonText(Button btn, string text)
    {
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
        }
    }

    private void OnButtonClick(int index)
    {
        if (selectedIndex == index)
            return;
        SetSelectedIndex(index);
        SelectIndexChange?.Invoke(index);
    }

    private void SetSelectedIndex(int index)
    {
        selectedIndex = index;
        for (int i = 0; i < buttons.Count; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (i == selectedIndex) ? SysColor.UI.CheckBtnSelected : SysColor.UI.CheckBtnNormal;
            }
        }
    }

    private void ClearButtons()
    {
        for (int i = buttons.Count - 1; i >= 1; i--)
        {
            if (buttons[i] != null)
            {
                Destroy(buttons[i].gameObject);
            }
        }
        buttons.Clear();
        selectedIndex = -1;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
}
