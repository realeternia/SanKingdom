using System;

[System.Serializable]
public class FoodCostAction : ChessAction
{
    public int ForceId;

    public FoodCostAction(int sourceId, int tick, int forceId)
        : base(sourceId, tick)
    {
        ForceId = forceId;
    }

    public override void Doing()
    {
        var foodInfo = BattleManager.Instance.GetFoodInfo(ForceId);
        if (foodInfo != null)
        {
            var costAmount = 10;
            if (foodInfo.food < costAmount)
            {
                var units = BattleManager.Instance.GetUnitsByForceId(foodInfo.forceId);
                foreach (var unit in units)
                    unit.LackFood((float)(costAmount - foodInfo.food) / costAmount);
            }

            foodInfo.food -= costAmount;
            if (foodInfo.food < 0)
                foodInfo.food = 0;
        }
    }
}
