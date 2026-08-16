using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class Chess : SceneObj
{
    [NonSerialized]
    public ChessViewObj viewObj;
    [NonSerialized]
    public BattleHeroInfo heroInfo;

    public int forceId;


    public bool isHero;
    public bool isFakeHero;
    public bool isShadow;
    public bool isSodNull;
    public bool isGate;
    public bool isWall;
    public bool isTower;

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
    /// <summary>
    /// 行动速度(1-20)，决定回合行动顺序（speed 大者先行动，相同则随机）
    /// </summary>
    [NonSerialized]
    public int speed = 10;
    [NonSerialized]
    public float attackRange = 1;
    public int atk;
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
    public float lastAttackTime = 0; // 上次攻击时刻(秒)，当前未赋值，保留现状

    public List<BattleSkill> skills = new List<BattleSkill>();

    public List<Buff> buffs = new List<Buff>();
    public int noMoveCount = 0;
    public int noActionCount = 0;
    public bool isInAttackRange = false;
    public bool isTurnFinished = false;
    public bool hasPendingAction = false;
    public bool isDying = false;

    // 持续伤害相关状态
    public List<DamageOverTimeState> dotStates = new List<DamageOverTimeState>();

    [Serializable]
    public class DamageOverTimeState
    {
        public int casterId;
        public int skillId;
        public float damage;
    }

    public bool dieAfterLife;
    public int lifeRoundCount; //剩余存活回合数

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
        if (isGate || isWall)
        {
            if (BattleManager.Instance.showUI && !BattleManager.Instance.quickMode)
            {
                var go = new GameObject($"{(isGate ? "Gate" : "Wall")}_{id}");
                go.transform.SetParent(BattleManager.Instance.battleUIManager.NodeUnits.transform);
                go.transform.position = position;
                viewObj = go.AddComponent<ChessViewObj>();
                viewObj.Init(this, Color.white);

                var unitConfig = BattleUnitConfig.GetConfig(battleUnitId);
                var prefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.UnitModel(unitConfig.Model));
                if (prefab != null)
                {
                    var model = UnityEngine.Object.Instantiate(prefab, go.transform);
                    model.transform.position = position;
                }
            }
            return;
        }

        if (isTower)
        {
            if (BattleManager.Instance.showUI && !BattleManager.Instance.quickMode)
            {
                var go = new GameObject($"Tower_{id}");
                go.transform.SetParent(BattleManager.Instance.battleUIManager.NodeUnits.transform);
                go.transform.position = position;
                viewObj = go.AddComponent<ChessViewObj>();
                viewObj.Init(this, Color.white);

                var unitConfig = BattleUnitConfig.GetConfig(battleUnitId);
                var prefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.UnitModel(unitConfig.Model));
                if (prefab != null)
                {
                    var model = UnityEngine.Object.Instantiate(prefab, go.transform);
                    model.transform.position = position;
                }
            }

            var towerArmsCfg = ArmsConfig.GetConfig(armsId);
            hitEffect = towerArmsCfg.HitEffect;
            missileSpeed = towerArmsCfg.MissileSpeed;
            missileHeight = towerArmsCfg.MissileHight;
            attackRange = towerArmsCfg.Range;
            speed = towerArmsCfg.Speed;
            return;
        }

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
        speed = armsConfig.Speed;

        // 科技加成：兵种移速加算
        int techMoveSpeed = ForceTech.GetArmsAttrAdd(forceId, armsId, "MoveSpeed");
        if (techMoveSpeed > 0)
            moveSpeed += techMoveSpeed;

        if (BattleManager.Instance.showUI)
        {
            SaveForceData force = GameManager.Instance.GetForce(forceId);
            if (isSodNull)
            {
                GameObject heroPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.UnitModel("UnitHero"));
                GameObject unitModel = UnityEngine.Object.Instantiate(heroPrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);
                unitModel.name = $"SodNull_{forceId}_{id}";
                unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                viewObj = unitModel.GetComponent<ChessViewObj>();
                viewObj.Init(this, force.LineColor);
            }
            else if (isHero)
            {
                GameObject heroPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.UnitModel("UnitHero"));
                GameObject unitModel = UnityEngine.Object.Instantiate(heroPrefab, position, Quaternion.identity, BattleManager.Instance.battleUIManager.NodeUnits.transform);
                unitModel.name = $"Hero_{forceId}_{id}";
                unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                viewObj = unitModel.GetComponent<ChessViewObj>();
                viewObj.Init(this, force.LineColor);

                if (!BattleManager.Instance.isDeployPhase)
                {
                    var heroInfo = BattleManager.Instance.battleUIManager.AddHero(forceId, heroId, level, heroId2, heroId3, inte, atk, def);
                    heroInfo.SetHpRate(maxHp, maxHp);
                    this.heroInfo = heroInfo;
                }

                viewObj.UpdateSoldierModels();
            }
            else
            {
                var battleUnitConfig = BattleUnitConfig.GetConfig(battleUnitId);
                GameObject unitPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.UnitModel(battleUnitConfig.Model));
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

    public void OnTurnStart()
    {
        isTurnFinished = false;
        hasPendingAction = false;
        // 重置攻击点数
        attackPoint = SystemConst.Battle.ATTACK_POINT_THRESHOLD; // 回合制下每回合可以行动一次

        // 召唤物生命周期（按回合）
        if (dieAfterLife)
        {
            lifeRoundCount--;
            if (lifeRoundCount <= 0)
            {
                Ondying();
                isTurnFinished = true;
                return;
            }
        }

        // 技能回合更新
        SkillManager.LogicUpdate(this);

        // 生命回复
        CheckHpReg();

        // 持续伤害（每回合结算一次）
        for (int i = dotStates.Count - 1; i >= 0; i--)
        {
            var dotState = dotStates[i];

            if (hp > 0)
            {
                var caster = BattleManager.Instance.GetChess(dotState.casterId);
                if (caster != null)
                {
                    DoSkillDamage(caster, dotState.skillId, (int)dotState.damage, false, 0);
                }
            }
        }

        // 格子持续效果结算：施法者行动时，对其施放的效果在其所在格结算一次伤害（城门/箭塔等不行动单位也可被结算）
        BattleManager.Instance.TriggerCellEffectsByCaster(this);
    }

    public void OnTurnAction()
    {
        if (hp <= 0)
        {
            isTurnFinished = true;
            return;
        }

        ChessAI.ProcessTurn(this);

        if (!hasPendingAction)
            isTurnFinished = true;
    }

    public void OnTurnEnd()
    {
        isTurnFinished = true;
        hasPendingAction = false;
    }

    /// <summary>
    /// 伤害结算后调用，结束攻击者的待定回合
    /// </summary>
    public void FinishPendingAction()
    {
        if (hasPendingAction)
        {
            hasPendingAction = false;
            isTurnFinished = true;
        }
    }

    public override void RenderUpdate()
    {
        base.RenderUpdate();
    }

    private void CheckHpReg()
    {
        if (regeHp > 0)
            AddHp(regeHp);
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
    }

    // 发起攻击（仅创建AttackAction，伤害计算在AttackAction中延迟执行）
    public void Attack(Chess victim, string hitEffectName)
    {
        if (victim == null)
            return;

        var isRanged = attackRange >= SystemConst.Battle.RANGE_ATTACK_THRESHOLD;
        var attackAction = new AttackAction(id, BattleManager.Instance.battleTime, victim.id, hitEffectName, "str", isRanged);
        BattleManager.Instance.AddChessAction(attackAction);
    }

    // 伤害结算（从AttackAction.Doing()和Missile.OnCrash()调用）
    public void OnAttackDamage(Chess victim, int damage, bool isCrit, bool isDodge, string hitEffect, string damType, int actionId)
    {
        GameLog.Info($"OnAttackDamage[aid={actionId}] src={id} tgt={victim.id} dmg={damage} crit={isCrit} dodge={isDodge}");

        var actualDamage = Mathf.Min(damage, victim.hp);
        victim.hp -= damage;
        if (id != victim.id)
            victim.lastDamagedPlayerId = id;

        // 城门血量同步：一扇门受伤，其余城门同损
        if (victim.isGate)
            BattleManager.Instance.SyncGateDamage(victim, damage);

        if(isCrit)
            BattleManager.Instance.AddBattleText("暴!", position, new UnityEngine.Vector2(0, 40), Color.red, 3);
        if(isDodge)
            BattleManager.Instance.AddBattleText("闪!", victim.position, new UnityEngine.Vector2(0, 40), Color.red, 3);

        if(damage > 0)
            BattleManager.Instance.AddBattleText("-" + damage.ToString(), victim.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.DamageColor, 7);

        if(damage > 0)
        {
            if(!string.IsNullOrEmpty(hitEffect))
                EffectManager.PlayHitEffect(this, victim, hitEffect);

            SkillManager.OnAttack(this, victim, damType, damage);
        }

        if (isHero && actualDamage > 0)
        {
            BattleStatManager.AddDamage(forceId, heroId, actualDamage);
        }

        if (victim.isHero && actualDamage > 0)
        {
            BattleStatManager.AddBeDamaged(victim.forceId, victim.heroId, actualDamage);
        }

        victim.OnHpChanged();

        // 伤害结算后结束攻击者的待定回合
        FinishPendingAction();
    }

    public void DoSkillDamage(Chess caster, int skillId, int damage, bool isFeedback, int actionId)
    {
        if(hp <= 0)
            return;

        GameLog.Info($"DoSkillDamage[aid={actionId}] caster={caster.id} tgt={id} skill={skillId} dmg={damage}");

        var skillCfg = BattleSkillConfig.GetConfig(skillId);
        SkillManager.OnDoSkillDamage(this, caster, skillCfg, ref damage, isFeedback);          

        // 创建SkillDamageAction并添加到BattleManager
        var action = new SkillDamageAction(caster.id, BattleManager.Instance.battleTime, id, skillId, damage);
        BattleManager.Instance.AddChessAction(action);
    }

    public void OnHpChanged()
    {
        if (heroInfo != null) // 英雄
            heroInfo.SetHpRate(hp, maxHp);
        
        if (isHero && viewObj != null)
            viewObj.UpdateSoldierModels();
        if (hp <= 0 && !SkillManager.isReplay)
        {
            Ondying();
            return;
        }

    }

    public void Ondying()
    {
        if (isDying) return;
        isDying = true;
        var action = new RemoveChessAction(id, BattleManager.Instance.battleTime);
        BattleManager.Instance.AddChessAction(action);
    }

    // 计算攻击伤害（含暴击、闪避、技能加成等），供AttackAction和技能共用
    public static (int damage, bool isCrit, bool isDodge, string effect) CalculateAttackDamage(Chess attacker, Chess defender, string damType, string hitEffect)
    {
        var damage = SysFormula.Battle.CalculateDamage(attacker.atk, attacker.hp, defender.def);
        var effect = hitEffect;
        var damageBase = damage;
        var damageMulti = 1f;
        var damageReal = 0;
        bool isCrit = false;
        bool isDodge = false;

        SkillManager.DuringAttack(attacker, defender, damType, ref damageBase, ref damageMulti, ref damageReal, ref effect);
        // 暴击
        if (attacker.critRate > 0 && BattleRandom.Value < attacker.critRate)
        {
            damageMulti += attacker.critDamageMulti;
            isCrit = true;
        }

        damage = (int)(damageBase * damageMulti);
        var levelDiff = (attacker.isHero && defender.isHero) ? attacker.level - defender.level : 0;
        var (minDamage, maxDamage) = SysFormula.Battle.GetDamageRange(levelDiff, isCrit, attacker.critDamageMulti);
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        if (damage > 0)
        {
            if (defender.dodgeRate > 0 && BattleRandom.Value < defender.dodgeRate)
            {
                damage = 0;
                isDodge = true;
            }
            else
            {
                //这里不改数值，只能伤害吸收
                SkillManager.BeforeAttack(attacker, defender, ref damage);
            }
        }

        if (damage + damageReal > 0)
            damage = Math.Max(damage, damageReal);
        else
            damage = 0;

        return (damage, isCrit, isDodge, effect);
    }

    public void AddHp(int addon)
    {        
        if(addon <= 0)
            throw new Exception("添加的血量不能小于等于0");

        var action = new AddHpAction(id, BattleManager.Instance.battleTime, addon);
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

    public void SetLifeRound(int round)
    {
        dieAfterLife = true;
        lifeRoundCount = round;
    }

    public SaveForceData GetForceInfo()
    {
        return GameManager.Instance.GetForce(forceId);
    }

    public bool IsInFight(float nowTime)
    {
        return nowTime < lastAttackTime + SystemConst.Battle.IN_FIGHT_TIME_THRESHOLD;
    }

    public void AddBuff(Buff buff, Chess caster, int endRound)
    {
        // 保留原有的buff刷新逻辑
        foreach(var item in buffs)
        {
            if(item.id == buff.id)
            {
                item.Refresh(caster, endRound);
                return;
            }
        }

        buffs.Add(buff);
        buff.OnAdd(this, caster);
    }

    // 添加持续伤害状态（回合制：每回合结算一次）
    public void AddDamageOverTimeState(int casterId, int skillId, float damage)
    {
        var dotState = new DamageOverTimeState
        {
            casterId = casterId,
            skillId = skillId,
            damage = damage
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

    public void AddSkill(int skillId, int parentSkillId)
    {
        if(skills.Find(skill => skill.id == skillId || skill.id == parentSkillId) != null)
            return;

        var skillAdd = SkillManager.CreateSkill(skillId, this);
        skillAdd.isGivenSkill = true;
        skills.Add(skillAdd);
    }
}

