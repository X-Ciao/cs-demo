using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update

    //刚体
    private Rigidbody rBody;
    //音频组件
    private AudioSource footPlayer;
    //是否在地面
    private bool isGround;
   
    
    void Start()
    {
        //获取刚体组件
        rBody = GetComponent<Rigidbody>();
        //获取声音组件
        footPlayer = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //如果按下空格键
        if(Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            //跳跃：给刚体一个向上的力
            rBody.AddForce(Vector3.up * 200);
        }

        //是否按下移动键
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //移动，按下移动键，并且当前没有播放声音，并且角色处于地面上
        if ((horizontal != 0 || vertical !=0) && isGround == true)
        {
            //播放脚步声
            if (footPlayer.isPlaying == false)
            {
                footPlayer.Play();
            }
        }
        else
        {
            //没有移动，没有按下移动键，停止脚步声
            footPlayer.Stop();
        }

    }
   

    //产生碰撞
    private void OnCollisionEnter(Collision collision)
    {
        //判断是不是地面
        if(collision.collider.tag == "Ground")
        {
            //踩在地面上
            isGround = true;
        }
    }

    //结束碰撞
    private void OnCollisionExit(Collision collision)
    {
        //判断是不是地面
        if (collision.collider.tag == "Ground")
        {
            //离开地面上
            isGround = false;
        }
    }
}
