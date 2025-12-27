using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;


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

    public void Init(Chess sourceChess, float size, string effectName)
    {
        this.effectName = effectName;
        hitEffectName = effectName;
        owner = sourceChess;
        this.size = size;
    }

    public void SetSkillInfo(int skillId, int damage)
    {
        this.skillId = skillId;        
        skillDamage = damage;
    }

    public void MoveToDirection(Vector3 targetPos, float time, float missileSpeed)
    {
        if (viewObj != null)
        {
            var missilePrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
            if (missilePrefab == null)
                missilePrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);
            GameObject missileEffect = UnityEngine.Object.Instantiate(missilePrefab, position, missilePrefab.transform.rotation, viewObj.transform);
            viewObj.transform.rotation = Quaternion.LookRotation(targetPos - position);
            viewObj.transform.position += new Vector3(0f, 2f, 0f);
            viewObj.transform.localScale = size * missilePrefab.transform.localScale;   
            position += new Vector3(0f, 2f, 0f);

            if (missileEffect.TryGetComponent(out MissileViewObj missileViewObj))
                hitEffectName = missileViewObj.hitEffectName;
        }

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

    public void MoveToTarget(Chess target, float missileSpeed, float missileHight)
    {
        if (viewObj != null)
        {
            var missilePrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
            if (missilePrefab == null)
                missilePrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);

            GameObject missileEffect = UnityEngine.Object.Instantiate(missilePrefab, position, Quaternion.identity, viewObj.transform);
            viewObj.transform.position += new Vector3(0f, 5f, 0f);
            position += new Vector3(0f, 5f, 0f);
            missileEffect.transform.localScale = missilePrefab.transform.localScale;

            if (missileEffect.TryGetComponent(out MissileViewObj missileViewObj))
                hitEffectName = missileViewObj.hitEffectName;
        }

        BattleManager.Instance.StartNLCoroutine(MoveMissileToTarget(target, missileSpeed, missileHight, BattleManager.tickTime));
    }


    // 定义协程方法，控制导弹移动
    IEnumerator MoveMissileToTarget( Chess target, float missileSpeed, float missileHight, float tickTime)
    {
        var targetPos = target.position + new Vector3(0f, 5f, 0f);

        float journeyLength = BattleManager.Instance.GetRange(position, targetPos);
        float totalLen = journeyLength;
        float realLen = 0;
        float startTime = BattleManager.Instance.time;
        
        float speed = missileSpeed * 2.5f; // 导弹移动速度

        float maxY = missileHight;

        var lastTime = BattleManager.Instance.time;
        while (!BattleManager.Instance.CheckInRange(position, targetPos, 0.5f))
        {
            // if (owner == null || owner.hp <= 0)
            // {
            //     Destroy(missile);
            //     yield break;
            // }
            if(target != null && target.hp > 0)
                targetPos = target.position + new Vector3(0f, 5f, 0f); //修正目标点
            float distCovered = (BattleManager.Instance.time - lastTime) * speed;
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
            lastTime = BattleManager.Instance.time;
            yield return new NLWaitForSeconds(tickTime);
        }

        OnCrash(target);
        if (viewObj != null)
        {
            UnityEngine.Object.Destroy(viewObj.gameObject);
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

 // 让hitEffect飞向targetPos的协程
    IEnumerator MoveMissileToDirection(Vector3 direction, float time, float speed, float detectArea, int targetCount, float tickTime)
    {
        Vector3 currentPos = position;
        direction.y = 0;
        float timePast = 0;
        float lastCheckTime = 0.2f;
        var checkedList = new List<Chess>();

        while (true)
        {
            // if (owner == null || owner.hp <= 0)
            //     yield break;

            // 计算本次移动的距离（基于速度和时间）
            float moveDistance = speed * 0.025f;

            // 按方向和距离移动 
            currentPos = position = currentPos + direction * moveDistance;

            if (timePast - lastCheckTime >= 0.2f)
            {
                var unitsInRange = BattleManager.Instance.GetUnitsInRange(currentPos, detectArea, owner.side, true);
                unitsInRange.RemoveAll(x => checkedList.Contains(x) || x.hp <= 0); //每个单位结算一次
                if (unitsInRange.Count > 0)
                {
                    if (unitsInRange.Count + checkedList.Count > targetCount)
                        BattleManager.Instance.RandomSelect(unitsInRange, targetCount - unitsInRange.Count - checkedList.Count);

                    foreach (var unit in unitsInRange)
                    {
                        checkedList.Add(unit);
                        OnCrash(unit);
                    }
                }

                lastCheckTime = timePast;
            }

            timePast += tickTime;
            if (timePast >= time || checkedList.Count >= targetCount)
            {
                if (viewObj != null)
                {
                    UnityEngine.Object.Destroy(viewObj.gameObject);
                }
                yield break;
            }

            yield return new NLWaitForSeconds(tickTime);
        }


    }

    private void OnCrash(Chess target)
    {
        if (target == null || target.hp <= 0 || owner == null || owner.hp <= 0)
            return;

        if (skillId == 0)
        {
            owner.Attack(target, hitEffectName);
        }
        else
        {
            target.OnSkillDamaged(owner, skillId, skillDamage);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}