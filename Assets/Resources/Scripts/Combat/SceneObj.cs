using System;
using UnityEngine;

[Serializable]
public class SceneObj : IRecoverable
{
    public int id;
    public Vector3 position;

    public SceneObj()
    {
        position = Vector3.zero;
    }
    
    public virtual void OnRecover()
    {
    }
    
    //计算用update
    public virtual void LogicUpdate(int tickIndex)
    {
    }

    //表现update
    public virtual void RenderUpdate(int tickIndex, float indexMini, float timeElapsed)
    {
    }
    
    public virtual void SetPosition(Vector3 pos)
    {
        position = pos;
    }
}
