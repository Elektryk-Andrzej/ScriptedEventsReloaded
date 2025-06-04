using System.Collections.Generic;

namespace SER.Helpers.Extensions;

public static class DictionaryExtensions
{
    public static void AddOrInitListWithKey<TKey, TCollection, TCollectionValue>(
        this Dictionary<TKey, TCollection> dictionary, 
        TKey key, 
        TCollectionValue value
    ) where TCollection : List<TCollectionValue>, new()
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key].Add(value);
        }
        else
        {
            dictionary[key] = [value];
        }
    }
}