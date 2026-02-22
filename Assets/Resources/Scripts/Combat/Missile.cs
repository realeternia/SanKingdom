using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

[Serializable]
public class Missile : SceneObj
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

    // Movement state variables
    public enum MoveState { None, ToTarget, ToDirection }
    public MoveState moveState = MoveState.None;

    // ToTarget variables
    public int targetChessId;
    private Chess targetChess{ get{ return BattleManager.Instance.GetChess(targetChessId); } }
    public float missileSpeed;
    public float maxY;

    // ToDirection variables
    public Vector3 direction;
    public Vector3 startPos;
    public float detectArea;
    public int targetCount;
    public float tickTimeTotal;
    public float liveTime;
    public float lastCheckTick;
    public List<int> checkedIdList; //已结算单位id列表

    public Missile(int id, Chess sourceChess, Vector3 startPos, float size, string effectName, int skillId, int damage)
    {
        base.id = id;
        this.effectName = effectName;
        hitEffectName = effectName;
        ownerId = sourceChess.id;
        this.size = size;
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

    public void MoveToTarget(Chess target, float missileSpeed, float missileHight)
    {
        if(viewObj != null && target != null && target.viewObj != null)
            viewObj.targetName = target.viewObj.name;

        // Initialize state for moving to target
        moveState = MoveState.ToTarget;
        targetChessId = target.id;
        this.missileSpeed = missileSpeed;
        maxY = missileHight;
        tickTimeTotal = liveTime / tickTimeTotal;
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
        this.missileSpeed = missileSpeed;
        this.detectArea = detectArea;
        this.targetCount = targetCount;
        tickTimeTotal = time;
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

    private void UpdateMoveToTarget(float tickTimeReal, float timeElapsed)
    {
        if (targetChess == null || targetChess.hp <= 0)
        {
            Cleanup();
            return;
        }

        var targetPos = targetChess.position + new Vector3(0f, 3f, 0f); // 修正目标点

        // Calculate movement
        float fractionOfJourney = liveTime / tickTimeTotal;
        
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

        liveTime += timeElapsed;
        if (liveTime >= tickTimeTotal)
        {
            OnCrash(targetChess, (int)Math.Floor(tickTimeReal));
            Cleanup();
            return;
        }
    }

    private void UpdateMoveToDirection(float tickTimeReal, float timeElapsed)
    {
        // Calculate movement distance based on speed and time
        float moveDistance = missileSpeed * liveTime;
        // Move in direction
        SetPosition(position + direction * moveDistance);
        SetDirection(Quaternion.LookRotation(direction));

        // Check for targets in range
        if (tickTimeReal - lastCheckTick >= 0.2f)
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

            lastCheckTick = tickTimeReal;
        }

        liveTime += timeElapsed;
        if (liveTime >= tickTimeTotal || checkedIdList.Count >= targetCount)
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