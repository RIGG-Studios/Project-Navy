using System;
using UnityEngine;

public class UnderWaterCamera : MonoBehaviour
{
    [SerializeField] private GameObject underwaterPostProcessing;
    [SerializeField] private UnderwaterChecker underwaterChecker;
    
    
    private void Update()
    {
        bool underWater = underwaterChecker.IsUnderWater();
        
        underwaterPostProcessing.SetActive(underWater);
    }
}
