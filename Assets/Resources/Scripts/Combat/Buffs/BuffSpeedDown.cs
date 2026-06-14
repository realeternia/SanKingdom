using System;

public class BuffSpeedDown : Buff
{
    public float moveSpeedDiff;
    public float attackRateDiff;
    public BuffSpeedDown(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        moveSpeedDiff = chess.moveSpeed * skillCfg.Strength;
        chess.moveSpeed -= moveSpeedDiff;

        attackRateDiff = chess.attackRate * skillCfg.Strength;
        chess.attackRate -= (int)attackRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.moveSpeed += moveSpeedDiff;
        chess.attackRate += (int)attackRateDiff;
    }
}