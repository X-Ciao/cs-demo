using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class StartPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> NetPosition = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server // 
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer && NetPosition.Value == default)
        {
            Move(); // 首次生成时移动到随机位置
        }
    }

    public void Move()
    {
        if (IsServer)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane();

            // 服务器直接更新transform位置
            transform.position = randomPosition;

            // 更新网络变量触发同步
            NetPosition.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        NetPosition.Value = GetRandomPositionOnPlane();
    }


    private Vector3 GetRandomPositionOnPlane()
    {

        float xPosition = Random.Range(41, 54);
        float yPosition = 2f;  // Y轴保持不变
        float zPosition = Random.Range(53, 68);

        return new Vector3(xPosition, yPosition, zPosition);
    }



    // Update is called once per frame
    void Update()
    {
        transform.position = NetPosition.Value;
    }
}
