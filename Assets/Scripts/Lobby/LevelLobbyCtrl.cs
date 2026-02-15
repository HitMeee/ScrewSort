using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLobbyCtrl : MonoBehaviour
{
    public GameObject panelAllLevel;
    public Button btnShowAllLevel;
    public Button btnCloseAllLevel;
    public void Start()
    {
        if (btnShowAllLevel != null)
        {
            btnShowAllLevel.onClick.AddListener(ShowAllLevel);
        }
        if (btnCloseAllLevel != null)
        {
            btnCloseAllLevel.onClick.AddListener(CloseAllLevel);
        }
    }
    public void ShowAllLevel()
    {
        if (panelAllLevel != null)
        {
            panelAllLevel.SetActive(true);
        }
    }
    public void CloseAllLevel()
    {
        if (panelAllLevel != null)
        {
            panelAllLevel.SetActive(false);
        }
    }
    
}
