using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController CharacterControllerPlayer;

    float Speed = 3f;

    float pitch = 0f;

    public Transform cameraTransform;

    private float gravity = -9.81f;
    private Vector3 velocity;

    //音频组件
    private AudioSource footPlayer;





        public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // 禁用非本地玩家的相机组件
            cameraTransform.GetComponent<AudioListener>().enabled = false;
            cameraTransform.GetComponent<Camera>().enabled = false;
        }
        else
        {
            // 仅本地玩家初始化控制器
            CharacterControllerPlayer = GetComponent<CharacterController>();
            footPlayer = GetComponent<AudioSource>();

            // 确保本地玩家启用相机
            cameraTransform.GetComponent<Camera>().enabled = true;
            cameraTransform.GetComponent<AudioListener>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
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

        if (IsOwner)
        {
           
            NetworkObject.transform.position = transform.position;
            NetworkObject.transform.rotation = transform.rotation;
        }
        SyncPositionServerRpc(transform.position);


        if (IsOwner)
        {
            Debug.Log($"Owner rotation: {transform.rotation}");
        }

    }

    void MovePlayer()
    {
        if (CharacterControllerPlayer.isGrounded && velocity.y < 0)
            velocity.y = -2f; // 轻微下压确保贴地


        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        move = Vector3.ClampMagnitude(move, 1f);

        move = transform.TransformDirection(move);

        CharacterControllerPlayer.SimpleMove(move * Speed);

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        CharacterControllerPlayer.Move((move + velocity) * Time.deltaTime);
    }

    float Sensitivity = 3f;

    float MinPitch = -45f;

    float MaxPitch = 45f;

    void Look()
    {

        float mouseX = Input.GetAxis("Mouse X") * Sensitivity;

        transform.Rotate(0, mouseX, 0);

        pitch -= Input.GetAxis("Mouse Y") * Sensitivity;

        pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
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


    [ServerRpc]
    void SyncPositionServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }


 

}


