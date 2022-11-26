using UnityEngine;

public class UnderwaterChecker : MonoBehaviour
{
    [SerializeField] private Transform waterHeightChecker;
    
    public bool IsUnderWater()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(waterHeightChecker.position);
        
        if (waterHeightChecker.position.y < waterHeight)
        {
            return true;
        }

        return false;
    }

    public float GetSubmergence()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(waterHeightChecker.position);
        float submergence = waterHeightChecker.position.y - waterHeight;
        
        return submergence;
    }
}
