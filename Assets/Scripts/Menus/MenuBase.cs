using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MenuBase : MonoBehaviour
{ 
    public string menuName;
    
    public bool IsOpen { get; private set; }

    protected MainMenu MainMenu;
    
    public void SetupMenu(MainMenu mainMenu)
    {
        MainMenu = mainMenu;
    }

    public virtual void OpenMenu()
    {
        IsOpen = true;
        gameObject.SetActive(true);
    }

    public virtual void CloseMenu()
    {
        IsOpen = false;
        gameObject.SetActive(false);
    }
}
