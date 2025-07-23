using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
    //关联火焰点
    public Transform FirePoint;
    //关联火焰预设体
    public GameObject FirePre;
    //关联子弹点
    public Transform BulletPoint;
    //关联子弹预设体
    public GameObject BulletPre;
    //关联枪声音效
    public AudioClip clip;
    //关联子弹UI
    public Text BulletText;
    //关联换弹音效
    public AudioClip reloadSound;

    //子弹个数
    private int bulletCount = 10;
    // 弹匣容量
    private int maxBulletCount = 10; 
    //开火间隔
    private float cd = 0.2f;
    //计时器
    private float timer = 0;
    //声音播放组件
    private AudioSource gunPlayer;
    // 备用弹药
    private int reserveAmmo = 180;
    // 换弹所需时间
    private float reloadTime = 1.5f;
    //// 弹药显示文本
    //private Text ammoText;           
    // 换弹相关变量
    private bool isReloading = false;
    // 换弹计时器
    private float reloadTimer = 0f;


    void Start()
    {
        //获取播放器组件
        gunPlayer = GetComponent<AudioSource>();
        //初始化UI
        UpdateAmmoDisplay();
    }

    
    void Update()
    {
        //检查游戏状态
        if (GameManager.Instance != null &&
            (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused))
        {
            return; // 游戏结束或暂停时直接返回
        }

        //计时
        timer += Time.deltaTime;

        // 更新换弹计时器（如果正在换弹）
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;


            // 检查换弹是否完成
            if (reloadTimer >= reloadTime)
            {
                FinishReload();
            }
        }
        
        //如果计时器满足cd，并且按下鼠标左键，并且有子弹，则开火
        if (timer > cd && Input.GetMouseButton(0) && bulletCount > 0)
        {
            Fire();
            
        }

        // 检测换弹按键（R键）并且可以换弹
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            TryStartReload();
        }

        // 自动换弹：当子弹用完时自动换弹
        if (bulletCount == 0 && !isReloading && reserveAmmo > 0)
        {
            TryStartReload();
        }
    }

    //射击函数
    void Fire()
    {
        //重置计时器
        timer = 0;
        //创建火焰
        Instantiate(FirePre, FirePoint.position, FirePoint.rotation);
        //创建子弹
        Instantiate(BulletPre, BulletPoint.position, BulletPoint.rotation);
        //子弹个数减少
        bulletCount--;
        // 更新弹药显示
        UpdateAmmoDisplay();
        //播放枪声
        gunPlayer.PlayOneShot(clip);
    }

    // 开始换弹
    void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;

        // 播放换弹音效
        if (reloadSound != null)
        {
            gunPlayer.PlayOneShot(reloadSound);
            GetComponent<Animator>().SetTrigger("Reload");
        }

    }

    // 完成换弹
    void FinishReload()
    {
        isReloading = false;

        // 计算需要补充的弹药量
        int neededAmmo = maxBulletCount - bulletCount;

        // 不能补充超过备用弹药的量
        int ammoToAdd = Mathf.Min(neededAmmo, reserveAmmo);

        // 补充弹药
        bulletCount += ammoToAdd;
        reserveAmmo -= ammoToAdd;

        // 更新弹药显示
        UpdateAmmoDisplay();

    }
    
    // 尝试开始换弹
    void TryStartReload()
    {
        // 如果不需要换弹（弹药已满或没有备用弹药）
        if (bulletCount == maxBulletCount || reserveAmmo <= 0)
        {
            return;
        }

        // 开始换弹
        StartReload();
    }

    // 更新弹药显示
    void UpdateAmmoDisplay()
    {
        if (BulletText != null)
        {
            BulletText.text = $"{bulletCount} / {reserveAmmo}";
        }
    }


}
