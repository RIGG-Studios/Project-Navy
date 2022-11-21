using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static void SafeDestroy(params UnityEngine.Object[] components)
    {
        if (!Application.isPlaying)
        {
            foreach (var component in components)
            {
                if (component != null) UnityEngine.Object.DestroyImmediate(component);
            }

        }
        else
        {
            foreach (var component in components)
            {
                if (component != null) UnityEngine.Object.Destroy(component);
            }
        }
    }
}
