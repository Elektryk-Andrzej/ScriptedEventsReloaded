using System;
using System.Collections;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SER.Helpers.Exceptions;

namespace SER.ValueSystem;

public abstract class Value
{
    public static Value Parse(object obj)
    {
        if (obj is null) throw new AndrzejFuckedUpException();
        if (obj is Value v) return v;
        
        return obj switch
        {     
            bool b     => new BoolValue(b),
            byte n     => new NumberValue(n),
            sbyte n    => new NumberValue(n),
            short n    => new NumberValue(n),
            ushort n   => new NumberValue(n),
            int n      => new NumberValue(n),
            uint n     => new NumberValue(n),
            long n     => new NumberValue(n),
            ulong n    => new NumberValue(n),
            float n    => new NumberValue((decimal)n),
            double n   => new NumberValue((decimal)n),
            decimal n  => new NumberValue(n),
            string s   => new TextValue(s),
            TimeSpan t => new DurationValue(t),
            Player p   => new PlayerValue(p),
            IEnumerable<Player> ps => new PlayerValue(ps),
            IEnumerable e => new CollectionValue(e),
            _             => new ReferenceValue(obj),
        };
    }
    
    public static string FriendlyName(Type type) => type.Name.Replace("Value", "").ToLower();
    public string FriendlyName() => FriendlyName(GetType());
}