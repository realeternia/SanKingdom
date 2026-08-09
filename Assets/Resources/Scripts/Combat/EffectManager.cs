using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;
public static class EffectManager
{

    public static void PlayHitEffect(Chess sourceChess, Chess targetChess, string effectName)
    {
        if(targetChess == null || targetChess.viewObj == null || BattleManager.Instance.quickMode)
            return;

        GameLog.Info($"PlayHitEffect: {effectName}");
        
        // 播放粒子特效
        var hitPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Effect(effectName));
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, targetChess.position, Quaternion.identity);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = targetChess.viewObj.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 5f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, 1.3f);
    }

    public static GameObject PlaySkillEffect(Chess sourceChess, string effect, float time = 1.3f)
    {
        if(sourceChess.viewObj == null || BattleManager.Instance.quickMode)
            return null;

        GameLog.Info("PlaySkillEffect: " + effect);
        var hitPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Effect(effect));

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.viewObj.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 5f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);
        return hitEffect;
    }

    public static GameObject PlayPosSkillEffect(Chess sourceChess, Vector3 sourcePos, float size, string effect, float time = 1.3f)
    {
        if (sourceChess != null && sourceChess.viewObj == null)
            return null;
        if (BattleManager.Instance.quickMode)
            return null;

        GameLog.Info("PlayPosSkillEffect: " + effect);
        var hitPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Effect(effect));

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourcePos, hitPrefab.transform.rotation);
        if (sourceChess != null && sourceChess.viewObj != null)
        {
            hitEffect.transform.parent = sourceChess.viewObj.transform;
            hitEffect.transform.localScale = size * hitPrefab.transform.localScale;
            hitEffect.transform.localPosition += new Vector3(0f, 5f, 0f);
        }
        else
        {
            hitEffect.transform.localScale = size * hitPrefab.transform.localScale;
            hitEffect.transform.position = sourcePos + new Vector3(0f, 5f, 0f);
        }
        // time<=0 表示持久特效，由调用方负责销毁
        if (time > 0)
            UnityEngine.Object.Destroy(hitEffect, time);

        return hitEffect;
    }

    public static GameObject PlayBuffEffect(Chess sourceChess, string effect)
    {
        if(sourceChess.viewObj == null || BattleManager.Instance.quickMode)
            return null;
        
        GameLog.Info("PlayBuffEffect: " + effect);
        var hitPrefab = ResourceCache.LoadPrefabBattle(ResPath.Prefab.Effect(effect));

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.viewObj.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 5f, 0f);

        return hitEffect;

    }

}
