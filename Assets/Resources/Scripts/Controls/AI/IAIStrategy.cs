using System.Collections.Generic;

public interface IAIStrategy
{
    void Execute(AIStrategyContext context);
    string GetStrategyName();
}
