using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update

    //����
    private Rigidbody rBody;
    //��Ƶ���
    private AudioSource footPlayer;
    //�Ƿ��ڵ���
    private bool isGround;
   
    
    void Start()
    {
        //��ȡ�������
        rBody = GetComponent<Rigidbody>();
        //��ȡ�������
        footPlayer = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //������¿ո��
        if(Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            //��Ծ��������һ�����ϵ���
            rBody.AddForce(Vector3.up * 200);
        }

        //�Ƿ����ƶ���
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //�ƶ��������ƶ��������ҵ�ǰû�в������������ҽ�ɫ���ڵ�����
        if ((horizontal != 0 || vertical !=0) && isGround == true)
        {
            //���ŽŲ���
            if (footPlayer.isPlaying == false)
            {
                footPlayer.Play();
            }
        }
        else
        {
            //û���ƶ���û�а����ƶ�����ֹͣ�Ų���
            footPlayer.Stop();
        }

    }
   

    //������ײ
    private void OnCollisionEnter(Collision collision)
    {
        //�ж��ǲ��ǵ���
        if(collision.collider.tag == "Ground")
        {
            //���ڵ�����
            isGround = true;
        }
    }

    //������ײ
    private void OnCollisionExit(Collision collision)
    {
        //�ж��ǲ��ǵ���
        if (collision.collider.tag == "Ground")
        {
            //�뿪������
            isGround = false;
        }
    }
}
