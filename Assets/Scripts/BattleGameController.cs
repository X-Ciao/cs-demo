using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleGameController : NetworkBehaviour
{
    int MinimumPlayers = 2;






    public void Start()
    {
        
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
