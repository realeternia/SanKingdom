using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Controls.Utils;

[Serializable]
public class Chess : SceneObj
{
    [NonSerialized]
    public ChessViewObj viewObj;
    [NonSerialized]
    public HeroInfo heroInfo;

    public int forceId;


    public bool isHero;
    public bool isFakeHero;
    public bool isShadow;

    public int heroId;
    public int heroId2;
    public int heroId3;
    public int battleUnitId;
    
    public int armsId;
    public int str;
    public int leadShip;
    public int inte;

    [NonSerialized]
    public string chessName = "0";

    public int targetChessId;
    // 目标单位
    // 移动速度

    public int level = 1;
    
    public float dodgeRate; //闪避
    public float critRate; //暴击
    public float critDamageMulti = SystemConst.Battle.DEFAULT_CRIT_DAMAGE_MULTI; //暴击伤害倍率

    public int lastDamagedPlayerId = -1;

    public int maxHp = 100;  // 最大生命值
    // 是否正在使用偏移路径
    public int hp = 100;

    public float HpRate{ get { return (float)hp / maxHp; } }    

    [NonSerialized]
    public float moveSpeed = 5f;
    [NonSerialized]
    public float attackRange = 10f;
    [NonSerialized]
    public int atk;
    [NonSerialized]
    public int def;
    [NonSerialized]
    public string hitEffect;
    [NonSerialized]
    public int missileSpeed = 10;
    [NonSerialized]
    public float missileHeight;
    
    // 攻击冷却时间
    public int attackPoint;
    public int attackRate; //攻击频率
    public int lastAttackTime = 0;
    public int lastTargetUpdateTick; // 上次更新目标的时间

    public List<Skill> skills = new List<Skill>();

    public List<Buff> buffs = new List<Buff>();
    public int noMoveCount = 0;
    public int noActionCount = 0;
    public bool isInAttackRange = false;

    // 跳跃相关状态
    public JumpState jumpState;

    [Serializable]
    public class JumpState
    {
        public Vector3 PosStart;
        public Vector3 PosTar;
        public float Height;
        public int TickTotal;
        public int TickPast;
    }

    // 持续伤害相关状态
    public List<DamageOverTimeState> dotStates = new List<DamageOverTimeState>();

    [Serializable]
    public class DamageOverTimeState
    {
        public int casterId;
        public int skillId;
        public float damage;
        public int tickCount;
        public int tickInterval;
    }

    public bool dieAfterLifeTime;
    public int lifeTickCount; //1s死亡一次

    public int regeTickCount; //1s回复一次
    public int regeHp; //回复血量

    public Chess(int id)
    {
        base.id = id;
    }

    public void Init(int forceId)
    {
        this.forceId = forceId;

        hp = maxHp;
        
        attackPoint = BattleRandom.Range(SystemConst.Battle.INIT_ATTACK_POINT_MIN, SystemConst.Battle.INIT_ATTACK_POINT_MAX);
        attackRate = 1;

        if (isHero)
        {
            GameLog.Info("Init Hero" + heroId);

            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg.Skills != null)
            {
                foreach (var skillId in heroCfg.Skills)
                    skills.Add(SkillManager.CreateSkill(skillId, this));
            }

        }

        // 创建UI
        CreateChessView();

    }

    private void CreateChessView()
    {
        if(heroId > 0)
        {
            var heroConfig = HeroConfig.GetConfig(heroId);
            chessName = heroConfig.Icon;
        }

        var armsConfig = ArmsConfig.GetConfig(armsId);
        hitEffect = armsConfig.HitEffect;
        missileSpeed = armsConfig.MissileSpeed;
        missileHeight = armsConfig.MissileHight;
        moveSpeed = armsConfig.MoveSpeed;
        attackRange = armsConfig.Range;        

        if (BattleManager.Instance.showUI)
        {
            SaveForceData force = GameManager.Instance.GetForce(forceId);
            if (isHero)
            {
                GameObject heroPrefab = Resources.Load<GameObject>(ResPath.Prefab.UnitModel("UnitHero"));
                GameObject unitModel = UnityEngine.Object.Instantiate(heroPrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);
                unitModel.name = $"Hero_{forceId}_{id}";
                unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                viewObj = unitModel.GetComponent<ChessViewObj>();
                viewObj.Init(this, force.LineColor);

                var heroInfo = BattleManager.Instance.battleUIManager.heroInfoGroup.AddHero(forceId, heroId, level);
                heroInfo.SetAttr(inte, str, leadShip);
                heroInfo.SetHpRate(maxHp, maxHp);
                this.heroInfo = heroInfo;

                viewObj.UpdateSoldierModels();
            }
            else
            {
                var battleUnitConfig = BattleUnitConfig.GetConfig(battleUnitId);
                GameObject unitPrefab = Resources.Load<GameObject>(ResPath.Prefab.UnitModel(battleUnitConfig.Model));
                GameObject unitModel = UnityEngine.Object.Instantiate(unitPrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);
                unitModel.name = $"UnitBing_{forceId}_{id}";
                unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                viewObj = unitModel.GetComponent<ChessViewObj>();
                viewObj.Init(this, force.LineColor);
            }
        }

    }

    public override void OnRecover()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].OnRecover();
        }
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].OnRecover();
        }
        CreateChessView();
    }    

    public override void LogicUpdate(int tickIndex)
    {
        if (hp <= 0)
        {
            Ondying();
            return;
        }
        // 死亡判定
        if (dieAfterLifeTime)
        {
            lifeTickCount--;
            if (lifeTickCount <= 0)
            {
                Ondying();
            }
        }        

        buffs.Where(x => tickIndex > x.endTime).ToList().ForEach(x => BuffManager.RemoveBuff(this, x.id));
        SkillManager.LogicUpdate(this, tickIndex);

        CheckHpReg();

        // 处理持续伤害逻辑
        for (int i = dotStates.Count - 1; i >= 0; i--)
        {
            var dotState = dotStates[i];
            dotState.tickCount++;

            if (dotState.tickCount >= dotState.tickInterval)
            {
                dotState.tickCount = 0;

                // 造成伤害
                if (hp > 0)
                {
                    var caster = BattleManager.Instance.GetChess(dotState.casterId);
                    if (caster != null)
                    {
                        DoSkillDamage(caster, dotState.skillId, (int)dotState.damage);
                    }
                }
            }
        }

        // 处理跳跃逻辑
        if (jumpState != null)
        {
            if (jumpState.TickPast >= jumpState.TickTotal)
            {
                // 确保到达目标位置
                MoveTo(jumpState.PosTar, true);
                jumpState = null;

                FindTarget(); //重新锁定一次
            }
        }
        else
        {
            MoveAndFight(tickIndex);
        }

    }

    public override void RenderUpdate(int tickIndex, float indexMini, float timeElapsed)
    {
        base.RenderUpdate(tickIndex, indexMini, timeElapsed);

        if (jumpState != null)
        {
            if (jumpState.TickPast < jumpState.TickTotal)
            {
                jumpState.TickPast++;
                // 计算插值因子
                float t = (float)jumpState.TickPast / jumpState.TickTotal;

                // 计算当前位置（带跳跃效果）
                float yOffset = jumpState.Height * Mathf.Sin(t * Mathf.PI);
                Vector3 currentPos = Vector3.Lerp(jumpState.PosStart, jumpState.PosTar, t);
                currentPos.y += yOffset;

                if (viewObj != null && position != Vector3.zero)
                    viewObj.transform.position = position; //只改view
            }
        }     
    }

    private void CheckHpReg()
    {
        if (regeHp > 0)
        {
            regeTickCount++;
            if (regeTickCount >= SystemConst.Battle.REGE_INTERVAL_TICKS)
            {
                regeTickCount -= SystemConst.Battle.REGE_INTERVAL_TICKS;
                AddHp(regeHp);
            }
        }
    }

    public override void SetPosition(Vector3 position)
    {
        if (position == Vector3.zero)
            return;
        base.SetPosition(position);
        position.y = 7 + id * 0.01f;
        if(viewObj != null)
            viewObj.transform.position = position;
    }

    public void LockTarget(Chess target1)
    {
        targetChessId = target1.id;
        // lastTargetUpdateTick = BattleManager.Instance.time;
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
                float distance = BattleManager.GetRange(position, chess.position);
                validTargets.Add((chess, distance));
            }
        }

        // 如果没有有效目标，直接返回
        if (validTargets.Count == 0)
        {
            targetChessId = 0;
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
            filteredTargets = validTargets.Where(t => t.distance <= nearestDistance + SystemConst.Battle.TARGET_SEARCH_EXTRA_RANGE).ToList();

        // 如果筛选后不足3个，则取全部
        int takeCount = Mathf.Min(SystemConst.Battle.TARGET_SCORE_SELECT_COUNT, filteredTargets.Count);
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
        targetChessId = scoredTargets[0].chess.id;
        if(viewObj != null)
            viewObj.lockTargetId = targetChessId;
    }

    // 计算目标分数
    private float CalculateTargetScore(Chess target, float distance)
    {
        float score = SysFormula.Battle.CalculateTargetScore(
            target.isHero, distance, attackRange,
            CalculateDamage(this, target), level, target.level, (float)target.hp / target.maxHp);
        return score;
    }

    private void MoveAndFight(int tickIndex)
    {
        if (noActionCount > 0)
            return;

        // 每3秒重新寻找目标
        if (tickIndex - lastTargetUpdateTick >= SystemConst.Battle.TARGET_UPDATE_INTERVAL_TICKS)
        {
            FindTarget();
            lastTargetUpdateTick = tickIndex;
        }

        // 检查目标是否存在
        var targetChess = BattleManager.Instance.GetChess(targetChessId);
        if (targetChess == null || targetChess.hp <= 0)
        {
            // 如果没有目标，尝试寻找新目标
            FindTarget();

            targetChess = BattleManager.Instance.GetChess(targetChessId);
            if (targetChess == null)
                return;
        }

        // 检查是否有辅助技能
        if (SkillManager.CheckAidSkill(this, tickIndex))
            return;

        // 检查目标是否在攻击范围内
        if (BattleManager.CheckInRange(position, targetChess.position, attackRange))
        {
            if (!isInAttackRange)
            {
                isInAttackRange = true;
                viewObj?.PlaySodAnim("idle");
            }
            attackPoint += attackRate;
            // 检查攻击冷却
            if (attackPoint >= SystemConst.Battle.ATTACK_POINT_THRESHOLD)
            {
                attackPoint -= SystemConst.Battle.ATTACK_POINT_COST;
                SkillManager.AimTarget(this, targetChess);
                if (attackRange >= SystemConst.Battle.RANGE_ATTACK_THRESHOLD)
                {
                    BattleManager.Instance.CreateAttackMissile(this, targetChess);
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

        if (isInAttackRange)
        {
            isInAttackRange = false;
            viewObj?.PlaySodAnim("sodmove");
        }

        var moveDest = GetMoveDest();
        if (moveDest != Vector3.zero)
        {
            attackPoint += attackRate;
            if (attackPoint >= SystemConst.Battle.MOVE_POINT_THRESHOLD)
            {
                attackPoint -= SystemConst.Battle.MOVE_POINT_COST;

                // 创建移动Action并添加到actions列表
                targetChess = BattleManager.Instance.GetChess(targetChessId);
                var moveAction = new MoveAction(id, tickIndex, targetChess != null ? targetChess.id : -1, moveDest);
                BattleManager.Instance.AddChessAction(moveAction);
            }
        }
    }

    private Vector3 GetMoveDest()
    {
        int moveFailCount = 0;
        var moveDis = moveSpeed * SystemConst.Battle.MOVE_DISTANCE_FACTOR;

        // 检查目标是否存在
        var targetChess = BattleManager.Instance.GetChess(targetChessId);
        for (int i = 0; i < SystemConst.Battle.MOVE_SHORT_ATTEMPT_COUNT; i++)
        {
            Vector3 nextPosition = Vector3.MoveTowards(position, targetChess.position, moveDis * (SystemConst.Battle.MOVE_SHORT_ATTEMPT_COUNT - i) / SystemConst.Battle.MOVE_SHORT_ATTEMPT_COUNT);
            if (BattleManager.Instance.IsPositionFree(this, nextPosition))
                return nextPosition;
        }

        for (int i = 0; i < SystemConst.Battle.MOVE_LONG_ATTEMPT_COUNT; i++)
        {
            Vector3 nextPosition = Vector3.MoveTowards(position, targetChess.position, moveDis * (i + 1) / SystemConst.Battle.MOVE_SHORT_ATTEMPT_COUNT);
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
            if (moveFailCount <= SystemConst.Battle.PATHFIND_FAIL_COUNT_LOW)
                angleOffset = SystemConst.Battle.PATHFIND_ANGLE_OFFSET_LOW;
            else if (moveFailCount <= SystemConst.Battle.PATHFIND_FAIL_COUNT_MID)
                angleOffset = SystemConst.Battle.PATHFIND_ANGLE_OFFSET_MID;
            else
                angleOffset = SystemConst.Battle.PATHFIND_ANGLE_OFFSET_HIGH;

            angleOffset *= BattleRandom.Value > 0.5f ? 1 : -1;

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
        var damage = CalculateDamage(this, victim);
        var effect = hitEffectName;
        var damageBase = damage;
        var damageMulti = 1f;
        var damageReal = 0; //真实伤害
        bool isCrit = false;
        bool isDodge = false;

        SkillManager.DuringAttack(this, victim, "str", ref damageBase, ref damageMulti, ref damageReal, ref effect);
        // 暴击
        if (critRate > 0 && BattleRandom.Value < critRate)
        {
            damageMulti += critDamageMulti;
            isCrit = true;
        }

        damage = (int)(damageBase * damageMulti);
        var levelDiff = (isHero && victim.isHero) ? level - victim.level : 0;
        var (minDamage, maxDamage) = SysFormula.Battle.GetDamageRange(levelDiff, isCrit, critDamageMulti);
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        if (damage > 0)
        {
            if (victim.dodgeRate > 0 && BattleRandom.Value < victim.dodgeRate)
            {
                damage = 0;
                isDodge = true;
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

            // 创建攻击Action并添加到actions列表
            var attackAction = new AttackAction(id, tickIndex, victim.id, damage, isCrit, isDodge, effect, "str");
            BattleManager.Instance.AddChessAction(attackAction);     
        }
    }

    public void DoSkillDamage(Chess caster, int skillId, int damage, bool isFeedback = false)
    {
        if(hp <= 0)
            return;

        var skillCfg = SkillConfig.GetConfig(skillId);
        SkillManager.OnDoSkillDamage(this, caster, skillCfg, ref damage, isFeedback);          

        // 创建SkillDamageAction并添加到BattleManager
        var action = new SkillDamageAction(caster.id, BattleManager.Instance.tickIndex, id, skillId, damage);
        BattleManager.Instance.AddChessAction(action);
    }

    public void OnHpChanged()
    {
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        
        if (isHero && viewObj != null)
            viewObj.UpdateSoldierModels();
    }

    public void Ondying()
    {
        var action = new RemoveChessAction(id, BattleManager.Instance.tickIndex);
        BattleManager.Instance.AddChessAction(action);
    }

    private static int CalculateDamage(Chess attacker, Chess defender)
    {
        return SysFormula.Battle.CalculateDamage(attacker.atk, attacker.hp, defender.def);
    }

    public void JumpToPosition(Vector3 targetPos, float jumpHeight = 10f, float moveDuration = 0.5f)
    {
        if(BattleManager.Instance.quickMode)
            return;

        if(jumpState != null)
            return;

        jumpState = new JumpState();
        jumpState.PosStart = position;
        jumpState.PosTar = targetPos;
        jumpState.Height = jumpHeight;
        jumpState.TickTotal = BattleManager.Instance.GetTickFromTime(moveDuration);
        jumpState.TickPast = 0;
    }

    public void AddHp(int addon)
    {        
        if(addon <= 0)
            throw new Exception("添加的血量不能小于等于0");

        var action = new ChessChangeHpAction(id, BattleManager.Instance.tickIndex, addon);
        BattleManager.Instance.AddChessAction(action);
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
        lifeTickCount = BattleManager.Instance.GetTickFromTime(time);
    }

    public SaveForceData GetForceInfo()
    {
        return GameManager.Instance.GetForce(forceId);
    }

    public bool IsInFight(int nowTick)
    {
        return nowTick < lastAttackTime + SystemConst.Battle.IN_FIGHT_TICK_THRESHOLD;
    }

    public void AddBuff(Buff buff, Chess caster, int endTick)
    {
        // 保留原有的buff刷新逻辑
        foreach(var item in buffs)
        {
            if(item.id == buff.id)
            {
                item.Refresh(caster, endTick);
                return;
            }
        }

        buffs.Add(buff);
        buff.OnAdd(this, caster);
    }

    // 添加持续伤害状态
    public void AddDamageOverTimeState(int casterId, int skillId, float damage)
    {
        var dotState = new DamageOverTimeState
        {
            casterId = casterId,
            skillId = skillId,
            damage = damage,
            tickCount = 0,
            tickInterval = BattleManager.Instance.GetTickFromTime(1) // 1秒 = 10 tick
        };
        dotStates.Add(dotState);
    }

    // 移除持续伤害状态
    public void RemoveDamageOverTimeState(int skillId)
    {
        dotStates.RemoveAll(state => state.skillId == skillId);
    }

    public void AddColorEffect(Color start, Color end)
    {
    }

    public void RemoveColorEffect()
    {
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
        viewObj?.PlayAnim(name);
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

