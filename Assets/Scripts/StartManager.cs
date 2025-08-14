using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if(!NetworkManager.Singleton.IsClient  && !NetworkManager.Singleton.IsServer)
        {
            ShowStartButtons();
           
        }
        
        GUILayout.EndArea();
    }

    static void ShowStartButtons()
    {
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        // 返回按钮
        if (GUILayout.Button("Return"))
        {
            // 停止网络连接
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // 跳转到开始场景
            SceneManager.LoadScene("Start Scenes");
        }
    }




}
