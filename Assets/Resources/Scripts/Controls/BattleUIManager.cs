using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class BattleUIManager : MonoBehaviour
{

    public Camera uiCamera;
    public bool isDebug = true; //自动判定的，不要改
    public GameObject NodeUnits;
   
    public HeroInfoGroup heroInfoGroup;
    public Button buttonRestart;
    public TMP_Text textRestart;

    public GameObject BattleResultPanel;

    public GameObject HudNode;
    public GameObject BattleTextNode;


    void Start()
    {
        buttonRestart.onClick.AddListener(OnEndButtonClick);

        StartCoroutine(DebugBattleBeginCheck());
        BattleManager.Instance.battleUIManager = this;
    }

    IEnumerator DebugBattleBeginCheck()
    {      
        // 延迟2秒
        yield return new WaitForSeconds(2f);
        ConfigManager.Init();
        if(isDebug)
        {
           // BattleBegin();
        }
    }

    // 创建血条HUD
    public void CreateCastleHUD(Player p, Vector3 castleSpawn)
    {
        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/HudCastle");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, HudNode.transform);
        hudObj.name = "CastleHUD";

        // 获取ChessHUD组件
        var hud = hudObj.GetComponent<CastleHUD>();

        // 初始化血条显示
        hud.Init(p, castleSpawn);
        p.castleHUD = hud;
    }

    public void ShowBattleBegin(Player player1, Player player2, int maxRound, int soldierNum1, int soldierNum2)
    {
        PanelManager.Instance.HideCity();
        PanelManager.Instance.HideWorld();

        BattleInfoTop.Instance.Init(player1.forceId, player2.forceId, soldierNum1, soldierNum2);
        BattleResultPanel.gameObject.SetActive(false);
        foreach (Transform child in NodeUnits.transform)
            UnityEngine.Object.Destroy(child.gameObject);
        heroInfoGroup.Reset();

        BattleInfoTop.Instance.UpdateRound(0, maxRound);
    }


    public void OnBattleEnd(BattleResult result, bool replay)
    {
        if (result == BattleResult.Win)
            textRestart.text = "你获胜了!!!";
        else if (result == BattleResult.Lose)
            textRestart.text = "你输了!!!";
        else
            textRestart.text = "平局!!!";

        BattleResultPanel.gameObject.SetActive(true);

        if(!replay)
            PanelManager.Instance.SendSignal("CityAttrChange", "", 0); //士兵数变了
    }

    public void OnEndButtonClick()
    {       
        foreach (Transform child in NodeUnits.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform cell in HudNode.transform)
        {
            Destroy(cell.gameObject);
        }

        PanelManager.Instance.ShowWorld();
        PanelManager.Instance.ShowBattleResultPanel(BattleManager.Instance.battleId);
    }       

    public void AddBattleText(string text, Vector3 worldPos, Vector2 speed, Color color, int duration)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/BattleTxt");
        var battleText = Instantiate(prefab, BattleTextNode.transform);

        // 将世界坐标转换为屏幕坐标
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPos = TransformWorldToScreen(worldPos + new UnityEngine.Vector3(5, 0, 5), parentCanvas);

        rectTransform.anchoredPosition = screenPos;

        var textCtr = battleText.transform.GetChild(0).GetComponent<TMP_Text>();
        textCtr.color = color;
        textCtr.text = text;
        Destroy(battleText, duration);

        //如果speed不为0，开一个协程移动文本
        if(speed != UnityEngine.Vector2.zero)
        {
            StartCoroutine(MoveText(battleText, speed, duration));
        }
    }

    // 战斗文本移动协程
    private IEnumerator MoveText(GameObject battleText, UnityEngine.Vector2 speed, int duration)
    {
        //获得屏幕分辨率
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        UnityEngine.Debug.Log($"screenWidth:{screenWidth} screenHeight:{screenHeight}");

        // 假设设计分辨率为 1920x1080，可根据实际项目修改
        const float designWidth = 2048f;
        const float designHeight = 1536f;
        // 根据当前屏幕分辨率计算缩放比例
        float scaleX = (float)screenWidth / designWidth;
        float scaleY = (float)screenHeight / designHeight;

        var nowTime = Time.time;
        float startTime = nowTime;
        float endTime = startTime + duration;
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        var lastTime = nowTime;

        while (lastTime < endTime)
        {
            // 考虑分辨率和缩放因素计算移动距离
            var timeDiff = Time.time - lastTime;
            lastTime = Time.time;

            float moveX = speed.x * timeDiff * scaleX;
            float moveY = speed.y * timeDiff * scaleY / 80;

            if (rectTransform == null)
            {
                Destroy(battleText);
                yield break;
            }

            // 更新位置
            rectTransform.Translate(new Vector3(moveX, moveY, 0));

            yield return new WaitForSeconds(0.05f); // 使用 yield return null 在下一帧继续执行，保证流畅移动

        }
    }

    public Vector2 TransformWorldToScreen(Vector3 worldPosition, RectTransform canvas)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            screenPosition,
            uiCamera,
            out localPosition
        );

        return localPosition;
    }


    // 管理器销毁时释放所有格子
    private void OnDestroy()
    {

    }
}