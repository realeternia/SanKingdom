using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
[Serializable]
public class Missile : SceneObj
{
    public int ownerId;
    [NonSerialized]
    public MissileViewObj viewObj;

    public int skillId;
    public int skillDamage;
    public int actionId;

    // 普通攻击伤害数据（skillId == 0时使用）
    public int attackDamage;
    public bool attackIsCrit;
    public bool attackIsDodge;
    public string attackDamType;

    // Movement state variables
    public enum MoveState { None, ToTarget, ToDirection }
    public MoveState moveState = MoveState.None;

    // ToTarget variables
    public int targetChessId;
    
    [NonSerialized]
    public string effectName;
    [NonSerialized]
    public string hitEffectName;
    [NonSerialized]
    private float size;    
    [NonSerialized]
    public float missileSpeed;
    [NonSerialized]
    public float maxY;
    [NonSerialized]
    public float detectArea;
    [NonSerialized]
    public int targetCount;
    [NonSerialized]
    public int forceId;
 
    // ToDirection variables
    public Vector3 direction;
    public Vector3 startPos;

    public float startTime; // 发射时刻(秒)
    public float travelSeconds; // 飞行时长(秒)
    public List<int> checkedIdList; //已结算单位id列表

    public Missile(int id, Chess sourceChess, Vector3 startPos, int skillId, int damage, int attackDamage = 0, bool attackIsCrit = false, bool attackIsDodge = false, string attackDamType = "str")
    {
        base.id = id;

        ownerId = sourceChess.id;
        this.startPos = startPos;
        position = startPos + new Vector3(0f, 6f, 0f);
        this.skillId = skillId;
        this.skillDamage = damage;
        this.attackDamage = attackDamage;
        this.attackIsCrit = attackIsCrit;
        this.attackIsDodge = attackIsDodge;
        this.attackDamType = attackDamType;
        forceId = sourceChess.forceId;

        // Reset state
        moveState = MoveState.None;
        targetChessId = 0;
        checkedIdList = new List<int>();
        startTime = BattleManager.Instance.battleTime;
    }

    public void Init()
    {
        CreateMissileView();
    }

    private void CreateMissileView()
    {
        if(skillId > 0)
        {
            var skillCfg = BattleSkillConfig.GetConfig(skillId);
            effectName = skillCfg.EffectHit;
            hitEffectName = effectName;
            missileSpeed = skillCfg.SummonSpeed;
            detectArea = skillCfg.SummonArea * 1.5f;
            targetCount = skillCfg.TargetCount;
            size = skillCfg.EffectSize;
        }
        else
        {
            var ownerChess = BattleManager.Instance.GetChess(ownerId);
            if (ownerChess == null)
            {
                GameLog.Error($"Missile owner chess not found: {ownerId}");
                return;
            }
            effectName = ownerChess.hitEffect;
            hitEffectName = ownerChess.hitEffect;
            missileSpeed = ownerChess.missileSpeed;
            maxY = ownerChess.missileHeight;
            size = 1;
        }
    
        if(!BattleManager.Instance.quickMode && BattleManager.Instance.showUI)
        {
            var missilePrefab = ResourceCache.LoadBattle<MissileViewObj>(ResPath.Prefab.MissileCom());
            viewObj = UnityEngine.Object.Instantiate(missilePrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);

            var effPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.MissileEffect(effectName));
            if (effPrefab == null)
                effPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Effect(effectName));
            if (effPrefab == null)
            {
                GameLog.Error($"Missile effect not found: {effectName}, skillId: {skillId}, using fallback");
                effPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.MissileDefaultEffect());
            }
            GameObject missileEffect = UnityEngine.Object.Instantiate(effPrefab, position, effPrefab.transform.rotation, viewObj.transform);
            viewObj.transform.position = position;
            missileEffect.transform.localScale = size * effPrefab.transform.localScale;

            var ownerChess = BattleManager.Instance.GetChess(ownerId);
            if (ownerChess != null && ownerChess.viewObj != null)
                viewObj.ownerName = ownerChess.viewObj.name;

            if (missileEffect.TryGetComponent(out MissileEffName missileViewObj))
                hitEffectName = missileViewObj.hitEffectName;            
        }
    }

    public override void OnRecover()
    {
        CreateMissileView();
    }
    
    public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        if(viewObj != null)
            viewObj.transform.position = pos;
    }

    public void MoveToTarget(Chess target)
    {
        if(viewObj != null && target != null && target.viewObj != null)
            viewObj.targetName = target.viewObj.name;

        // Initialize state for moving to target
        moveState = MoveState.ToTarget;
        targetChessId = target.id;
        
        // Calculate travel time based on distance and speed
        Vector3 targetPos = target.position + new Vector3(0f, 7f, 0f);
        float distance = Vector3.Distance(startPos, targetPos);
        
        // Ensure missileSpeed is not zero
        float speed = missileSpeed > 0 ? missileSpeed : 10f; // Default speed if not set
        travelSeconds = distance / speed;
        
        // Ensure minimum travel time to avoid division by zero
        if (travelSeconds <= 0f)
            travelSeconds = 0.1f;
    }

    public void MoveToDirection(Vector3 targetPos, float time)
    {
        // Initialize state for moving to direction
        moveState = MoveState.ToDirection;
        direction = (targetPos - position).normalized;
        direction.y = 0;
        travelSeconds = time;
        checkedIdList = new List<int>();
    }

    public override void RenderUpdate()
    {
        float battleTime = BattleManager.Instance.battleTime;
        switch (moveState)
        {
            case MoveState.ToTarget:
                UpdateMoveToTarget(battleTime);
                break;
            case MoveState.ToDirection:
                UpdateMoveToDirection(battleTime);
                break;
        }
    }
    
    public override void LogicUpdate()
    {
        var battleTime = BattleManager.Instance.battleTime;
        if(targetCount > 0 && checkedIdList.Count < targetCount)
        {
            var unitsInRange = BattleManager.Instance.GetUnitsInRange(position, Mathf.Max(1, Mathf.CeilToInt(detectArea)), forceId, true);
            unitsInRange.RemoveAll(x => checkedIdList.Contains(x.id) || x.hp <= 0); // Each unit only once
            if (unitsInRange.Count > 0)
            {
                if (unitsInRange.Count + checkedIdList.Count > targetCount)
                    BattleManager.RandomSelect(unitsInRange, targetCount - checkedIdList.Count);

                foreach (var unit in unitsInRange)
                {
                    checkedIdList.Add(unit.id);
                    OnCrash(unit);
                }
            }
        }
        else if(targetCount > 0 && checkedIdList.Count >= targetCount)
        {
            Cleanup();
            return;
        }

        if(targetChessId > 0)
        {
            var targetChess = BattleManager.Instance.GetChess(targetChessId);
            if (targetChess == null || targetChess.hp <= 0)
            {
                Cleanup();
                return;
            }
        }
        if ((battleTime - startTime) >= travelSeconds)
        {
            if(targetChessId > 0)
            {
                var targetChess = BattleManager.Instance.GetChess(targetChessId);
                if (targetChess != null)
                    OnCrash(targetChess);
            }
            Cleanup();
            return;
        }
       
    }

    private void UpdateMoveToTarget(float battleTime)
    {
        var targetChess = BattleManager.Instance.GetChess(targetChessId);
        if (targetChess == null)
            return;

        var targetPos = targetChess.position + new Vector3(0f, 7f, 0f); // 修正目标点

        // Calculate movement
        float fractionOfJourney = (battleTime - startTime) / travelSeconds;
        
        if (maxY > 0)
        {
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, fractionOfJourney);

            // Calculate parabola height
            float parabolaHeight = maxY * Mathf.Sin(fractionOfJourney * Mathf.PI);
            horizontalPos.y += parabolaHeight;

            SetPosition(horizontalPos);
            SetDirection(Quaternion.LookRotation(targetPos - position));
        }
        else
        {
            // Straight path
            SetPosition(Vector3.Lerp(startPos, targetPos, fractionOfJourney));
        }
    }

    private void UpdateMoveToDirection(float battleTime)
    {
        // Calculate movement distance based on speed and time
        // 保持原"单位/逻辑步"速度语义
        float moveDistance = missileSpeed * ((battleTime - startTime) / SystemConst.Battle.LOGIC_STEP);
        // Move in direction
        SetPosition(position + direction * moveDistance);
        SetDirection(Quaternion.LookRotation(direction));
    }

    private void Cleanup()
    {

        BattleManager.Instance.RemoveMissile(this);
        moveState = MoveState.None;
    }


    public void SetDirection(Quaternion dir)
    {
        if(viewObj != null)
            viewObj.transform.rotation = dir;
    }    

    private void OnCrash(Chess target)
    {
        var ownerChess = BattleManager.Instance.GetChess(ownerId);
        if (target == null || target.hp <= 0 || ownerChess == null || ownerChess.hp <= 0)
            return;

        GameLog.Info($"Missile.OnCrash[aid={actionId}] owner={ownerId} tgt={target.id} skill={skillId}");

        if (skillId == 0)
        {
            ownerChess.OnAttackDamage(target, attackDamage, attackIsCrit, attackIsDodge, hitEffectName, attackDamType, actionId);
        }
        else
        {
            target.DoSkillDamage(ownerChess, skillId, skillDamage, false, actionId);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}