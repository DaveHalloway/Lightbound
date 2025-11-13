using UnityEngine;
using System;

public static class WorldShiftEvents
{
    public static Action<bool> OnWorldShift; // true = day, false = night

    public static void Invoke(bool isDay)
    {
        OnWorldShift?.Invoke(isDay);
    }
}
