using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Controls.Utils;

public class BattleHeroInfo : MonoBehaviour
{
    public TMP_Text heroName;
    public TMP_Text heroLevelTxt;
    public TMP_Text heroHpTxt;
    public Image heroImage;
    public Image hero2Image;
    public Image hero3Image;
    public Image healthImg;
    public Image errorImg;

    public TMP_Text heroInteTxt;
    public TMP_Text heroAtkTxt;
    public TMP_Text heroDefTxt;

    void Start()
    {
    }

    public void Init(int heroId, int level, int heroId2, int heroId3, int inte, int atk, int def)
    {
        errorImg.gameObject.SetActive(false);
        heroImage.gameObject.SetActive(true);
        var heroCfg = HeroConfig.GetConfig(heroId);
        var iconPath = ResPath.Texture.HeroIcon(heroCfg.Icon);
        var sprite = ResourceCache.LoadSpriteBattle(iconPath);
        GameLog.Info($"BattleHeroInfo.Init heroId={heroId} icon={heroCfg.Icon} path={iconPath} sprite={sprite}");
        heroImage.sprite = sprite;
        heroName.text = heroCfg.Name;
        heroLevelTxt.text = level.ToString();
        SetViceHeroImage(hero2Image, heroId2);
        SetViceHeroImage(hero3Image, heroId3);
        SetAttr(inte, atk, def);
    }

    public void SetAttr(int inte, int atk, int def)
    {
        SetText(heroInteTxt, inte);
        SetText(heroAtkTxt, atk);
        SetText(heroDefTxt, def);
    }

    private void SetViceHeroImage(Image image, int heroId)
    {
        if (heroId <= 0)
        {
            image.gameObject.SetActive(false);
            return;
        }
        image.gameObject.SetActive(true);
        var heroCfg = HeroConfig.GetConfig(heroId);
        image.sprite = ResourceCache.LoadSpriteBattle(ResPath.Texture.HeroIcon(heroCfg.Icon));
    }

    private void SetText(TMP_Text text, int val)
    {
        text.text = val.ToString();
        if (val >= 95)
        {
            text.color = Color.red;
        }
        else if (val >= 90)
        {
            text.color = new Color(1, 0.5f, 0);
        }
        else if (val >= 80)
        {
            text.color = Color.yellow;
        }
        else if (val >= 70)
        {
            text.color = Color.green;
        }
    }

    public void SetHpRate(int hp, int maxHp)
    {
        if (maxHp <= 0)
            return;
        if(hp < 0)
            hp = 0;
        var hpRate = (float)hp / maxHp;
        heroHpTxt.text = hp + " / " + maxHp;
        healthImg.rectTransform.sizeDelta = new Vector2((int)(hpRate * 185), healthImg.rectTransform.sizeDelta.y);
        if (hpRate <= 0)
        {
            errorImg.gameObject.SetActive(true);
            heroName.color = Color.gray;
            heroLevelTxt.color = Color.gray;
        }
        else if(hpRate <= 0.5)
            healthImg.color = new Color(0.4f, .33f, 0);
        else
            healthImg.color = new Color(0f, .4f, .1f);
    }
}
