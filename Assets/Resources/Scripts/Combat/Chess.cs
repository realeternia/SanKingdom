using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chess
{
    public ChessViewObj viewObj;
    public int id;
    public int forceId;

    public int maxHp = 100;  // 最大生命值

    public bool isHero;
    public int heroId;
    public string chessName = "0";

    public Vector3 position{ get; private set; }

    // 目标单位
    public Chess targetChess;
    // 移动速度
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public int inte;
    public int str;
    public int leadShip;
    public int level = 1;
    public bool isShadow;
    public bool isFakeHero;
    public float dodgeRate; //闪避
    public float critRate; //暴击
    public float critDamageMulti = 0.5f; //暴击伤害倍率

    public int lastDamagedPlayerId = -1;

    // 是否正在使用偏移路径
    public int hp = 100;
    public int attackDamage = 30;
    public string hitEffect;
    public int missileSpeed = 10;
    public float missileHight;
    public int soldierId;
    private int soldierLevel = 0;


    // 攻击冷却时间
    public int attackPoint;
    public int attackRate; //攻击频率
    private int lastAttackTime = 0;
    private int lastTargetUpdateTick; // 上次更新目标的时间

    public HeroInfo heroInfo;

    public List<Skill> skills = new List<Skill>();

    public List<Buff> buffs = new List<Buff>();
    public List<BuffTime> buffTimes = new List<BuffTime>(); //记录最近20s的buff记录
    public int noMoveCount = 0;
    public int noActionCount = 0;

    private bool dieAfterLifeTime;
    private int lifeTickCount; //1s死亡一次

    private int regeTickCount; //1s回复一次
    public int regeHp; //回复血量

    private List<ChessAction> actions = new List<ChessAction>();

    public void Init(int forceId, Color c)
    {
        this.forceId = forceId;

        hp = maxHp;
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        
        attackPoint = UnityEngine.Random.Range(1, 10); // 随机获得初始气力
        attackRate = 1;

        if (isHero)
        {
            Debug.Log("Init Hero" + heroId);

            var heroCfg = HeroConfig.GetConfig(heroId);
            // 初始化技能
            if (heroCfg.Skills != null)
            {
                foreach (var skillId in heroCfg.Skills)
                    skills.Add(SkillManager.CreateSkill(skillId, this));
            }
        }
        if(viewObj != null)
            viewObj.Init(this, c);
    }

    public void LogicUpdate(int tickIndex)
    {
        if (hp <= 0)
            return;

        buffs.Where(x => BattleManager.Instance.tickIndex > x.endTime).ToList().ForEach(x => BuffManager.RemoveBuff(this, x.id));

        if(regeHp > 0)
        {
            regeTickCount ++;
            if(regeTickCount >= 10)
            {
                regeTickCount -= 10;
                AddHp(regeHp);
            }
        }

        MoveAndFight(tickIndex);

        if (dieAfterLifeTime)
        {
            lifeTickCount --;
            if (lifeTickCount <= 0)
            {
                Ondying();
            }
        }
    }


    void Update()
    {

    }

    public void CheckInitAttr(int lv, int soldierNum)
    {
        level = lv;

        var heroConfig = HeroConfig.GetConfig(heroId);
        var attr = HeroSelectionTool.GetCardAttr(heroId, lv);

        maxHp = soldierNum;
        moveSpeed = heroConfig.MoveSpeed;
        attackRange = heroConfig.Range;
        attackDamage = attr.Lead / 3;
        inte = attr.Inte;
        str = attr.Str;
        leadShip = attr.Lead;

        hp = maxHp;

        if (heroInfo != null)
            heroInfo.SetAttr(inte, str, leadShip);
    }

    // 只能开场用
    public void AddSoldierLevel(int lv, int atkAdd, int hpAdd)
    {
        if (isHero)
            return;

        var soldierCfg = SoldierConfig.GetConfig(soldierId);
        if (soldierCfg.SoldierAtkRate <= 0)
            return;

        //根据level变化模型scale
        soldierLevel += lv;
        if(viewObj != null)
            viewObj.transform.localScale = new Vector3(5 + soldierLevel * 0.75f, 3, 5 + soldierLevel * 0.75f);

        attackDamage += (int)(lv * atkAdd * soldierCfg.SoldierAtkRate);
        maxHp += (int)(lv * hpAdd * soldierCfg.SoldierHpRate);
        hp = maxHp;
    }

    public void SetPosition(Vector3 position)
    {
        position.y = 7 + id * 0.01f;
        this.position = position;
        if(viewObj != null)
            viewObj.transform.position = position;
    }

    public void LockTarget(Chess target1)
    {
        targetChess = target1;
        // lastTargetUpdateTick = BattleManager.Instance.time;
    }

    private int lackIndex;
    public void LackFood(float lackRate)
    {
        hp = Math.Max(1, hp - (int)((15 + lackIndex * 5) * lackRate)); //饿不死人
        lackIndex++;
    }


    // 寻找side不等于自己的单位
    public void FindTarget()
    {
        if (attackRange == 0)
            return;

        // 获取所有Chess组件
        var allChess = BattleManager.Instance.GetUnitsInRange(position, 0, forceId, true);
        List<(Chess chess, float distance)> validTargets = new List<(Chess, float)>();

        // 收集所有有效目标及其距离
        foreach (Chess chess in allChess)
        {
            if (chess != this)
            {
                float distance = BattleManager.Instance.GetRange(position, chess.position);
                validTargets.Add((chess, distance));
            }
        }

        // 如果没有有效目标，直接返回
        if (validTargets.Count == 0)
        {
            targetChess = null;
            return;
        }

        // 按距离排序
        validTargets.Sort((a, b) => a.distance.CompareTo(b.distance));

        // 获取最近单位的距离
        float nearestDistance = validTargets[0].distance;
        List<(Chess chess, float distance)> filteredTargets = null;
        if(nearestDistance <= attackRange)
            filteredTargets = validTargets.Where(t => t.distance <= attackRange).ToList(); //如果有射程内的，就继续找一个射程内的
        else
            filteredTargets = validTargets.Where(t => t.distance <= nearestDistance + 10f).ToList();

        // 如果筛选后不足3个，则取全部
        int takeCount = Mathf.Min(3, filteredTargets.Count);
        List<(Chess chess, float distance)> topTargets = filteredTargets.Take(takeCount).ToList();

        // 对目标进行打分
        List<(Chess chess, float score)> scoredTargets = new List<(Chess, float)>();
        foreach (var (chess, distance) in topTargets)
        {
            float score = CalculateTargetScore(chess, distance);
            scoredTargets.Add((chess, score));
        }

        // 按分数降序排序
        scoredTargets.Sort((a, b) => b.score.CompareTo(a.score));

        // 选择分数最高的作为目标
        targetChess = scoredTargets[0].chess;
        if(viewObj != null)
            viewObj.lockTargetId = targetChess.id;
    }

    // 计算目标分数
    private float CalculateTargetScore(Chess target, float distance)
    {
        float score = target.isHero ? 10 : 30;

        // 添加最大属性差作为积分项（权重可根据游戏平衡调整）
        if (distance < attackRange * 2)
        {
            score += 30 * UnityEngine.Random.value;

            score += CalculateDamage(this, target, out var type) / 2;
            score += (level - target.level) * 7f;

            // 生命值权重（生命值越低分数越高）
            var targetHpRate = (float)target.hp / target.maxHp;
            if (targetHpRate < 0.5f)
                score += (0.5f - targetHpRate) * 100f + 10;
        }
        else
        {
             score += 100f / (distance + 1f);  // 避免除以0
        }

        return score;
    }

    private void MoveAndFight(int tickIndex)
    {
        if (noActionCount > 0)
            return;

        // 每3秒重新寻找目标
        if (tickIndex - lastTargetUpdateTick >= 30)
        {
            FindTarget();
            lastTargetUpdateTick = tickIndex;
        }

        // 检查目标是否存在
        if (targetChess == null || targetChess.hp <= 0)
        {
            // 如果没有目标，尝试寻找新目标
            FindTarget();

            if (targetChess == null)
                return;
        }

        // 检查是否有辅助技能
        if (SkillManager.CheckAidSkill(this, tickIndex))
            return;

        // 检查目标是否在攻击范围内
        if (BattleManager.Instance.CheckInRange(position, targetChess.position, attackRange))
        {
            attackPoint += attackRate;
            // 检查攻击冷却
            if (attackPoint >= 20) //集气2s
            {
                attackPoint -= 20;
                SkillManager.AimTarget(this, targetChess);
                if (attackRange >= 20)
                {
                    BattleManager.Instance.CreateAttackMissile(this, targetChess, hitEffect);
                }
                else
                {
                    Attack(targetChess, hitEffect, tickIndex); // 普通攻击
                }
            }
            lastAttackTime = tickIndex;
            return;
        }

        if (noMoveCount > 0 || moveSpeed == 0)
            return;

        var moveDest = GetMoveDest();
        if (moveDest != Vector3.zero)
        {
            attackPoint += attackRate;
            if (attackPoint >= 10)
            {
                attackPoint -= 10;

                // 创建移动Action并添加到actions列表
                var moveAction = new MoveAction
                {
                    SourceId = id,
                    Tick = tickIndex,
                    TargetId = targetChess != null ? targetChess.id : -1,
                    TargetPosition = moveDest
                };
                actions.Add(moveAction);

                BattleManager.Instance.MoveTo(this, moveDest, true);
            }
        }
    }

    private Vector3 GetMoveDest()
    {
        int moveFailCount = 0;
        var moveDis = moveSpeed * 0.5f;

        for (int i = 0; i < 4; i++)
        {
            Vector3 nextPosition = Vector3.MoveTowards(position, targetChess.position, moveDis * (4 - i) / 4); //尝试短距离移动
            if (BattleManager.Instance.IsPositionFree(this, nextPosition))
                return nextPosition;
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 nextPosition = Vector3.MoveTowards(position, targetChess.position, moveDis * (i + 1) / 4); //尝试长距离移动
            if (BattleManager.Instance.IsPositionFree(this, nextPosition))
                return nextPosition;
        }

        if (moveFailCount == 0)
        {
            // 根据连续失败次数尝试不同角度找路
            // 计算原始方向
            Vector3 direction = (targetChess.position - position).normalized;
            float angleOffset;

            // 根据失败次数确定偏移角度
            if (moveFailCount <= 3)
                angleOffset = 45f;
            else if (moveFailCount <= 5)
                angleOffset = 70f;
            else
                angleOffset = 90f;

            // 随机选择向上或向下偏移
            angleOffset *= UnityEngine.Random.value > 0.5f ? 1 : -1;

            // 计算旋转后的方向
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 newDirection = rotation * direction;

            for (int i = 0; i < 4; i++)
            {
                var nextPosition = position + newDirection * moveDis * (4 - i) / 4;
                if (BattleManager.Instance.IsPositionFree(this, nextPosition))
                    return nextPosition;
            }
        }
        return Vector3.zero;
    }    

    // 攻击目标
    public void Attack(Chess victim, string hitEffectName, int tickIndex)
    {
        if (victim == null)
            return;

        // 造成伤害
        var damage = CalculateDamage(this, victim, out var damType);
        var effect = hitEffectName;
        var damageBase = damage;
        var damageMulti = 1f;
        var damageReal = 0; //真实伤害
        bool isCrit = false;

        SkillManager.DuringAttack(this, victim, damType, ref damageBase, ref damageMulti, ref damageReal, ref effect);
        // 暴击
        if (critRate > 0 && UnityEngine.Random.value < critRate)
        {
            damageMulti += critDamageMulti;
            BattleManager.Instance.AddBattleText("暴!", position, new UnityEngine.Vector2(0, 40), Color.red, 3);
            isCrit = true;
        }

        damage = (int)(damageBase * damageMulti);
        var minDamage = 10 + level / 2;
        var maxDamage = 50 + level;
        if (isHero && victim.isHero)
        {
            //等级压制
            var levelDiff = level - victim.level;
            if (levelDiff != 0)
            {
                minDamage = Math.Clamp(minDamage + levelDiff, 8, minDamage * 2);
                maxDamage = Math.Clamp(maxDamage + levelDiff * 4, 40, maxDamage * 2);
            }

            var attackJobCfg = ConfigManager.GetJobConfig(HeroConfig.GetConfig(heroId).Job);
            var victimJob = ConfigManager.GetJobConfig(HeroConfig.GetConfig(victim.heroId).Job).NameS;
            if (attackJobCfg.OvercomeStrong != null && attackJobCfg.OvercomeStrong.Contains(victimJob))
                damage = Math.Max(damage + 15, minDamage / 2 + 7);
            else if (attackJobCfg.OvercomeWeak != null && attackJobCfg.OvercomeWeak.Contains(victimJob))
                damage = Math.Max(damage + 8, minDamage / 2 + 4);
        }
        if(isCrit)
        {
            minDamage = (int)(minDamage * (1 + critDamageMulti));
            maxDamage = (int)(maxDamage * (1 + critDamageMulti));
        }
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        if (damage > 0)
        {
            if (victim.dodgeRate > 0 && UnityEngine.Random.value < victim.dodgeRate)
            {
                damage = 0;
                BattleManager.Instance.AddBattleText("闪!", victim.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
            }
            else
            {
                //这里不改数值，只能伤害吸收
                SkillManager.BeforeAttack(this, victim, ref damage);
            }
        }

        if (damage + damageReal > 0)
        {
            damage = Math.Max(damage, damageReal);

            victim.hp -= damage;
            if (victim != this)
                victim.lastDamagedPlayerId = forceId;
            // 记录战斗统计
            if (isHero)
                BattleStatManager.AddBattleStat(forceId, heroId, damage, true, victim.isHero);

            SkillManager.OnAttack(this, victim, damType, damage);
        }

        // 创建攻击Action并添加到actions列表
        var attackAction = new AttackAction
        {
            SourceId = id,
            Tick = tickIndex,
            TargetId = victim.id,
            Damage = damage,
        };
        actions.Add(attackAction);

        if(!string.IsNullOrEmpty(effect))
            EffectManager.PlayHitEffect(this, victim, effect);
        victim.OnHpChanged();
    }

    public void OnSkillDamaged(Chess caster, int skillId, int damage, bool isFeedback = false)
    {
        // if(damage <= 0)
        //     throw new Exception("伤害值不能小于等于0");

        if (isHero)
        {
            SkillManager.OnDoSkillDamage(this, caster, SkillConfig.GetConfig(skillId), ref damage, isFeedback);
        }
        else
        {
            damage = Math.Max(damage, caster.attackDamage);//防止对士兵伤害过大
        }

        if(hp <= 0)
            return;

        hp -= damage;
        if(caster != this)
            lastDamagedPlayerId = caster.forceId;

        // 记录战斗统计
        if(caster.isHero)
            BattleStatManager.AddBattleStat(caster.forceId, caster.heroId, damage, false, isHero);            

        OnHpChanged();
    }


    public void OnHpChanged()
    {
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        if (hp <= 0)
        {
            Ondying();
        }
    }

    public void Ondying()
    {
        buffs.Clear();
        BattleManager.Instance.OnUnitDying(this);

        if (viewObj != null)
        {
            viewObj.DestroyHUD();
        }
        Debug.Log("OnDying " + id);
        if (viewObj != null)
        {
            UnityEngine.Object.Destroy(viewObj.gameObject);
            viewObj = null;
        }

        if ((forceId == 1 || forceId == 2 && !isShadow ))
            BGMPlayer.Instance.PlaySound("Sounds/tnt", 7);
    }

    private int CalculateDamage(Chess attacker, Chess defender, out string type)
    {
        if (!attacker.isHero || !defender.isHero)
        {
            type = "leadShip";
            return attacker.attackDamage;
        }

        // 计算攻击者三属性与防御者对应属性的差值
        float inteDiff = attacker.inte - defender.inte;
        float leadShipDiff = attacker.leadShip - defender.leadShip;
        float strDiff = attacker.str - defender.str;

        // 找出最大差值
        float maxDiff = Mathf.Max(inteDiff, leadShipDiff, strDiff);
        type = "";
        if(maxDiff == inteDiff)
        {
            type = "inte";
        }
        else if(maxDiff == leadShipDiff)
        {
            type = "leadShip";
        }
        else
        {
            type = "str";
        }

        // 伤害 = 最大差值 * 6
        int damage = Mathf.RoundToInt(maxDiff * 6);
        return damage;
    }

    public void AddHp(int addon)
    {
        if(addon <= 0)
            throw new Exception("添加的血量不能小于等于0");

        hp = Mathf.Clamp(hp + addon, 0, maxHp);
        OnHpChanged();
    }

    public void HealTarget(Chess target, int checkSkillId, int addon)
    {
        SkillManager.OnHealTarget(this, target, checkSkillId, ref addon);
        target.AddHp(addon);
    }

    public void Cooldown(int time)
    {
        attackPoint += time;
    }

    public void SetLifeTime(float time)
    {
        dieAfterLifeTime = true;
        // lifeTick = time;
    }

    public Player GetPlayerInfo()
    {
        return GameManager.Instance.GetPlayer(forceId);
    }

    public bool IsInFight(int nowTick)
    {
        return nowTick < lastAttackTime + 3;
    }

    public void AddBuff(Buff buff, Chess caster, float time)
    {
        // 计算buffTimes中所有20秒以内且buffId等于当前buff.id的buff的时间和
        float buffCount = 0;
        var nowTick = BattleManager.Instance.tickIndex;
        buffTimes.RemoveAll(buff => nowTick - buff.tick > 1200); // 30秒 = 30 / 0.025 = 1200 ticks
        foreach (var existingBuffTime in buffTimes)
        {
            if (existingBuffTime.id == buff.id)
                buffCount++;
        }
        if(buffCount >= 3)
        {
            time = Math.Max(.1f, time * (10 - buffCount) * .1f);
            buff.SetTime(time);
        }

        // 保留原有的buff刷新逻辑
        foreach(var item in buffs)
        {
            if(item.id == buff.id)
            {
                item.Refresh(caster, time);
                return;
            }
        }

        buffs.Add(buff);
        buff.OnAdd(this, caster);
        buffTimes.Add(new BuffTime{id = buff.id, tick = BattleManager.Instance.tickIndex});
    }

    public void AddColorEffect(Color start, Color end)
    {
        viewObj?.AddColorEffect(start, end);
    }

    public void RemoveColorEffect()
    {
        viewObj?.RemoveColorEffect();
    }

    public int GetAttr(string attr)
    {
        switch (attr)
        {
            case "inte":
                return inte;
            case "leadShip":
                return leadShip;
            case "str":
                return str;
            default:
                throw new ArgumentException("Invalid attribute name: " + attr);
        }
    }

    public int GetAttrTotal()
    {
        return inte + leadShip + str;
    }

    public void AddAttr(string attr, int value)
    {
        switch (attr)
        {
            case "inte":
                inte += value;
                break;
            case "leadShip":
                leadShip += value;
                break;
            case "str":
                str += value;
                break;
        }
        if(heroInfo != null)
            heroInfo.SetAttr(inte, str, leadShip);
    }

    public float HpRate{ get { return (float)hp / maxHp; } }

    public bool HasBuff(int id)
    {
        // Use Exists method since buffs is a List<Buff>
        return buffs.Exists(buff => buff.id == id);
    }

    public Buff GetBuff(int id)
    {
        return buffs.Find(buff => buff.id == id);
    }

    public bool MoveTo(Vector3 targetPosition, bool isForce = false)
    {
        return BattleManager.Instance.MoveTo(this, targetPosition, isForce);
    }

    public void PlayerAnim(string name)
    {
        if(BattleManager.Instance.quickMode)
            return;
        viewObj?.PlayerAnim(name);
    }

    public void StartJump(float time)
    {
        if(BattleManager.Instance.quickMode)
            return;
        viewObj?.StartJump(time);
    }

    public void StopJump()
    {
        if(BattleManager.Instance.quickMode)
            return;
        viewObj?.StopJump();
    }

    public void AddSkill(int skillId, int parentSkillId)
    {
        if(skills.Find(skill => skill.id == skillId || skill.id == parentSkillId) != null)
            return;

        var skillAdd = SkillManager.CreateSkill(skillId, this);
        skillAdd.isGivenSkill = true;
        skills.Add(skillAdd);
    }


}

