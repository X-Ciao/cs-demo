using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartManager : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if(!NetworkManager.Singleton.IsClient  && !NetworkManager.Singleton.IsServer)
        {
            ShowStartButtons();
            GUILayout.EndArea();
        }
        else
        {
            ShowConnectedLabels();
            GUILayout.EndArea();
        }
    }

    static void ShowStartButtons()
    {
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void ShowConnectedLabels()
    {
        if(GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        {
            if(NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkClient))
            {

                //var player = networkClient.PlayerObject.GetComponent<StarPlayer>();
                //if (player)
                //{
                //    player.move();
                //}

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
