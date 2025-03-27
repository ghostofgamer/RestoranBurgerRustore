using System;
using System.Collections.Generic;
using UnityEngine;

// todo потом можно посмотреть, можно ли сделать свою полноценную структуру данных, чтобы доступ к элементам осуществлялся через [] скобки (а не через Get)

/// <summary>
/// Представляет упрощенный функционал Dictionary, доступный для Unity Inspector
/// </summary>
[Serializable]
public class UnityDictionary<TKey, TValue>
{
    [SerializeField] private List<UnityDictionaryKeyValue> keyValues;
    public List<UnityDictionaryKeyValue> KeyValues => keyValues;

    public int Count => keyValues.Count;

    public UnityDictionary()
    {
        keyValues = new List<UnityDictionaryKeyValue>();
    }

    public UnityDictionary(UnityDictionary<TKey, TValue> baseDictionary)
    {
        keyValues = new List<UnityDictionaryKeyValue>();
        foreach (var keyValue in baseDictionary.keyValues) keyValues.Add(keyValue);
    }

    public void Add(TKey key, TValue value)
    {
        keyValues.Add(new UnityDictionaryKeyValue(key, value));
    }

    public bool ContainsKey(TKey key)
    {
        for (int i = 0; i < keyValues.Count; i++)
        {
            if (keyValues[i].Key.Equals(key)) return true;
        }

        return false;
    }

    public TValue Get(TKey key)
    {
        for (int i = 0; i < keyValues.Count; i++)
        {
            if (keyValues[i].Key.Equals(key)) return keyValues[i].Value;
        }

        Debug.LogError("Попытка получить значение по несуществующему ключу");

        return keyValues[-1].Value;
    }

    public void Set(TKey key, TValue value)
    {
        for (int i = 0; i < keyValues.Count; i++)
        {
            if (keyValues[i].Key.Equals(key))
            {
                keyValues[i].Value = value;
                return;
            }
        }

        Add(key, value);
    }

    [Serializable]
    public class UnityDictionaryKeyValue
    {
        [SerializeField] private TKey key;
        [SerializeField] private TValue value;

        public TKey Key => key;
        public TValue Value
        {
            get => value;
            set => this.value = value;
        }

        public UnityDictionaryKeyValue()
        {
            key = default;
            value = default;
        }

        public UnityDictionaryKeyValue(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> result = new();
        foreach (var element in keyValues)
        {
            if (result.ContainsKey(element.Key)) continue;
            result.Add(element.Key, element.Value);
        }
        return result;
    }
}