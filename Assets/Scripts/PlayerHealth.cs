using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<float> health = new NetworkVariable<float>(
        100f,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server  // 
    );

    MeshRenderer[] renderers;

    private BattleGameController battleGameController;

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void ReduceHealth(float damage)
    {
        health.Value -= damage;

        if (health.Value < 1)//当玩家血量归零时
        {
            PlayerDiedClientRpc();//使玩家预设体隐身

            ReturnGameClientRpc();

            //DisableAttack(); //禁用玩家射击函数

            //PlayerDied();//向游戏管理器报告死亡
        }
    }

    //void PlayerDied()
    //{
    //    if (IsOwner)
    //    {
    //        battleGameController.ReportPlayerDeathServerRpc();
    //    }
    //}

    //void DisableAttack()
    //{
    //    PlayerAttack attackingScript = GetComponent<PlayerAttack>();

    //    attackingScript.enabled = false;
    //}

    [ClientRpc]
    void PlayerDiedClientRpc()
    {

        foreach(var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    public NetworkVariable<int> DiedNumber = new NetworkVariable<int>(
    0,
    readPerm: NetworkVariableReadPermission.Everyone,
    writePerm: NetworkVariableWritePermission.Server  
);


    [ClientRpc]
    public void ReturnGameClientRpc()
    {
        DiedNumber.Value++;

        int NumberOfPlayers = 2;
        if (DiedNumber.Value >= NumberOfPlayers - 1)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // 解锁鼠标用于UI操作
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
       
    }

}
