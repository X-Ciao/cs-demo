using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleGameController : NetworkBehaviour
{
    int MinimumPlayers = 2;

    public GameObject OverPanel; // 结算面板




    public void Start()
    {
        //隐藏结算界面
        if (OverPanel != null)
            OverPanel.SetActive(false);
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length < MinimumPlayers)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        
    }
    
    
}
