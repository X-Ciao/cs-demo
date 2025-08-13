using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // 玩家预设体的标签
    public string playerTag = "Player";

    // 碰撞检测方法
    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞对象是否是玩家
        if (collision.gameObject.CompareTag(playerTag))
        {
            var playerHitHealthScript = collision.collider.GetComponent<PlayerHealth>();

            if (playerHitHealthScript != null)
            {

                float reduceHealthBy = 10f;

                playerHitHealthScript.ReduceHealth(reduceHealthBy);
            }


            // 销毁子弹
            Destroy(gameObject);
        }
    }
}
