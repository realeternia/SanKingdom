using System;
using UnityEngine;

[Serializable]
public class SceneObj : IRecoverable
{
    public int id;
    public Vector3 position;
    
    public virtual void OnRecover()
    {
    }
    
    public virtual void LogicUpdate(int tickIndex)
    {
    }

    public virtual void FixUpdate(int tickIndex, float indexMini, float timeElapsed)
    {
    }
    
    public virtual void SetPosition(Vector3 pos)
    {
        position = pos;
    }
}
