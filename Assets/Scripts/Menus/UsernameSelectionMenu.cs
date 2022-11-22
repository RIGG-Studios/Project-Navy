using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UsernameSelectionMenu : MenuBase
{
    [SerializeField] private InputField usernameInputField;
    
    public void SetUsername()
    {
        string userName = usernameInputField.text;

        if (userName != String.Empty)
        {
            PhotonNetwork.LocalPlayer.NickName = userName;
        }
        
        MainMenu.OpenMenuByName("MainMenu");
    }
}
