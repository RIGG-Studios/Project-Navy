using UnityEngine;


[CreateAssetMenu]
public class WavePreset : ScriptableObject
{
    //shader uses 3 gerstner wave functions so we have to send in 3 sets
    public WaveSettings[] waves = new WaveSettings[3];
}
