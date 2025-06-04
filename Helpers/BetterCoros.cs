using System;
using System.Collections.Generic;
using MEC;
using SER.Helpers.Exceptions;
using SER.ScriptSystem;

namespace SER.Helpers;

public static class BetterCoros
{
    public static CoroutineHandle Run(this IEnumerator<float> coro, Script scr, Action<Exception>? onException = null)
    {
        return Timing.RunCoroutine(Wrapper(coro, scr, onException));
    }

    public static void Kill(this CoroutineHandle coro)
    {
        Timing.KillCoroutines(coro);
    }

    private static IEnumerator<float> Wrapper(IEnumerator<float> routine, Script scr, Action<Exception>? onException = null)
    {
        while (true)
        {
            try
            {
                if (!routine.MoveNext()) break;
            }
            catch (ArgumentFetchException ex)
            {
                Log.Error(scr, ex.Message);
                yield break;
            }
            catch (Exception ex)
            {
                Log.Error(scr, $"Coroutine failed with {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                onException?.Invoke(ex);
                yield break;
            }

            yield return routine.Current;
        }
    }
    
    public static IEnumerator<float> SlowWaitUntilTrue(Func<bool> condition)
    {
        while (true)
        {
            if (condition())
                break;

            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;
        }
    }
}