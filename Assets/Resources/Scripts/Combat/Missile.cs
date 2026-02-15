using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

[Serializable]
public class Missile// : MonoBehaviour
{
    private int ownerId;
    public Chess owner{ get{ return BattleManager.Instance.GetChess(ownerId); } }
    [NonSerialized]
    public MissileViewObj viewObj;

    public string effectName;
    public string hitEffectName;

    private float size;

    public int skillId;
    public int skillDamage;

    public Vector3 position;

    // Movement state variables
    public enum MoveState { None, ToTarget, ToDirection }
    public MoveState moveState = MoveState.None;

    // ToTarget variables
    public int targetChessId;
    private Chess targetChess{ get{ return BattleManager.Instance.GetChess(targetChessId); } }
    public float missileSpeed;
    public float missileHight;
    public float journeyLength;
    public float totalLen;
    public float realLen;
    public float maxY;

    // ToDirection variables
    public Vector3 direction;
    public float timeLimit;
    public float detectArea;
    public int targetCount;
    public float liveTick;
    public float lastCheckTime;
    public List<int> checkedIdList; //已结算id列表

    public void Init(Chess sourceChess, Vector3 startPos, float size, string effectName)
    {
        this.effectName = effectName;
        hitEffectName = effectName;
        ownerId = sourceChess.id;
        this.size = size;
        position = startPos + new Vector3(0f, 2f, 0f);

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
            viewObj.ownerName = owner.viewObj.name;

            if (missileEffect.TryGetComponent(out MissileEffName missileViewObj))
                hitEffectName = missileViewObj.hitEffectName;            
        }

        // Reset state
        moveState = MoveState.None;
        targetChessId = 0;
        checkedIdList = new List<int>();
    }

    public void SetSkillInfo(int skillId, int damage)
    {
        this.skillId = skillId;        
        skillDamage = damage;
    }

    public void MoveToTarget(Chess target, float missileSpeed, float missileHight)
    {
        if(viewObj != null && target != null && target.viewObj != null)
            viewObj.targetName = target.viewObj.name;

        // Initialize state for moving to target
        moveState = MoveState.ToTarget;
        targetChessId = target.id;
        this.missileSpeed = missileSpeed;
        this.missileHight = missileHight;
        var targetPos = target.position;
        journeyLength = BattleManager.Instance.GetRange(position, targetPos);
        totalLen = journeyLength;
        realLen = 0;
        maxY = missileHight;
    }

    public void MoveToDirection(Vector3 targetPos, float time, float missileSpeed)
    {
        var detectArea = 10f;
        var targetCount = 1;
        if (skillId > 0)
        {
            var skillCfg = SkillConfig.GetConfig(skillId);
            detectArea = skillCfg.SummonArea * 1.5f;
            targetCount = skillCfg.TargetCount;
        }

        // Initialize state for moving to direction
        moveState = MoveState.ToDirection;
        direction = (targetPos - position).normalized;
        direction.y = 0;
        timeLimit = time;
        this.missileSpeed = missileSpeed;
        this.detectArea = detectArea;
        this.targetCount = targetCount;
        liveTick = 0;
        checkedIdList = new List<int>();
    }

    public void LogicUpdate(int tickIndex, float indexMini, float timeElapsed)
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

    private void UpdateMoveToTarget(float tickTimeReal, float timeElapsed)
    {
        if (targetChess == null || targetChess.hp <= 0)
        {
            Cleanup();
            return;
        }

        var targetPos = targetChess.position + new Vector3(0f, 3f, 0f); // 修正目标点
        if (BattleManager.Instance.CheckInRange(position, targetPos, 0.5f))
        {
            OnCrash(targetChess, (int)Math.Floor(tickTimeReal));
            Cleanup();
            return;
        }

        // Calculate movement
        float distCovered = timeElapsed * missileSpeed;
        journeyLength = BattleManager.Instance.GetRange(position, targetPos);
        float fractionOfJourney = distCovered / journeyLength;
        
        if (maxY > 0)
        {
            Vector3 horizontalPos = Vector3.Lerp(position, targetPos, fractionOfJourney);
            realLen += distCovered * 1.1f;
            if(realLen > totalLen)
                realLen = totalLen;

            // Calculate parabola height
            float parabolaHeight = maxY * Mathf.Sin((realLen / totalLen) * Mathf.PI);
            horizontalPos.y += parabolaHeight;

            SetPosition(horizontalPos);
            SetDirection(Quaternion.LookRotation(targetPos - position));
        }
        else
        {
            // Straight path
            SetPosition(Vector3.Lerp(position, targetPos, fractionOfJourney));
        }
    }

    private void UpdateMoveToDirection(float tickTimeReal, float timeElapsed)
    {
        // Calculate movement distance based on speed and time
        float moveDistance = missileSpeed * timeElapsed;
        // Move in direction
        SetPosition(position + direction * moveDistance);
        SetDirection(Quaternion.LookRotation(direction));

        // Check for targets in range
        if (tickTimeReal - lastCheckTime >= 0.2f)
        {
            var unitsInRange = BattleManager.Instance.GetUnitsInRange(position, detectArea, owner.forceId, true);
            unitsInRange.RemoveAll(x => checkedIdList.Contains(x.id) || x.hp <= 0); // Each unit only once
            if (unitsInRange.Count > 0)
            {
                if (unitsInRange.Count + checkedIdList.Count > targetCount)
                    BattleManager.Instance.RandomSelect(unitsInRange, targetCount - checkedIdList.Count);

                foreach (var unit in unitsInRange)
                {
                    checkedIdList.Add(unit.id);
                    OnCrash(unit, (int)Math.Floor(tickTimeReal));
                }
            }

            lastCheckTime = tickTimeReal;
        }

        liveTick += timeElapsed;
        if (liveTick >= timeLimit || checkedIdList.Count >= targetCount)
        {
            Cleanup();
            return;
        }
    }

    private void Cleanup()
    {
        if (viewObj != null)
        {
            UnityEngine.Object.Destroy(viewObj.gameObject);
        }
        BattleManager.Instance.RemoveMissile(this);
        moveState = MoveState.None;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
        if(viewObj != null)
            viewObj.transform.position = pos;
    }

    public void SetDirection(Quaternion dir)
    {
        if(viewObj != null)
            viewObj.transform.rotation = dir;
    }    

    private void OnCrash(Chess target, int tickIndex)
    {
        if (target == null || target.hp <= 0 || owner == null || owner.hp <= 0)
            return;

        if (skillId == 0)
        {
            owner.Attack(target, hitEffectName, tickIndex);
        }
        else
        {
            target.OnSkillDamaged(owner, skillId, skillDamage);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}