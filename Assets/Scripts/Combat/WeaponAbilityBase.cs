using UnityEngine;

//Handles weapon abilities
public abstract class WeaponAbilityBase : ScriptableObject
{
    public virtual void Update(){return;}
    public virtual void Init(){return;}
    public virtual void CleanUp(){return;}

    public abstract void Execute(PlayerStats stats);
}
