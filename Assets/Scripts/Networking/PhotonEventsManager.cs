using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class NetworkPlayer
{
    public string playerName;
    public int actorID;
    public int viewID;

    public short kills;
    public short deaths;
    
    public PhotonView playerPhotonView => PhotonView.Find(viewID);

    public NetworkPlayer(string playerName, int actorID, int viewID, short kills, short deaths)
    {
        this.playerName = playerName;
        this.actorID = actorID;
        this.viewID = viewID;
        this.kills = kills;
        this.deaths = deaths;
    }
}

public enum EventCodes : byte
{
    AddPlayer = 0,
    UpdatePlayers = 2,
    RemovePlayer = 6,
    ChangePlayerStats = 4,
    DamageEntity = 5,
    StartMatch = 7
}

public class PhotonEventsManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static PhotonEventsManager Instance;

    public List<NetworkPlayer> players = new List<NetworkPlayer>();

    public NetworkPlayer LocalPlayer => players[_localIndex];

    private int _localIndex;
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        
        PhotonNetwork.AddCallbackTarget(false);
    }
    
    public void RaiseEvent(EventCodes code, ReceiverGroup receiverGroup, object[] args)
    {
        object[] package = Utility.BuildPackage(code, args);

        RaiseEventOptions eventOptions = new RaiseEventOptions()
        {
            Receivers = receiverGroup
        };

        SendOptions sendOptions = new SendOptions()
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent((byte)code, package, eventOptions, sendOptions);
    }


    public void OnEvent(EventData photonEvent)
    {
        EventCodes code = (EventCodes)photonEvent.Code;
        object[] o = photonEvent.Code >= 200 ? null : (object[])photonEvent.CustomData;
        
        switch (code)
        {
            case EventCodes.AddPlayer:
                AddNewPlayer(o);
                break;
            
            case EventCodes.UpdatePlayers:
                UpdatePlayers(o);
                break;
            
            case EventCodes.ChangePlayerStats:
                UpdatePlayerStats(o);
                break;
            
            case EventCodes.DamageEntity:
                DamageEntity(o);
                break;
        }
    }

    private void AddNewPlayer(object[] package)
    {
        string playerName = (string)package[0];
        int actorID = (int)package[1];
        short kills = (short)package[2];
        short deaths = (short)package[3];
        int viewID = (int)package[4];

        NetworkPlayer player = new NetworkPlayer(playerName, actorID, viewID, kills, deaths);
        
        players.Add(player);

        object[] updatedPackage = new object[]
        {
            players
        };
        
        RaiseEvent(EventCodes.UpdatePlayers, ReceiverGroup.All, updatedPackage);
    }

    private void UpdatePlayers(object[] package)
    {
        players = new List<NetworkPlayer>();

        for (int i = 1; i < package.Length; i++)
        {
            object[] extract = (object[])package[i];
            
            string playerName = (string)extract[0];
            int actorID = (int)extract[1];
            short kills = (short)extract[2];
            short deaths = (short)extract[3];
            int viewID = (int)extract[4];

            NetworkPlayer player = new NetworkPlayer(playerName, actorID, viewID, kills, deaths);
            players.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actorID)
            {
                _localIndex = i - 1;
            }
        }
    }

    private void UpdatePlayerStats(object[] package)
    {
        int actor = (int)package[0];
        int stat = (int)package[1];
        int amount = (int)package[2];

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].actorID == actor)
            {
                switch (stat)
                {
                    case 0: //kills
                        players[i].kills += (byte)amount;
                        break;
                    
                    case 1: //deaths
                        players[i].deaths += (byte)amount;
                        break;
                    
                    case 2: //view id
                        players[i].viewID = amount;
                        break;
                }
            }
        }
    }

    private void DamageEntity(object[] package)
    {
        
    }

    public NetworkPlayer FindPlayerByActorID(int actorID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (actorID == players[i].actorID)
            {
                return players[i];
            }
        }

        return null;
    }
}
