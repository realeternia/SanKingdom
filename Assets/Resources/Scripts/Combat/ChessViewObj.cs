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
    public GameObject flagObj;
    public Renderer rendFlag;
    public Material materialFlag;    
    private Material originalMaterial;
    private Texture originalTexture;
    private string currentMaterialType = "default";

    public int lockTargetId;
    private List<GameObject> soldiers = new List<GameObject>();
    private Color outlineColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(Chess chessUnit, Color c)
    {
        this.chessUnit = chessUnit;
        this.outlineColor = c;

        if (chessUnit.isSodNull)
        {
            originalMaterial = rend.sharedMaterial;
            material = new Material(rend.sharedMaterial);
            rend.material = material;
            if (flagObj != null)
                flagObj.SetActive(false);
            UpdateSoldierModels();
            return;
        }

        var chessName = chessUnit.chessName;
        originalMaterial = rend.sharedMaterial;
        material = new Material(rend.sharedMaterial);
        originalTexture = ResourceCache.LoadBattle<Texture>(ResPath.Texture.HeroIcon(chessName));
        if (!string.IsNullOrEmpty(chessName))
        {
            material.mainTexture = originalTexture;
        }

        if (chessUnit.isHero)
        {
            materialFlag = new Material(rendFlag.sharedMaterial);
            materialFlag.mainTexture = ResourceCache.LoadBattle<Texture>(ResPath.Texture.HeroIcon(chessName));
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
        GameObject hudPrefab = ResourceCache.LoadPrefabBattle(chessUnit.isHero || chessUnit.isFakeHero ? ResPath.Prefab.Hud() : ResPath.Prefab.HudSmall());

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

     private Coroutine jumpCoroutine = null;

    public void PlayAnim(string name)
    {
        if(string.IsNullOrEmpty(name))
            return;
        var animator = GetComponent<Animator>();
        if(animator == null)
            return;
        animator.Play(name);
    }

    public void PlaySodAnim(string name)
    {
        if(string.IsNullOrEmpty(name))
            return;
        if(BattleManager.Instance.quickMode)
            return;
        
        foreach (var soldier in soldiers)
        {
            if (soldier != null)
            {
                var animator = soldier.transform.Find("body")?.GetComponent<Animator>();
                animator?.Play(name);
            }
        }
    }

    public void FaceTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        
        if (direction.sqrMagnitude < 0.001f)
            return;
        
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;       
        foreach (var soldier in soldiers)
        {
            if (soldier != null)
            {
                Vector3 currentRotation = soldier.transform.localRotation.eulerAngles;
                soldier.transform.localRotation = Quaternion.Euler(currentRotation.x, targetAngle, currentRotation.z);
            }
        }
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
        string sodType;
        int modelCountFactor;

        if (chessUnit.isSodNull)
        {
            sodType = "SodNull";
            modelCountFactor = 1;
        }
        else
        {
            var armsConfig = ArmsConfig.GetConfig(chessUnit.armsId);
            sodType = armsConfig.Model;
            modelCountFactor = armsConfig.ModelCountFactor;
        }
        
        GameObject soldierPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Arms(sodType));
        if (soldierPrefab == null)
        {
            GameLog.Warn(sodType + " prefab not found!");
            return;
        }

        int targetCount = (int)Math.Ceiling((float)chessUnit.hp / modelCountFactor);
        int currentCount = soldiers.Count;

        if (targetCount > currentCount)
        {
            int gridSize = 5;
            float spacing = 2.7f;
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
                
                var meshMgr = soldier.transform.Find("body")?.GetComponent<UnityMeshMgr>();
                if (meshMgr != null)
                {
                    foreach (var mesh in meshMgr.meshes)
                    {
                        if (mesh != null)
                        {
                            var meshRenderer = mesh.GetComponent<Renderer>();
                            if (meshRenderer != null)
                            {
                                var meshMaterial = new Material(meshRenderer.sharedMaterial);
                                meshMaterial.SetColor("_OutlineColor", outlineColor);
                                meshRenderer.material = meshMaterial;
                            }
                        }
                    }
                }

                // if (UnityEngine.Random.value > 0.5f)
                // {
                //     SwitchMaterialByName(UnityEngine.Random.value > 0.5f ? "silver" : "gold");
                // }

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
                var dissolveEffect = soldier.AddComponent<DissolveEffect>();
                dissolveEffect.StartDissolve();
            }
        }
    }

    public void SwitchMaterialByName(string materialName)
    {
        if (rend == null) return;

        if (currentMaterialType == materialName) return;

        currentMaterialType = materialName;

        Material baseMat = null;
        Color targetColor = Color.white;
        Color emissionColor = Color.black;
        Color outlineColor = Color.white;
        Color specColor = Color.white;
        float metallic = 1f;
        float glossiness = 0.8f;
        float emissionStrength = 0.5f;

        switch (materialName)
        {
            case "gold":
                baseMat = ResourceCache.LoadBattle<Material>(ResPath.Material.GoldChess());
                targetColor = SysColor.Chess.GoldMain;
                emissionColor = SysColor.Chess.GoldEmission;
                outlineColor = SysColor.Chess.GoldOutline;
                specColor = SysColor.Chess.GoldSpec;
                metallic = 1f;
                glossiness = 0.85f;
                emissionStrength = 0.5f;
                break;
            case "silver":
                baseMat = ResourceCache.LoadBattle<Material>(ResPath.Material.SilverChess());
                targetColor = SysColor.Chess.SilverMain;
                emissionColor = SysColor.Chess.SilverEmission;
                outlineColor = SysColor.Chess.SilverOutline;
                specColor = SysColor.Chess.SilverSpec;
                metallic = 1f;
                glossiness = 0.9f;
                emissionStrength = 0.3f;
                break;
            default:
                if (originalMaterial != null)
                {
                    material = new Material(originalMaterial);
                    if (originalTexture != null)
                    {
                        material.mainTexture = originalTexture;
                    }
                    material.SetColor("_OutlineColor", this.outlineColor);
                    rend.material = material;
                }
                SwitchSoldiersMaterialDefault();
                return;
        }

        if (baseMat != null)
        {
            material = new Material(baseMat);
            if (originalTexture != null)
            {
                material.mainTexture = originalTexture;
            }
            material.SetColor("_Color", targetColor);
            material.SetColor("_EmissionColor", emissionColor);
            material.SetColor("_OutlineColor", outlineColor);
            material.SetColor("_SpecColor", specColor);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", glossiness);
            material.SetFloat("_EmissionStrength", emissionStrength);
            rend.material = material;
            
            SwitchSoldiersMaterial(baseMat, targetColor, emissionColor, outlineColor, specColor, metallic, glossiness, emissionStrength);
        }
    }

    private void SwitchSoldiersMaterial(Material baseMat, Color targetColor, Color emissionColor, Color outlineColor, Color specColor, float metallic, float glossiness, float emissionStrength)
    {
        foreach (var soldier in soldiers)
        {
            if (soldier == null) continue;
            
            Renderer[] soldierRenderers = soldier.GetComponentsInChildren<Renderer>();
            foreach (var soldierRend in soldierRenderers)
            {
                if (soldierRend == null) continue;
                
                Material newMat = new Material(baseMat);
                
                if (soldierRend.sharedMaterial != null && soldierRend.sharedMaterial.mainTexture != null)
                {
                    newMat.mainTexture = soldierRend.sharedMaterial.mainTexture;
                }
                
                newMat.SetColor("_Color", targetColor);
                newMat.SetColor("_EmissionColor", emissionColor);
                newMat.SetColor("_OutlineColor", outlineColor);
                newMat.SetColor("_SpecColor", specColor);
                newMat.SetFloat("_Metallic", metallic);
                newMat.SetFloat("_Glossiness", glossiness);
                newMat.SetFloat("_EmissionStrength", emissionStrength);
                
                soldierRend.material = newMat;
            }
        }
    }

    private void SwitchSoldiersMaterialDefault()
    {
        foreach (var soldier in soldiers)
        {
            if (soldier == null) continue;
            
            Renderer[] soldierRenderers = soldier.GetComponentsInChildren<Renderer>();
            foreach (var soldierRend in soldierRenderers)
            {
                if (soldierRend == null) continue;
                
                if (soldierRend.sharedMaterial != null)
                {
                    Material newMat = new Material(soldierRend.sharedMaterial);
                    newMat.SetColor("_OutlineColor", outlineColor);
                    soldierRend.material = newMat;
                }
            }
        }
    }

    private void OnDestroy()
    {

    }    
}