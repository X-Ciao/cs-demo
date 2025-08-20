using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    

    [Header("Movement Settings")]
    public float Speed = 3f;
    public float Sensitivity = 3f;
    public float MinPitch = -45f;
    public float MaxPitch = 45f;
    public float InterpolationSpeed = 15f;
    public float JumpForce = 5f;

    // 组件引用
    private CharacterController CharacterControllerPlayer;
    private Transform cameraTransform;
    private AudioSource footPlayer;

    // 旋转状态
    private float currentPitch = 0f;
    private float currentYaw = 0f;
    private float targetYaw = 0f;



    private float gravity = -9.81f;
    private Vector3 velocity;

    

    private NetworkVariable<Vector3> SyncedPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector2> SyncedRotation = new NetworkVariable<Vector2>();

    // 同步计时器
    private float syncTimer;
    private const float SYNC_INTERVAL = 0.1f; // 每秒10次同步


    public override void OnNetworkSpawn()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;

        if (!IsOwner)
        {
            // 禁用非本地玩家的相机组件
            cameraTransform.GetComponent<AudioListener>().enabled = false;
            cameraTransform.GetComponent<Camera>().enabled = false;
            // 注册同步回调
            SyncedPosition.OnValueChanged += UpdatePosition;
            SyncedRotation.OnValueChanged += UpdateRotation;
        }
        else
        {
            // 初始化本地玩家组件
            CharacterControllerPlayer = GetComponent<CharacterController>();
            footPlayer = GetComponent<AudioSource>();

            // 启用本地玩家的摄像头和音频监听器
            cameraTransform.GetComponent<Camera>().enabled = true;
            cameraTransform.GetComponent<AudioListener>().enabled = true;

            // 锁定鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 初始化旋转值
            currentYaw = transform.eulerAngles.y;
            targetYaw = currentYaw;
        }
        if (IsServer) // 服务器设置初始同步值
        {
            SyncedPosition.Value = transform.position;
            SyncedRotation.Value = new Vector2(0, transform.eulerAngles.y); // 假设初始俯仰角为0
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        MovePlayer();

        Look();

        Jump();

        Footfall();

        // 定时同步
        syncTimer += Time.deltaTime;
        if (syncTimer >= SYNC_INTERVAL)
        {
            SyncTransformServerRpc(transform.position, currentPitch, currentYaw);
            syncTimer = 0;
        }

    }

    void LateUpdate()
    {
        if (!IsOwner)
        {
            // 平滑位置插值
            transform.position = Vector3.Lerp(
                transform.position, 
                SyncedPosition.Value, 
                InterpolationSpeed * Time.deltaTime
            );
            
            // 平滑旋转插值
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.Euler(0, targetYaw, 0), 
                InterpolationSpeed * Time.deltaTime
            );
        }
    }

    [ServerRpc]
    void SyncTransformServerRpc(Vector3 position, float cameraPitch, float playerYaw)
    {
        SyncedPosition.Value = position;
        SyncedRotation.Value = new Vector2(cameraPitch, playerYaw);
    }

    void UpdatePosition(Vector3 previous, Vector3 current)
    {

    }

    void UpdateRotation(Vector2 previous, Vector2 current)
    {
        if (!IsOwner)
        {
            // 更新目标旋转值
            targetYaw = current.y;

            // 直接设置相机俯仰
            cameraTransform.localEulerAngles = new Vector3(current.x, 0, 0);
        }
    }

    void MovePlayer()
    {
         // 地面检测和重力重置
        if (CharacterControllerPlayer.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            // 应用重力
            velocity.y += gravity * Time.deltaTime;
        }

        // 获取输入
        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"), 
            0, 
            Input.GetAxis("Vertical")
        );
        
        // 限制移动向量长度
        move = Vector3.ClampMagnitude(move, 1f);
        
        // 转换为世界空间方向
        move = transform.TransformDirection(move);
        
        // 应用移动
        CharacterControllerPlayer.Move((move * Speed + velocity) * Time.deltaTime);
    }

   

    void Look()
    {

        // 处理水平旋转
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity;
        currentYaw += mouseX;
        transform.rotation = Quaternion.Euler(0, currentYaw, 0);

        // 处理俯仰旋转
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, MinPitch, MaxPitch);
        cameraTransform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CharacterControllerPlayer.isGrounded) // 
        {
            // 速度跳跃
            float jumpForce = 5f;
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
    }

    void Footfall()
    {
        bool isMoving = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        if (isMoving && CharacterControllerPlayer.isGrounded) // 使用内置属性
        {
            if (!footPlayer.isPlaying) footPlayer.Play();
        }
        else footPlayer.Stop();
    }




 

}


