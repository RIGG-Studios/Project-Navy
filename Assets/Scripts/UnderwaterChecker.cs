using System;
using UnityEngine;

public class UnderwaterChecker : MonoBehaviour
{
    [SerializeField] private Transform waterHeightChecker;
    [SerializeField] private float maxDiff;
    
    
    public bool IsUnderWater()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(waterHeightChecker.position);
        float diff = waterHeightChecker.position.y - waterHeight;
        
        if (diff <= maxDiff)
        {
            return true;
        }

        return false;
    }
}
