using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class Missile// : MonoBehaviour
{
    public Chess owner;
    public MissileViewObj viewObj;

    public string effectName;
    private string hitEffectName;

    private float size;

    public int skillId;
    public int skillDamage;

    public Vector3 position;

    public void Init(Chess sourceChess, Vector3 startPos, float size, string effectName)
    {
        this.effectName = effectName;
        hitEffectName = effectName;
        owner = sourceChess;
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
            //viewObj.transform.rotation = Quaternion.LookRotation(targetPos - position);
            viewObj.transform.position = position;
            missileEffect.transform.localScale = size * effPrefab.transform.localScale;   
            viewObj.ownerName = owner.viewObj.name;

            if (missileEffect.TryGetComponent(out MissileEffName missileViewObj))
                hitEffectName = missileViewObj.hitEffectName;            
        }        
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
        BattleManager.Instance.StartNLCoroutine(MoveMissileToTarget(target, missileSpeed, missileHight, BattleManager.tickTime));
    }

    // 定义协程方法，控制导弹移动
    IEnumerator MoveMissileToTarget( Chess target, float missileSpeed, float missileHight, float tickTime)
    {
        var targetPos = target.position;

        float journeyLength = BattleManager.Instance.GetRange(position, targetPos);
        float totalLen = journeyLength;
        float realLen = 0;
        
        float speed = missileSpeed; // 导弹移动速度

        float maxY = missileHight;
        
        var lastTime = BattleManager.Instance.tickIndex;
        while (!BattleManager.Instance.CheckInRange(position, targetPos, 0.5f))
        {
            // if (owner == null || owner.hp <= 0)
            // {
            //     Destroy(missile);
            //     yield break;
            // }
            if(target != null && target.hp > 0)
                targetPos = target.position + new Vector3(0f, 3f, 0f); //修正目标点
            float distCovered = (BattleManager.Instance.tickIndex - lastTime) * speed;
            journeyLength = BattleManager.Instance.GetRange(position, targetPos);
            float fractionOfJourney = distCovered / journeyLength;
            
            if (maxY > 0)
            {
                Vector3 horizontalPos = Vector3.Lerp(position, targetPos, fractionOfJourney);

                // UnityEngine.Debug.Log("fractionOfJourney: " + fractionOfJourney);
                realLen += distCovered * 1.1f;
                if(realLen > totalLen)
                    realLen = totalLen;

                // 计算抛物线高度
                float parabolaHeight = maxY * Mathf.Sin((realLen / totalLen) * Mathf.PI);
                horizontalPos.y += parabolaHeight;

                SetPosition(horizontalPos);
                SetDirection(Quaternion.LookRotation(targetPos - position));
            }
            else
            {
                // 直线路径
                SetPosition(Vector3.Lerp(position, targetPos, fractionOfJourney));
            }
            lastTime = BattleManager.Instance.tickIndex;
            yield return new NLWaitForSeconds(tickTime);
        }

        OnCrash(target);
        if (viewObj != null)
        {
            UnityEngine.Object.Destroy(viewObj.gameObject);
        }
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

        BattleManager.Instance.StartNLCoroutine(MoveMissileToDirection((targetPos - position).normalized, time, missileSpeed, detectArea, targetCount, BattleManager.tickTime));
    }    

 // 让hitEffect飞向targetPos的协程
    IEnumerator MoveMissileToDirection(Vector3 direction, float time, float speed, float detectArea, int targetCount, float tickTime)
    {
        direction.y = 0;
        float timePast = 0;
        float lastCheckTime = 0.2f;
        var checkedList = new List<Chess>();

        var lastTime = BattleManager.Instance.tickIndex;
        while (true)
        {
            // if (owner == null || owner.hp <= 0)
            //     yield break;

            // 计算本次移动的距离（基于速度和时间）
            var timeElapsed = BattleManager.Instance.tickIndex - lastTime;
            float moveDistance = speed * timeElapsed;
            // 按方向和距离移动 
            SetPosition(position + direction * moveDistance);
            SetDirection(Quaternion.LookRotation(direction));

            if (timePast - lastCheckTime >= 0.2f)
            {
                var unitsInRange = BattleManager.Instance.GetUnitsInRange(position, detectArea, owner.side, true);
                unitsInRange.RemoveAll(x => checkedList.Contains(x) || x.hp <= 0); //每个单位结算一次
                if (unitsInRange.Count > 0)
                {
                    if (unitsInRange.Count + checkedList.Count > targetCount)
                        BattleManager.Instance.RandomSelect(unitsInRange, targetCount - checkedList.Count);

                    foreach (var unit in unitsInRange)
                    {
                        checkedList.Add(unit);
                        OnCrash(unit);
                    }
                }

                lastCheckTime = timePast;
            }

            timePast += timeElapsed;
            if (timePast >= time || checkedList.Count >= targetCount)
            {
                if (viewObj != null)
                {
                    UnityEngine.Object.Destroy(viewObj.gameObject);
                }
                yield break;
            }

            lastTime = BattleManager.Instance.tickIndex; 
            yield return new NLWaitForSeconds(tickTime);
        }

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

    private void OnCrash(Chess target)
    {
        if (target == null || target.hp <= 0 || owner == null || owner.hp <= 0)
            return;

        if (skillId == 0)
        {
            owner.Attack(target, hitEffectName, BattleManager.Instance.tickIndex);
        }
        else
        {
            target.OnSkillDamaged(owner, skillId, skillDamage);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}