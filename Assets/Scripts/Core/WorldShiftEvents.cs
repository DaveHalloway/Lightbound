using UnityEngine;
using System;

public class WorldShiftEvents
{
    // Event that broadcasts whenever the world changes
    public static Action<bool> OnWorldShift; // true = day, false = night

    public static void Invoke(bool isDay)
    {
        OnWorldShift?.Invoke(isDay);
    }
}
