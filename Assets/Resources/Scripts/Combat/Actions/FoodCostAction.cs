using System;

[System.Serializable]
public class FoodCostAction : ChessAction
{
    public int ForceId;
    public int CostAmount;

    public FoodCostAction(int sourceId, int tick, int forceId, int costAmount)
        : base(sourceId, tick)
    {
        ForceId = forceId;
        CostAmount = costAmount;
    }

    public override void Doing()
    {
        var foodInfo = BattleManager.Instance.GetFoodInfo(ForceId);
        if (foodInfo != null)
        {
            foodInfo.food -= CostAmount;
            if (foodInfo.food < 0)
                foodInfo.food = 0;
        }
    }
}
