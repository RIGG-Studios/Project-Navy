using Photon.Realtime;
using UnityEngine;

public static class PhotonDamageHandler
{
    public static void SendDamageRequest(int attackerID, int victimID, float damage)
    {
        object[] package =
        {
            attackerID,
            victimID,
            damage
        };

        PhotonEventsManager.Instance.RaiseEvent(EventCodes.DamageEntity, ReceiverGroup.All, package);
    }

    public static void RecieveDamageEvent(int attackerID, int victimID, float damage)
    {
        NetworkPlayer victim = PhotonEventsManager.Instance.FindPlayerByActorID(victimID);
        NetworkPlayer attacker = PhotonEventsManager.Instance.FindPlayerByActorID(attackerID);

        if (victim != null)
        {
            victim.playerPhotonView.GetComponent<Player>().Damage(attackerID, damage);
            attacker.playerPhotonView.RPC("OnPlayerDamagedOther", attacker.playerPhotonView.Owner, victimID);
        }
        else
        {
            Debug.Log("Error finding victim network player");
        }
    }
}
