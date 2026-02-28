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
 
    // ToDirection variables
    public Vector3 direction;
    public Vector3 startPos;

    public int tickTotal;
    public float tickPast;
    public List<int> checkedIdList; //已结算单位id列表

    public Missile(int id, Chess sourceChess, Vector3 startPos, int skillId, int damage)
    {
        base.id = id;

        ownerId = sourceChess.id;
        this.startPos = startPos;
        position = startPos + new Vector3(0f, 2f, 0f);
        this.skillId = skillId;
        this.skillDamage = damage;

        // Reset state
        moveState = MoveState.None;
        targetChessId = 0;
        checkedIdList = new List<int>();
    }

    public void Init()
    {
        CreateMissileView();
    }

    private void CreateMissileView()
    {
        if(skillId > 0)
        {
            var skillCfg = SkillConfig.GetConfig(skillId);
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
            effectName = ownerChess.hitEffect;
            hitEffectName = ownerChess.hitEffect;
            missileSpeed = ownerChess.missileSpeed;
            maxY = ownerChess.missileHeight;
            size = 1;
        }
    
        if(!BattleManager.Instance.quickMode && BattleManager.Instance.showUI)
        {
            var missilePrefab = Resources.Load<MissileViewObj>("Prefabs/MissileCom");
            viewObj = UnityEngine.Object.Instantiate(missilePrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);

            var effPrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
            if (effPrefab == null)
                effPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);
            GameObject missileEffect = UnityEngine.Object.Instantiate(effPrefab, position, effPrefab.transform.rotation, viewObj.transform);
            viewObj.transform.position = position;
            missileEffect.transform.localScale = size * effPrefab.transform.localScale;

            var ownerChess = BattleManager.Instance.GetChess(ownerId);
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
        Vector3 targetPos = target.position + new Vector3(0f, 3f, 0f);
        float distance = Vector3.Distance(startPos, targetPos);
        
        // Ensure missileSpeed is not zero
        float speed = missileSpeed > 0 ? missileSpeed : 10f; // Default speed if not set
        tickTotal = BattleManager.Instance.GetTickFromTime(distance / speed);
        
        // Ensure minimum travel time to avoid division by zero
        if (tickTotal <= 0)
            tickTotal = 1;
    }

    public void MoveToDirection(Vector3 targetPos, float time)
    {
        // Initialize state for moving to direction
        moveState = MoveState.ToDirection;
        direction = (targetPos - position).normalized;
        direction.y = 0;
        tickTotal = BattleManager.Instance.GetTickFromTime(time);
        checkedIdList = new List<int>();
    }

    public override void FixUpdate(int tickIndex, float indexMini, float timeElapsed)
    {
        float tickReal = tickIndex + indexMini;
        switch (moveState)
        {
            case MoveState.ToTarget:
                UpdateMoveToTarget(tickReal, timeElapsed);
                break;
            case MoveState.ToDirection:
                UpdateMoveToDirection(tickReal, timeElapsed);
                break;
        }
    }
    
    public override void LogicUpdate(int tickIndex)
    {
        
    }

    private void UpdateMoveToTarget(float tickTimeReal, float timeElapsed)
    {
        var targetChess = BattleManager.Instance.GetChess(targetChessId);
        if (targetChess == null || targetChess.hp <= 0)
        {
            Cleanup();
            return;
        }

        var targetPos = targetChess.position + new Vector3(0f, 3f, 0f); // 修正目标点

        // Calculate movement
        float fractionOfJourney = tickPast / tickTotal;
        
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

        tickPast += timeElapsed;
        if (tickPast >= tickTotal)
        {
            OnCrash(targetChess, (int)Math.Floor(tickTimeReal));
            Cleanup();
            return;
        }
    }

    private void UpdateMoveToDirection(float tickTimeReal, float timeElapsed)
    {
        // Calculate movement distance based on speed and time
        float moveDistance = missileSpeed * tickPast;
        // Move in direction
        SetPosition(position + direction * moveDistance);
        SetDirection(Quaternion.LookRotation(direction));

        // Check for targets in range
        {
            var ownerChess = BattleManager.Instance.GetChess(ownerId);
            var unitsInRange = BattleManager.Instance.GetUnitsInRange(position, detectArea, ownerChess.forceId, true);
            unitsInRange.RemoveAll(x => checkedIdList.Contains(x.id) || x.hp <= 0); // Each unit only once
            if (unitsInRange.Count > 0)
            {
                if (unitsInRange.Count + checkedIdList.Count > targetCount)
                    BattleManager.RandomSelect(unitsInRange, targetCount - checkedIdList.Count);

                foreach (var unit in unitsInRange)
                {
                    checkedIdList.Add(unit.id);
                    OnCrash(unit, (int)Math.Floor(tickTimeReal));
                }
            }
        }

        tickPast += timeElapsed;
        if (tickPast >= tickTotal || checkedIdList.Count >= targetCount)
        {
            Cleanup();
            return;
        }
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

    private void OnCrash(Chess target, int tickIndex)
    {
        var ownerChess = BattleManager.Instance.GetChess(ownerId);
        if (target == null || target.hp <= 0 || ownerChess == null || ownerChess.hp <= 0)
            return;

        if (skillId == 0)
        {
            ownerChess.Attack(target, hitEffectName, tickIndex);
        }
        else
        {
            target.DoSkillDamage(ownerChess, skillId, skillDamage);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}