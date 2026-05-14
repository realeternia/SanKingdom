using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;
public class CastleHUD : MonoBehaviour
{
    public TMP_Text castleName;
    public TMP_Text textAtk;
    public TMP_Text textHp;
    public Image healthImg;
    private SaveForceData owner;
    private bool isFlashing = false;

    private int baseAtk;
    private int baseHp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(SaveForceData force, Vector3 castleSpawn)
    {
         owner = force;
        castleName.text = force.Name;

        var castleUnitCfg = BattleUnitConfig.GetConfig(500001);
        baseAtk = castleUnitCfg.Atk;
        baseHp = castleUnitCfg.Hp;
        textAtk.text = baseAtk.ToString();
        textHp.text = baseHp.ToString();

        UpdatePosition(castleSpawn);
    }

    private void UpdatePosition(Vector3 castleSpawn)
    {
        Vector3 worldPosition = new Vector3(castleSpawn.x + 5, castleSpawn.y + 3f, castleSpawn.z + 5);
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = BattleManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);
        rectTransform.anchoredPosition = screenPosition + new Vector2(-75, 0);
    }
}