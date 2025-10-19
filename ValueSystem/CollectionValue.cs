using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers.ResultSystem;
using SER.VariableSystem.Variables;

namespace SER.ValueSystem;

public class CollectionValue(IEnumerable value) : LiteralValue<IEnumerable>(value)
{
    public LiteralValue[] CastedValues
    {
        get
        {
            if (field is not null) return field;
            
            List<LiteralValue> list = [];
            list.AddRange(from object item in Value select ParseFromObject(item));
            return field = list.ToArray();
        }
    } = null!;
    
    public TryGet<LiteralValue> GetAt(int index)
    {
        if (index < 1) return $"Provided index {index}, but index cannot be less than 1";
        
        try
        {
            return CastedValues[index - 1];
        }
        catch (ArgumentOutOfRangeException)
        {
            return $"There is no value at index {index}";
        }
    }

    protected override string StringRep
    {
        get
        {
            List<string> objects = [];
            objects.AddRange(from object? obj in CastedValues select obj?.ToString() ?? "???");
            return $"[{string.Join(", ", objects)}]";
        }
    }
}