using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class UnderWaterCamera : MonoBehaviour
{
    [SerializeField] private GameObject underwaterPostProcessing;
    [SerializeField] private float maxDiff;
    
    private void Update()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(transform.position);

        float diff = (transform.position.y - waterHeight);
        
        if (diff <= maxDiff)
        {
            underwaterPostProcessing.SetActive(true);
        }
        else
        {
            underwaterPostProcessing.SetActive(false);
        }
    }
}
