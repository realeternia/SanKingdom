using System;
using System.Collections.Generic;
using UnityEngine;

public class CacheStats
{
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public int CacheCount { get; set; }
    public long MemoryUsed { get; set; }
}

public class LFUCache<TKey, TValue>
{
    private class CacheEntry
    {
        public TValue Value;
        public int Frequency;
        public long MemorySize;
    }

    private readonly Dictionary<TKey, CacheEntry> _cache;
    private readonly Dictionary<int, LinkedList<TKey>> _frequencyLists;
    private readonly Dictionary<TKey, LinkedListNode<TKey>> _nodeMap;
    private readonly int _maxCount;
    private readonly long _maxMemoryBytes;
    private readonly Func<TValue, long> _memorySizeEstimator;
    private int _minFrequency;
    private long _memoryUsed;
    private long _hitCount;
    private long _missCount;

    public int Count => _cache.Count;
    public long MemoryUsed => _memoryUsed;

    public LFUCache(int maxCount, long maxMemoryBytes = long.MaxValue, Func<TValue, long> memorySizeEstimator = null)
    {
        _cache = new Dictionary<TKey, CacheEntry>();
        _frequencyLists = new Dictionary<int, LinkedList<TKey>>();
        _nodeMap = new Dictionary<TKey, LinkedListNode<TKey>>();
        _maxCount = maxCount;
        _maxMemoryBytes = maxMemoryBytes;
        _memorySizeEstimator = memorySizeEstimator;
        _minFrequency = 0;
        _memoryUsed = 0;
        _hitCount = 0;
        _missCount = 0;
    }

    public TValue Get(TKey key)
    {
        if (!_cache.TryGetValue(key, out var entry))
        {
            _missCount++;
            return default(TValue);
        }

        _hitCount++;
        IncreaseFrequency(key, entry);
        return entry.Value;
    }

    public void Add(TKey key, TValue value)
    {
        if (_cache.ContainsKey(key))
        {
            UpdateExistingEntry(key, value);
            return;
        }

        long memorySize = EstimateMemorySize(value);

        while (ShouldEvict(memorySize))
        {
            EvictOne();
        }

        CreateNewEntry(key, value, memorySize);
    }

    public bool Remove(TKey key)
    {
        if (!_cache.TryGetValue(key, out var entry))
        {
            return false;
        }

        RemoveFromFrequencyList(key, entry.Frequency);
        _cache.Remove(key);
        _nodeMap.Remove(key);
        _memoryUsed -= entry.MemorySize;

        UpdateMinFrequencyAfterRemoval();

        return true;
    }

    public void Clear()
    {
        _cache.Clear();
        _frequencyLists.Clear();
        _nodeMap.Clear();
        _minFrequency = 0;
        _memoryUsed = 0;
    }

    public CacheStats GetStats()
    {
        return new CacheStats
        {
            HitCount = _hitCount,
            MissCount = _missCount,
            CacheCount = _cache.Count,
            MemoryUsed = _memoryUsed
        };
    }

    private void IncreaseFrequency(TKey key, CacheEntry entry)
    {
        int oldFreq = entry.Frequency;
        int newFreq = oldFreq + 1;

        RemoveFromFrequencyList(key, oldFreq);

        entry.Frequency = newFreq;
        AddToFrequencyList(key, newFreq);

        if (oldFreq == _minFrequency && !HasFrequencyList(_minFrequency))
        {
            _minFrequency = newFreq;
        }
    }

    private void UpdateExistingEntry(TKey key, TValue value)
    {
        var entry = _cache[key];
        long oldMemorySize = entry.MemorySize;
        long newMemorySize = EstimateMemorySize(value);

        _memoryUsed -= oldMemorySize;

        while (ShouldEvict(newMemorySize) && _cache.Count > 1)
        {
            EvictOne();
            if (_cache.ContainsKey(key))
            {
                entry = _cache[key];
                oldMemorySize = entry.MemorySize;
                _memoryUsed -= oldMemorySize;
            }
            else
            {
                CreateNewEntry(key, value, newMemorySize);
                return;
            }
        }

        entry.Value = value;
        entry.MemorySize = newMemorySize;
        _memoryUsed += newMemorySize;

        IncreaseFrequency(key, entry);
    }

    private void CreateNewEntry(TKey key, TValue value, long memorySize)
    {
        var entry = new CacheEntry
        {
            Value = value,
            Frequency = 1,
            MemorySize = memorySize
        };

        _cache[key] = entry;
        _memoryUsed += memorySize;

        AddToFrequencyList(key, 1);
        _minFrequency = 1;
    }

    private bool ShouldEvict(long newMemorySize)
    {
        if (_cache.Count >= _maxCount)
        {
            return true;
        }

        if (_maxMemoryBytes != long.MaxValue && _memoryUsed + newMemorySize > _maxMemoryBytes)
        {
            return _cache.Count > 0;
        }

        return false;
    }

    private void EvictOne()
    {
        if (!_frequencyLists.TryGetValue(_minFrequency, out var list) || list.Count == 0)
        {
            UpdateMinFrequencyAfterRemoval();
            return;
        }

        TKey keyToEvict = list.First.Value;
        Remove(keyToEvict);

        Debug.LogWarning($"LFUCache: Evicted key '{keyToEvict}' at frequency {_minFrequency}");
    }

    private void RemoveFromFrequencyList(TKey key, int frequency)
    {
        if (_frequencyLists.TryGetValue(frequency, out var list))
        {
            if (_nodeMap.TryGetValue(key, out var node))
            {
                list.Remove(node);
                _nodeMap.Remove(key);

                if (list.Count == 0)
                {
                    _frequencyLists.Remove(frequency);
                }
            }
        }
    }

    private void AddToFrequencyList(TKey key, int frequency)
    {
        if (!_frequencyLists.ContainsKey(frequency))
        {
            _frequencyLists[frequency] = new LinkedList<TKey>();
        }

        LinkedListNode<TKey> node = _frequencyLists[frequency].AddLast(key);
        _nodeMap[key] = node;
    }

    private bool HasFrequencyList(int frequency)
    {
        return _frequencyLists.ContainsKey(frequency) && _frequencyLists[frequency].Count > 0;
    }

    private void UpdateMinFrequencyAfterRemoval()
    {
        if (_cache.Count == 0)
        {
            _minFrequency = 0;
            return;
        }

        if (!HasFrequencyList(_minFrequency))
        {
            _minFrequency = FindMinFrequency();
        }
    }

    private int FindMinFrequency()
    {
        int minFreq = int.MaxValue;
        foreach (var freq in _frequencyLists.Keys)
        {
            if (freq < minFreq && _frequencyLists[freq].Count > 0)
            {
                minFreq = freq;
            }
        }
        return minFreq == int.MaxValue ? 1 : minFreq;
    }

    private long EstimateMemorySize(TValue value)
    {
        if (_memorySizeEstimator != null)
        {
            return _memorySizeEstimator(value);
        }

        return EstimateDefaultMemorySize(value);
    }

    private long EstimateDefaultMemorySize(TValue value)
    {
        if (value == null)
        {
            return 0;
        }

        Type type = typeof(TValue);

        if (type.IsValueType)
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(type);
        }

        if (type == typeof(string))
        {
            return (value as string)?.Length * sizeof(char) ?? 0;
        }

        if (value is UnityEngine.Object unityObj)
        {
            return EstimateUnityObjectMemory(unityObj);
        }

        return IntPtr.Size * 2;
    }

    private long EstimateUnityObjectMemory(UnityEngine.Object obj)
    {
        if (obj is Texture2D texture2D)
        {
            return texture2D.width * texture2D.height * (texture2D.format == TextureFormat.RGBA32 ? 4 : 1);
        }

        if (obj is Texture texture)
        {
            return texture.width * texture.height * 4;
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
}
