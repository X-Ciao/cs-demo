using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //���ӵ��������һ����
        GetComponent<Rigidbody>().AddForce(transform.forward * 800);
        
        // 2����Զ�����
        Destroy(gameObject, 2f);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
