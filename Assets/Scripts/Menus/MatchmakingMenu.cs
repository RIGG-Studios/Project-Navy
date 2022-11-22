using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingMenu : MenuBase
{
    public override void OpenMenu()
    {
        base.OpenMenu();
        
        MatchMaking.Instance.CreateSearch();
    }
}
