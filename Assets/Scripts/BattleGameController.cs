using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleGameController : NetworkBehaviour
{
    int MinimumPlayers = 2;

    public GameObject OverPanel; // 结算面板

    public NetworkVariable<int> DiedNumber = new NetworkVariable<int>(
        0,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server  // 
    );


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
    
    [ServerRpc(RequireOwnership = false)]
    public void ReportPlayerDeathServerRpc()
    {
        DiedNumber.Value++;
        GameOver();
    }

    void GameOver()
    {
        if (DiedNumber.Value >= MinimumPlayers - 1)
        {
            EndGameClientRpc();
        }
    }

    [ClientRpc]
    private void EndGameClientRpc()
    {
        Time.timeScale = 0;

        //隐藏游戏面板，显示结算面板
        OverPanel.SetActive(true);

        DisablePlayer();
    }

    void DisablePlayer()
    {
        foreach (var movement in FindObjectsOfType<PlayerMovement>())
        {
            movement.enabled = false;//禁用移动视角脚本
        }

        foreach (var Attacking in FindObjectsOfType<PlayerAttack>())
        {
            Attacking.enabled = false;//禁用开枪脚本 
        }
    }

    [ClientRpc]
    public void ReturnGameClientRpc()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
