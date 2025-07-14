using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //给子弹组件刚体一个力
        GetComponent<Rigidbody>().AddForce(transform.forward * 800);
        
        // 2秒后自动销毁
        Destroy(gameObject, 2f);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
