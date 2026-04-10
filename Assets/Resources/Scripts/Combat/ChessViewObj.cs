using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;
using Controls.Utils;

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
    private List<GameObject> soldiers = new List<GameObject>();

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
            material.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
        }
        //material.SetColor("_OutlineColor", c);

        if (chessUnit.isHero)
        {
            var heroCfg = HeroConfig.GetConfig(chessUnit.heroId);

            materialFlag = new Material(rendFlag.sharedMaterial);
            materialFlag.mainTexture = Resources.Load<Texture>("Skins/" + chessName);
            materialFlag.SetColor("_OutlineColor", c);
            rendFlag.material = materialFlag;
        }

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
        GameLog.Info("StartJump " + height + " "  + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

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

    public void UpdateSoldierModels()
    {
        GameObject soldierPrefab = Resources.Load<GameObject>("Prefabs/Arms/SodBow");
        if (soldierPrefab == null)
        {
            Debug.LogWarning("SodBow prefab not found!");
            return;
        }

        int targetCount = chessUnit.hp / 40;
        int currentCount = soldiers.Count;

        if (targetCount > currentCount)
        {
            int gridSize = 5;
            float spacing = 0.2f;
            float offsetX = -((gridSize - 1) * spacing) / 2f;
            float offsetZ = -((gridSize - 1) * spacing) / 2f;

            List<(int index, float distance)> positions = new List<(int, float)>();
            for (int i = 0; i < 25; i++)
            {
                int row = i / gridSize;
                int col = i % gridSize;
                float x = offsetX + col * spacing;
                float z = offsetZ + row * spacing;
                float distance = Mathf.Sqrt(x * x + z * z);
                positions.Add((i, distance));
            }
            positions.Sort((a, b) => a.distance.CompareTo(b.distance));

            for (int i = currentCount; i < targetCount && i < 25; i++)
            {
                int index = positions[i].index;
                int row = index / gridSize;
                int col = index % gridSize;

                Vector3 localPos = new Vector3(
                    offsetX + col * spacing,
                    0f,
                    offsetZ + row * spacing
                );

                GameObject soldier = UnityEngine.Object.Instantiate(soldierPrefab, transform);
                soldier.transform.localPosition = localPos;
                soldier.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                soldier.name = $"Soldier_{i}";
                soldiers.Add(soldier);
            }
        }
        else if (targetCount < currentCount)
        {
            int gridSize = 5;
            float spacing = 0.2f;
            float offsetX = -((gridSize - 1) * spacing) / 2f;
            float offsetZ = -((gridSize - 1) * spacing) / 2f;

            List<(GameObject obj, float distance)> soldierDistances = new List<(GameObject, float)>();
            foreach (var soldier in soldiers)
            {
                if (soldier != null)
                {
                    Vector3 localPos = soldier.transform.localPosition;
                    float distance = Mathf.Sqrt(localPos.x * localPos.x + localPos.z * localPos.z);
                    soldierDistances.Add((soldier, distance));
                }
            }
            soldierDistances.Sort((a, b) => b.distance.CompareTo(a.distance));

            int removeCount = currentCount - targetCount;
            for (int i = 0; i < removeCount && i < soldierDistances.Count; i++)
            {
                var soldier = soldierDistances[i].obj;
                soldiers.Remove(soldier);
                UnityEngine.Object.Destroy(soldier);
            }
        }
    }

    private void OnDestroy()
    {

    }    
}