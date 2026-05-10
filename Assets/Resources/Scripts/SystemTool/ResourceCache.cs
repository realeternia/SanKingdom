using System;
using UnityEngine;

public static class ResourceCache
{
    public static readonly LFUCache<string, UnityEngine.Object> UICache;
    public static readonly LFUCache<string, UnityEngine.Object> BattleCache;

    static ResourceCache()
    {
        UICache = new LFUCache<string, UnityEngine.Object>(
            SystemConst.ResourceCache.UI_CACHE_MAX_COUNT,
            SystemConst.ResourceCache.UI_CACHE_MAX_MEMORY_BYTES,
            EstimateMemorySize);

        BattleCache = new LFUCache<string, UnityEngine.Object>(
            SystemConst.ResourceCache.BATTLE_CACHE_MAX_COUNT,
            SystemConst.ResourceCache.BATTLE_CACHE_MAX_MEMORY_BYTES,
            EstimateMemorySize);
    }

    public static T LoadUI<T>(string path) where T : UnityEngine.Object
    {
        var cached = UICache.Get(path);
        if (cached != null)
        {
            return cached as T;
        }

        var resource = Resources.Load<T>(path);
        if (resource != null)
        {
            UICache.Add(path, resource);
        }
        return resource;
    }

    public static T LoadBattle<T>(string path) where T : UnityEngine.Object
    {
        var cached = BattleCache.Get(path);
        if (cached != null)
        {
            return cached as T;
        }

        var resource = Resources.Load<T>(path);
        if (resource != null)
        {
            BattleCache.Add(path, resource);
        }
        return resource;
    }

    public static GameObject LoadPrefabUI(string path)
    {
        return LoadUI<GameObject>(path);
    }

    public static GameObject LoadPrefabBattle(string path)
    {
        return LoadBattle<GameObject>(path);
    }

    public static Sprite LoadSpriteUI(string path)
    {
        return LoadUI<Sprite>(path);
    }

    public static Sprite LoadSpriteBattle(string path)
    {
        return LoadBattle<Sprite>(path);
    }

    public static void ClearUICache()
    {
        UICache.Clear();
    }

    public static void ClearBattleCache()
    {
        BattleCache.Clear();
    }

    public static void ClearAll()
    {
        UICache.Clear();
        BattleCache.Clear();
    }

    public static (CacheStats uiStats, CacheStats battleStats) GetStats()
    {
        return (UICache.GetStats(), BattleCache.GetStats());
    }

    private static long EstimateMemorySize(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return 0;
        }

        if (obj is Texture texture)
        {
            return texture.width * texture.height * GetTextureBytesPerPixel(texture);
        }

        if (obj is Mesh mesh)
        {
            return mesh.vertexCount * 12 + mesh.triangles.Length * 4;
        }

        if (obj is AudioClip audioClip)
        {
            return audioClip.samples * audioClip.channels * 2;
        }

        return IntPtr.Size * 2;
    }

    private static int GetTextureBytesPerPixel(Texture texture)
    {
        if (texture is Texture2D tex2D)
        {
            switch (tex2D.format)
            {
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                    return 4;
                case TextureFormat.RGB24:
                    return 3;
                case TextureFormat.R16:
                    return 2;
                case TextureFormat.R8:
                    return 1;
                default:
                    return 4;
            }
        }
        return 4;
    }
}
