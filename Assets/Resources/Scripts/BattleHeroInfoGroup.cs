using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeroInfoGroup : MonoBehaviour
{
    public GameObject heroInfoRectSide1;
    public GameObject heroInfoRectSide2;
    private int countSide1;
    private int countSide2;
    public GameObject heroPrefab;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        countSide1 = 0;
        countSide2 = 0;
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
        int count = isSide1 ? countSide1 : countSide2;
        GameObject heroInfoRect = isSide1 ? heroInfoRectSide1 : heroInfoRectSide2;
        
        BattleHeroInfo heroInfo = Instantiate(heroPrefab, heroInfoRect.transform).GetComponent<BattleHeroInfo>();
        heroInfo.transform.localPosition = new Vector3(0, -60 - 120 * count, 0);
        heroInfo.Init(heroId, level, heroId2, heroId3, inte, atk, def);

        if(isSide1)
        {
            countSide1++;
        }
        else
        {
            countSide2++;
        }

        heroInfoRect.GetComponent<RectTransform>().sizeDelta = new Vector2(heroInfoRect.GetComponent<RectTransform>().sizeDelta.x, 120 * count + 3);

        return heroInfo;
    }
}
