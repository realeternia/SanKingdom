using System;
using System.Collections;
using System.Collections.Generic;

// 协程等待指令基类
public abstract class YieldInstruction
{
    public virtual bool CheckWait(float timePast) { return false; }

}

// 等待秒数的指令
public class NLWaitForSeconds : YieldInstruction
{
    private float _targetTime;
    
    public NLWaitForSeconds(float seconds)
    {
        _targetTime = seconds;
    }
    
    // 当前时间（需要根据实际情况实现）
    private float _timePast = 0;

    public override bool CheckWait(float timePast)
    {
        _timePast += timePast;
        return _timePast < _targetTime;
    }
}

// 协程管理器
public class NLCoroutineManager
{
    private List<IEnumerator> _coroutines = new List<IEnumerator>();
    private List<IEnumerator> _coroutinesToAdd = new List<IEnumerator>();

    // 更新所有协程
    public void Update(float timePast)
    {
        // 添加新协程
        if (_coroutinesToAdd.Count > 0)
        {
            _coroutines.AddRange(_coroutinesToAdd);
            _coroutinesToAdd.Clear();
        }

        // 更新所有协程
        for (int i = _coroutines.Count - 1; i >= 0; i--)
        {
            var coroutine = _coroutines[i];
            if (!MoveNext(coroutine, timePast))
            {
                _coroutines.RemoveAt(i);
            }
        }
    }

    // 执行协程的下一步
    private static bool MoveNext(IEnumerator coroutine, float timePast)
    {
        if (coroutine.Current is YieldInstruction yieldInstruction)
        {
            if (yieldInstruction.CheckWait(timePast))
            {
                return true; // 继续等待
            }
        }

        // 执行下一步
        if (coroutine.MoveNext())
        {
            return true;
        }

        return false; // 协程结束
    }

    // 启动协程
    public void StartCoroutine(IEnumerator coroutine)
    {
        UnityEngine.Debug.Log("StartCoroutine " + coroutine);
        // 先执行第一步
        if (coroutine.MoveNext())
        {
            _coroutinesToAdd.Add(coroutine);
        }
    }

    // 停止协程
    public void StopCoroutine(IEnumerator coroutine)
    {
        _coroutines.Remove(coroutine);
        _coroutinesToAdd.Remove(coroutine);
    }
}