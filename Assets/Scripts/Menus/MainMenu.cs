using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private List<MenuBase> _menus = new List<MenuBase>();

    private MenuBase _currentMenu;
    
    private void Awake()
    {
        MenuBase[] menus = GetComponentsInChildren<MenuBase>(true);

        if (menus.Length > 0)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                _menus.Add(menus[i]);
                menus[i].SetupMenu(this);
            }
        }
    }

    private void Start()
    {
        OpenMenuByName("ConnectingMenu");
    }

    public void OpenMenuByName(string name)
    {
        MenuBase nextMenu = FindMenuByName(name);

        if (nextMenu == null) return;

        if (_currentMenu != null)
        {
            _currentMenu.CloseMenu();
        }
        
        _currentMenu = nextMenu;
        _currentMenu.OpenMenu();
    }

    private MenuBase FindMenuByName(string menuName)
    {
        for (int i = 0; i < _menus.Count; i++)
        {
            if (_menus[i].menuName == menuName)
            {
                return _menus[i];
            }
        }

        return null;
    }
}
