using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ConnectingMenu : MenuBase
{
    private void Awake()
    {
        NetworkConnection.Instance.ConnectToPhoton();
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            OnConnected();
        }
    }

    private void OnConnected()
    {
        MainMenu.OpenMenuByName("UsernameSelectionMenu");
    }
}
