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

            RequestReturnGame();

        }
    }


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


    private void RequestReturnGame()
    {
        // 客户端向服务器发送增加死亡计数的请求
        IncreaseDiedCountServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreaseDiedCountServerRpc()
    {
        // 服务器安全地修改死亡计数
        DiedNumber.Value++;

        // 在服务器端检查游戏结束条件
        CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        int NumberOfPlayers = 2; // 应改为动态获取玩家数

        if (DiedNumber.Value >= NumberOfPlayers - 1)
        {
            // 通知所有客户端重启游戏
            RestartGameClientRpc();
        }
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        // 重置光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 关闭网络连接
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            yield return new WaitUntil(() => !NetworkManager.Singleton.IsListening);
        }

        // 重新加载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

       
    }

}


