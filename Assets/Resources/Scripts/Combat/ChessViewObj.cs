using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;

public class ChessViewObj : MonoBehaviour
{     
    public Chess chessUnit;
    private ChessHUD hud;
    public Renderer rend;
    public Material material;
    public Renderer rendFlag;
    public Material materialFlag;    
    private Coroutine colorEffectCoroutine; // 协程引用，用于追踪颜色效果协程

    public int lockTargetId;
    public int moveFailCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(Chess chessUnit, Color c)
    {
        this.chessUnit = chessUnit;

        var chessName = chessUnit.chessName;
          // 创建材质实例
        material = new Material(rend.sharedMaterial);
        if (!string.IsNullOrEmpty(chessName))
        {
            if (chessName.StartsWith("PlayerPic"))
                material.mainTexture = Resources.Load<Texture>(chessName);
            else
                material.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
        }
        material.SetColor("_OutlineColor", c);

        var hasSKill = false;

        if (chessUnit.isHero)
        {
            var heroCfg = HeroConfig.GetConfig(chessUnit.heroId);
            // 初始化技能
            if (heroCfg.Skills != null)
            {
                foreach (var skillId in heroCfg.Skills)
                {
                    var skillCfg = SkillConfig.GetConfig(skillId);
                    if (!string.IsNullOrEmpty(skillCfg.Icon) && !hasSKill)
                    {
                        material.SetTexture("_SecondTex", Resources.Load<Texture>("SkillPic/" + skillCfg.Icon));
                        hasSKill = true;
                    }
                }
            }

            materialFlag = new Material(rendFlag.sharedMaterial);
            var playerInfo = GameManager.Instance.GetPlayer(chessUnit.forceId);
            materialFlag.mainTexture = Resources.Load<Texture>(playerInfo.imgPath);
            rendFlag.material = materialFlag;
        }

        if (!hasSKill)
            material.SetFloat("_SecondTexSize", 0.1f);
        rend.material = material; // 这会为这个渲染器创建一个独立的材质实例

        if(!BattleManager.Instance.quickMode)
            CreateHUD();
    }

    // 创建血条HUD
    private void CreateHUD()
    {
        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>(chessUnit.isHero || chessUnit.isFakeHero ? "Prefabs/Hud" : "Prefabs/HudSmall");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, BattleManager.Instance.battleUIManager.HudNode.transform);
        hudObj.name = "ChessHUD";

        // 获取ChessHUD组件
        hud = hudObj.GetComponent<ChessHUD>();

        // 设置属性
        hud.chessUnit = chessUnit;

        // 初始化血条显示
        hud.UpdateHealthDisplay();

    }    

    public void DestroyHUD()
    {
        if (hud != null)
        {
            Destroy(hud.gameObject);
            hud = null;
        }
    }

    public void AddColorEffect(Color start, Color end)
    {
        // 如果协程已经在运行，则直接返回
        if (colorEffectCoroutine != null)
            return;
        
        colorEffectCoroutine = StartCoroutine(ColorLerpCoroutine(start, end));
    }

    public void RemoveColorEffect()
    {
        // 停止颜色效果协程
        if (colorEffectCoroutine != null)
        {
            StopCoroutine(colorEffectCoroutine);
            colorEffectCoroutine = null;
        }
        
        // 恢复默认颜色
        material.SetColor("_Color", Color.white);
    }

    IEnumerator ColorLerpCoroutine(Color start, Color end)
    {
        float time = 0f;
        while (true)
        {
            // 使用正弦函数实现颜色平滑过渡
            float t = Mathf.Sin(time*20) * 0.5f + 0.5f;
            var color = Color.Lerp(start, end, t);
         //   UnityEngine.Debug.Log("ColorLerpCoroutine " + color + " start=" + start + " end=" + end);

            material.SetColor("_Color", color);
            time += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);

        }
    }

     private Coroutine jumpCoroutine = null;

    public void PlayerAnim(string name)
    {
        if(string.IsNullOrEmpty(name))
            return;
        var animator = GetComponent<Animator>();
        if(animator == null)
            return;
        animator.Play(name);
    }

    public void StartJump(float time)
    {
        var height = 15;
        UnityEngine.Debug.Log("StartJump " + height + " "  + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        // 如果已经在跳跃，先打断当前跳跃
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            transform.position = new Vector3(transform.position.x, 7, transform.position.z); // 恢复到原始位置
        }
        
        jumpCoroutine = StartCoroutine(JumpCoroutine(height, time));
    }

    public void StopJump()
    {
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
            transform.position = new Vector3(transform.position.x, 7, transform.position.z); // 恢复到原始位置
        }
    }

    IEnumerator JumpCoroutine(int jumpHeight, float jumpDuration)
    {
        float elapsedTime = 0f;
        
        Vector3 originalPosition = transform.position;
        while (elapsedTime < jumpDuration)
        {
            float progress = elapsedTime / jumpDuration;
            
            // 使用抛物线运动：y = 4h * (x - x²) 其中h是最大高度
            float height = 4f * jumpHeight * (progress - progress * progress) + 7;
            
            // 更新位置
            Vector3 newPosition = originalPosition;
            newPosition.y += height;
            transform.position = Vector3.Lerp(originalPosition, newPosition, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终回到原始位置
        transform.position = new Vector3(transform.position.x, 7, transform.position.z);
        jumpCoroutine = null;
    }

    private void OnDestroy()
    {

    }    
}