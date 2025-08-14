using Unity.Netcode;
using UnityEngine;

public class Move : NetworkBehaviour
{
    public float horizontalSensitivity = 180f;
    public float verticalSensitivity = 180f;

    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Camera playerCamera;

    void Start()
    {
        if (!IsOwner) return;

        // 确保组件在编辑器中已赋值
        if (head == null) head = transform;
        if (body == null) body = transform.parent;
        if (playerCamera == null) playerCamera = GetComponent<Camera>();

        // 激活本地玩家摄像头
        playerCamera.enabled = true;
        playerCamera.GetComponent<AudioListener>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner) return;

        // 移动逻辑
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical);

        if (dir != Vector3.zero)
        {
            // 使用物理移动而非直接Transform
            body.GetComponent<Rigidbody>().MovePosition(
                body.position + body.TransformDirection(dir) * Time.deltaTime * 3
            );
        }

        // 视角旋转
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0)
        {
            body.Rotate(Vector3.up, mouseX * horizontalSensitivity * Time.deltaTime);
        }

        if (mouseY != 0)
        {
            // 计算新角度并限制范围
            float newPitch = head.localEulerAngles.x - mouseY * verticalSensitivity * Time.deltaTime;

            // 将角度转换到-180~180范围
            if (newPitch > 180) newPitch -= 360;

            // 应用角度限制
            newPitch = Mathf.Clamp(newPitch, -60, 60);
            head.localEulerAngles = new Vector3(newPitch, 0, 0);
        }
    }
}