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
    public Button buttonInfo;
    public GameObject BattleResultPanel;
    public GameObject BattleResultCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    public GameObject BattleResultHeroCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    private List<GameObject> battleResultCells = new List<GameObject>(); // 维护创建的结果单元格列表

    public GameObject HudNode;
    public GameObject BattleTextNode;


    void Start()
    {
        buttonRestart.onClick.AddListener(BattleEnd);
        buttonInfo.onClick.AddListener(ShowBattleResult);

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

    public void BattleEnd()
    {
        // 销毁所有结果单元格
        foreach (GameObject cell in battleResultCells)
        {
            if (cell != null)
                Destroy(cell);
        }
        battleResultCells.Clear();
        
        foreach (Transform child in NodeUnits.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform cell in HudNode.transform)
        {
            Destroy(cell.gameObject);
        }

        PanelManager.Instance.ShowWorld();
    }    

    public void OnBattleEnd(List<int> playerList, bool hasWin, bool replay)
    {
        if (hasWin)
            textRestart.text = "你获胜了!!!";
        else
            textRestart.text = "你输了!!!";

        // 销毁之前的结果单元格
        foreach (GameObject cell in battleResultCells)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        battleResultCells.Clear();

        // 为每个玩家创建结果单元格
        if (BattleResultCellPrefab != null)
        {
            // 根据玩家的 mark 进行排序
            var sortedPlayers = playerList
                .Select(id => new { Id = id, Mark = GameManager.Instance.GetPlayer(id)?.mark ?? 0 })
                .OrderByDescending(p => p.Mark)
                .Select(p => p.Id)
                .ToArray();
            for (int i = 0; i < sortedPlayers.Length; i++)
            {
                int playerId = sortedPlayers[i];
                // 创建结果单元格
                GameObject cell = Instantiate(BattleResultCellPrefab, BattleResultPanel.transform);

                // 设置位置，每个单元格垂直偏移50
                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(302, -120 - i * 50); // 起始位置向下100，每个单元格间距50
                }

                // 获取并设置单元格数据
                BattleResultCellControl cellControl = cell.GetComponent<BattleResultCellControl>();
                if (cellControl != null)
                {
                    var player = GameManager.Instance.GetPlayer(playerId);
                    if (player != null)
                    {
                        cellControl.SetData(player, i + 1, 1);
                    }
                }

                // 添加到维护列表
                battleResultCells.Add(cell);
            }
        }
        buttonInfo.gameObject.SetActive(true);
        // 获取RectTransform组件并设置宽度
        RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
        battleResultRect.sizeDelta = new Vector2(650, battleResultRect.sizeDelta.y);
        BattleResultPanel.gameObject.SetActive(true);

        if(!replay)
            PanelManager.Instance.SendSignal("CityAttrChange", "", 0); //士兵数变了
    }

    public void ShowBattleResult()
    {
        var top10 = BattleStatManager.GetTop10();
        buttonInfo.gameObject.SetActive(false);
        // 获取RectTransform组件并设置宽度
        RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
        battleResultRect.sizeDelta = new Vector2(battleResultRect.sizeDelta.x + 800, battleResultRect.sizeDelta.y);
        for (int i = 0; i < top10.Count; i++)
        {
            var battleStat = top10[i];
            var cell = Instantiate(BattleResultHeroCellPrefab, BattleResultPanel.transform);
            cell.GetComponent<BattleResultHeroCellControl>().SetData(battleStat, i + 1);

            // 设置位置，每个单元格垂直偏移50
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(302 + 700, -120 - i * 50); // 起始位置向下100，每个单元格间距50

            battleResultCells.Add(cell);
        }
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