using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
    //���������
    public Transform FirePoint;
    //��������Ԥ����
    public GameObject FirePre;
    //�����ӵ���
    public Transform BulletPoint;
    //�����ӵ�Ԥ����
    public GameObject BulletPre;
    //����ǹ����Ч
    public AudioClip clip;
    //�����ӵ�UI
    public Text BulletText;
    //����������Ч
    public AudioClip reloadSound;

    //�ӵ�����
    private int bulletCount = 10;
    // ��ϻ����
    private int maxBulletCount = 10; 
    //������
    private float cd = 0.2f;
    //��ʱ��
    private float timer = 0;
    //�����������
    private AudioSource gunPlayer;
    // ���õ�ҩ
    private int reserveAmmo = 180;
    // ��������ʱ��
    private float reloadTime = 1.5f;
    //// ��ҩ��ʾ�ı�
    //private Text ammoText;           
    // ������ر���
    private bool isReloading = false;
    // ������ʱ��
    private float reloadTimer = 0f;


    void Start()
    {
        //��ȡ���������
        gunPlayer = GetComponent<AudioSource>();
        //��ʼ��UI
        UpdateAmmoDisplay();
    }

    
    void Update()
    {
        //�����Ϸ״̬
        if (GameManager.Instance != null &&
            (!GameManager.Instance.isGameActive || GameManager.Instance.isPaused))
        {
            return; // ��Ϸ��������ͣʱֱ�ӷ���
        }

        //��ʱ
        timer += Time.deltaTime;

        // ���»�����ʱ����������ڻ�����
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;


            // ��黻���Ƿ����
            if (reloadTimer >= reloadTime)
            {
                FinishReload();
            }
        }
        
        //�����ʱ������cd�����Ұ������������������ӵ����򿪻�
        if (timer > cd && Input.GetMouseButton(0) && bulletCount > 0)
        {
            Fire();
            
        }

        // ��⻻��������R�������ҿ��Ի���
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            TryStartReload();
        }

        // �Զ����������ӵ�����ʱ�Զ�����
        if (bulletCount == 0 && !isReloading && reserveAmmo > 0)
        {
            TryStartReload();
        }
    }

    //�������
    void Fire()
    {
        //���ü�ʱ��
        timer = 0;
        //��������
        Instantiate(FirePre, FirePoint.position, FirePoint.rotation);
        //�����ӵ�
        Instantiate(BulletPre, BulletPoint.position, BulletPoint.rotation);
        //�ӵ���������
        bulletCount--;
        // ���µ�ҩ��ʾ
        UpdateAmmoDisplay();
        //����ǹ��
        gunPlayer.PlayOneShot(clip);
    }

    // ��ʼ����
    void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;

        // ���Ż�����Ч
        if (reloadSound != null)
        {
            gunPlayer.PlayOneShot(reloadSound);
            GetComponent<Animator>().SetTrigger("Reload");
        }

    }

    // ��ɻ���
    void FinishReload()
    {
        isReloading = false;

        // ������Ҫ����ĵ�ҩ��
        int neededAmmo = maxBulletCount - bulletCount;

        // ���ܲ��䳬�����õ�ҩ����
        int ammoToAdd = Mathf.Min(neededAmmo, reserveAmmo);

        // ���䵯ҩ
        bulletCount += ammoToAdd;
        reserveAmmo -= ammoToAdd;

        // ���µ�ҩ��ʾ
        UpdateAmmoDisplay();

    }
    
    // ���Կ�ʼ����
    void TryStartReload()
    {
        // �������Ҫ��������ҩ������û�б��õ�ҩ��
        if (bulletCount == maxBulletCount || reserveAmmo <= 0)
        {
            return;
        }

        // ��ʼ����
        StartReload();
    }

    // ���µ�ҩ��ʾ
    void UpdateAmmoDisplay()
    {
        if (BulletText != null)
        {
            BulletText.text = $"{bulletCount} / {reserveAmmo}";
        }
    }


}
