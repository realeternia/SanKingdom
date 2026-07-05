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

    public GameObject heroInfoRectSide1;
    public GameObject heroInfoRectSide2;
    public GameObject heroPrefab;
    private int heroCountSide1;
    private int heroCountSide2;

    public Button buttonRestart;
    public TMP_Text textRestart;

    public GameObject BattleResultPanel;

    public GameObject HudNode;
    public GameObject BattleTextNode;

    public Button deployConfirmButton;


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

    public void ShowBattleBegin(SaveForceData force1, SaveForceData force2, int maxRound, int soldierNum1, int soldierNum2)
    {
        PanelManager.Instance.HideWorld();

        BattleInfoTop.Instance.Init(force1.forceId, force2.forceId, soldierNum1, soldierNum2);
        BattleResultPanel.gameObject.SetActive(false);
        foreach (Transform child in NodeUnits.transform)
            UnityEngine.Object.Destroy(child.gameObject);
        foreach (Transform child in HudNode.transform)
            UnityEngine.Object.Destroy(child.gameObject);
        ResetHeroInfo();

        BattleInfoTop.Instance.UpdateRound(0, maxRound);
    }


    public void OnBattleEnd(BattleResult result, bool replay)
    {
        // BattleResult.Win/Lose 是从攻击方(side[0])视角定义的
        // 玩家在防守方(side[1])时需要翻转
        BattleResult playerResult = result;
        if (BattleManager.Instance.playerSideIndex == 1)
        {
            if (result == BattleResult.Win) playerResult = BattleResult.Lose;
            else if (result == BattleResult.Lose) playerResult = BattleResult.Win;
        }

        if (playerResult == BattleResult.Win)
            textRestart.text = "你获胜了!!!";
        else if (playerResult == BattleResult.Lose)
            textRestart.text = "你输了!!!";
        else
            textRestart.text = "平局!!!";

        BattleResultPanel.gameObject.SetActive(true);

        if(!replay)
            PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = 0 });
    }

    public void OnEndButtonClick()
    {
        if (BattleManager.Instance.isDeployPhase)
        {
            BattleManager.Instance.isDeployPhase = false;
            BattleManager.Instance.IsBattleRunning = false;
            BattleManager.Instance.chessList.Clear();
        }

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

    public void ShowDeployConfirmButton()
    {
        if (deployConfirmButton != null)
        {
            deployConfirmButton.onClick.RemoveAllListeners();
            deployConfirmButton.onClick.AddListener(OnDeployConfirmClick);
            deployConfirmButton.gameObject.SetActive(true);
        }
    }

    public void HideDeployConfirmButton()
    {
        if (deployConfirmButton != null)
            deployConfirmButton.gameObject.SetActive(false);
    }

    private void OnDeployConfirmClick()
    {
        BattleManager.Instance.OnDeployConfirm();
    }       

    public void AddBattleText(string text, Vector3 worldPos, Vector2 speed, Color color, int duration)
    {
        var prefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.BattleTxt());
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

        GameLog.Info($"screenWidth:{screenWidth} screenHeight:{screenHeight}");

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

    public void ResetHeroInfo()
    {
        heroCountSide1 = 0;
        heroCountSide2 = 0;
        foreach (Transform child in heroInfoRectSide1.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in heroInfoRectSide2.transform)
        {
            Destroy(child.gameObject);
        }
        GameLog.Debug("Reset " + heroInfoRectSide1.transform.childCount + " " + heroInfoRectSide2.transform.childCount);
    }

    public BattleHeroInfo AddHero(int forceId, int heroId, int level, int heroId2, int heroId3, int inte, int atk, int def)
    {
        var attackerForceId = BattleManager.Instance.playerInfoList[0].forceId;
        bool isSide1 = forceId == attackerForceId;
        int count = isSide1 ? heroCountSide1 : heroCountSide2;
        GameObject heroInfoRect = isSide1 ? heroInfoRectSide1 : heroInfoRectSide2;

        BattleHeroInfo heroInfo = Instantiate(heroPrefab, heroInfoRect.transform).GetComponent<BattleHeroInfo>();
        heroInfo.transform.localPosition = new Vector3(0, -60 - 120 * count, 0);
        heroInfo.Init(heroId, level, heroId2, heroId3, inte, atk, def);

        if(isSide1)
        {
            heroCountSide1++;
        }
        else
        {
            heroCountSide2++;
        }

        heroInfoRect.GetComponent<RectTransform>().sizeDelta = new Vector2(heroInfoRect.GetComponent<RectTransform>().sizeDelta.x, 120 * count + 3);

        return heroInfo;
    }
}