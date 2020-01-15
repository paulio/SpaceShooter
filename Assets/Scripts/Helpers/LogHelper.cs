using UnityEngine;

public static class LogHelper 
{
    public static void CheckForNull(object target, string nameOfObject)
    {
        if (target == null)
        {
            Debug.LogError($"{nameOfObject} is null");
        }
    }
}
