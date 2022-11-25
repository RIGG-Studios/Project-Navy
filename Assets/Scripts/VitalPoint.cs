using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private Player player;
    
    public int ActorID => player.ActorID;
    
    public void Damage(int attackerID, float damageAmount)
    {
        player.Damage(attackerID, damageAmount);
    }
}
