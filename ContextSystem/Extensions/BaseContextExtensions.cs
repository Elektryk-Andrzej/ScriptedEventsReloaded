﻿using System.Collections.Generic;
using SER.ContextSystem.BaseContexts;
using SER.Helpers;
using SER.Helpers.Exceptions;

namespace SER.ContextSystem.Extensions;

public static class BaseContextExtensions
{
    public static IEnumerator<float> ExecuteBaseContext(this Context context)
    {
        Log.Debug($"Executing context {context.GetType().Name}");
        switch (context)
        {
            case StandardContext standardContext:
                standardContext.Run();
                yield break;
            case YieldingContext yieldingContext:
                var coro = yieldingContext.Run();
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }
                
                yield break;
            default:
                throw new AndrzejFuckedUpException("context is not standard nor yielding");
        }
    }
}