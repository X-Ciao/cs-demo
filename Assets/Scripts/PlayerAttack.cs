using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //关联火焰点
    public Transform FirePoint;
    //关联火焰预设体
    public GameObject FirePre;
    //关联子弹点
    public Transform BulletPoint;
    //关联子弹预设体
    public GameObject BulletPre;



    //开火间隔
    private float cd = 0.2f;
    //计时器
    private float timer = 0;

    public float bulletForce = 800f; // 子弹发射力度
    public float bulletLifeTime = 2f; // 子弹生存时间（秒）




    bool attacking = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        attacking = Input.GetMouseButton(0);

        //计时
        timer += Time.unscaledDeltaTime;

        if (attacking && timer > cd)
        {
            Attack();
        }
    }


    void Attack()
    {
        //重置计时器
        timer = 0;

        // 实例化子弹
        GameObject bullet = Instantiate(BulletPre, BulletPoint.position, BulletPoint.rotation);

        // 获取子弹刚体组件
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            // 添加向前的力
            bulletRb.AddForce(BulletPoint.forward * bulletForce);
        }

        // 设置子弹在2秒后销毁
        Destroy(bullet, bulletLifeTime);

    }



}
