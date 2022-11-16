using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class UnderWaterCamera : MonoBehaviour
{
    [SerializeField] private GameObject defaultPostProcessing;
    [SerializeField] private GameObject underwaterPostProcessing;
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private float maxDiff;
    
    private void Update()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(transform.position, Time.time);

        float diff = (transform.position.y - waterHeight);

        depthText.text = diff.ToString(CultureInfo.InvariantCulture);
        
        if (transform.position.y < waterHeight)
        {
            underwaterPostProcessing.SetActive(true);
            defaultPostProcessing.SetActive(false);
        }
        else
        {
            underwaterPostProcessing.SetActive(false);
            defaultPostProcessing.SetActive(true);
        }
    }
}
