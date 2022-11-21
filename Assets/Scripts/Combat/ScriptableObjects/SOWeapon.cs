using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]

//A weapon, will be configurable via the inspector and have custom editor scripts
//All ability variables will be displayed here probably via an editor script
public class SOWeapon : ScriptableObject
{
    public WeaponAbilityBase[] abilities;
    //Temporary, will make a struct with input actions and abilities once we switch to the new input system
    public KeyCode[] binds;
}