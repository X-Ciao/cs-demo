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
           
        }
        
        GUILayout.EndArea();
    }

    static void ShowStartButtons()
    {
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
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
