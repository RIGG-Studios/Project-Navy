using UnityEngine;

//Handles weapon juice
public abstract class WeaponJuiceBase : ScriptableObject
{
    public virtual void Update(){return;}
    public virtual void Init(){return;}
    public virtual void CleanUp(){return;}
    
    public abstract void Execute();
}