using System;
using UnityEngine;
using UnityEngine.UI;

public class UnderwaterChecker : MonoBehaviour
{
    [SerializeField] private Transform waterHeightChecker;

    
    public bool IsUnderWater()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(waterHeightChecker.position);

        if (waterHeightChecker.transform.position.y < waterHeight)
        {
            return true;
        }

        return false;
    }
}
