using UnityEngine;

[System.Serializable]
public class AttackAction : ChessAction
{
    public int TargetId;
    public int Damage;
    public bool IsCrit;
    public bool IsDodge;
    public string HitEffect;
    public string DamType;

    public AttackAction(int sourceId, int tick, int targetId, int damage, bool isCrit, bool isDodge, string hitEffect, string damType)
        : base(sourceId, tick)
    {
        TargetId = targetId;
        Damage = damage;
        IsCrit = isCrit;
        IsDodge = isDodge;
        HitEffect = hitEffect;
        DamType = damType;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        var targetChess = BattleManager.Instance.GetChess(TargetId);

        sourceChess?.viewObj?.FaceTo(targetChess.position);
        sourceChess?.viewObj?.PlaySodAnim("sodattack");

        var actualDamage = Mathf.Min(Damage, targetChess.hp);
        targetChess.hp -= Damage;
        if (SourceId != targetChess.id)
            targetChess.lastDamagedPlayerId = SourceId;

        var attacker = BattleManager.Instance.GetChess(SourceId);
        if(IsCrit)
            BattleManager.Instance.AddBattleText("暴!", attacker.position, new UnityEngine.Vector2(0, 40), Color.red, 3);
        if(IsDodge)
            BattleManager.Instance.AddBattleText("闪!", targetChess.position, new UnityEngine.Vector2(0, 40), Color.red, 3);

        if(Damage > 0)
        {
            if(!string.IsNullOrEmpty(HitEffect))
                EffectManager.PlayHitEffect(sourceChess, targetChess, HitEffect);

            SkillManager.OnAttack(sourceChess, targetChess, DamType, Damage); 
        }

        if (attacker.isHero && actualDamage > 0)
        {
            BattleStatManager.AddDamage(attacker.forceId, attacker.heroId, actualDamage);
        }
        
        if (targetChess.isHero && actualDamage > 0)
        {
            BattleStatManager.AddBeDamaged(targetChess.forceId, targetChess.heroId, actualDamage);
        }

        targetChess.OnHpChanged();   
    }
}
