using System.Collections; // 使用协程
using UnityEngine; // Unity引擎核心库
using UnityEngine.EventSystems;
using UnityEngine.UI; // UI系统

public class EnemyControl : MonoBehaviour
{
    // 敌人移动分组
    public float moveSpeed;// 移动速度
    public float roamRange = 5f; // 巡逻范围半径
    public float chaseRange = 8f; // 追逐玩家的范围
    public float rotationSpeed = 5f; // 旋转速度

    // 敌人战斗设置分组
    public int maxHealth = 3; // 最大生命值
    public int currentHealth; // 当前生命值
    public float respawnTime = 3f; // 复活时间
    public int scoreValue = 100; // 击败后获得的分数

    // UI设置分组
    public Slider healthBarSlider; // 血条Slider组件
    public Canvas healthBarCanvas; // 血条Canvas组件

    // 组件引用
    private Transform player; // 玩家位置引用
    private Renderer enemyRenderer; // 渲染器组件
    private Collider enemyCollider; // 碰撞器组件
    private Rigidbody rb; // 刚体组件

    // 状态变量
    public Vector3 startPos; // 初始位置
    private Vector3 targetPos; // 巡逻目标位置
    private bool isDead = false; // 是否死亡标志
    private bool isChasing = false; // 是否在追逐玩家
    private EnemyState currentState = EnemyState.Roaming; // 当前状态（默认巡逻）

    // 血条旋转控制
    private Quaternion healthBarRotation; // 血条初始旋转值

    //传递移动方向
    private Vector3 moveDirection;

    public LayerMask obstacleLayers; // 设置障碍物层级（玩家、建筑、其他敌人）
    public PhysicMaterial enemyPhysicMaterial; // 低摩擦力的物理材质

    // 敌人状态枚举
    private enum EnemyState
    {
        Roaming, // 巡逻状态
        Chasing  // 追逐玩家状态
    }



    void Start()
    {
        moveSpeed = 2f;
        // 查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // 获取组件
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();


        // 初始化位置和生命值
        startPos = transform.position;
        currentHealth = maxHealth;
        // 设置巡逻目标点
        SetNewTarget();

        // 初始化血条
        if (healthBarSlider != null)
        {
            // 设置血条最大值和当前值
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
            // 保存血条初始旋转
            healthBarRotation = healthBarCanvas.transform.rotation;
        }

        // 添加物理材质设置
        if (enemyCollider != null)
        {
            enemyCollider.material = enemyPhysicMaterial;
        }

        // 设置刚体碰撞检测模式
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        
    }

    void Update()
    {
        // 如果死亡则跳过更新
        if (isDead) return;

        // 更新血条位置和旋转
        UpdateHealthBar();

        // 计算与玩家的距离
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 状态机处理
        switch (currentState)
        {
            case EnemyState.Roaming: // 巡逻状态
                // 如果玩家进入追逐范围
                if (distanceToPlayer <= chaseRange)
                {
                    // 切换到追逐状态
                    currentState = EnemyState.Chasing;
                    break;
                }


                moveDirection = (targetPos - transform.position).normalized;

                // 检查是否到达目标点
                if (Vector3.Distance(transform.position, targetPos) < 0.5f)
                {
                    // 设置新目标点
                    SetNewTarget();
                }
                break;

            case EnemyState.Chasing: // 追逐状态
                // 如果玩家超出追逐范围
                if (distanceToPlayer > chaseRange)
                {
                    // 切换回巡逻状态
                    currentState = EnemyState.Roaming;
                    // 设置新目标点
                    SetNewTarget();
                    break;
                }


                moveDirection = (player.position - transform.position).normalized;
                moveDirection.y = 0;
                break;
        }
    }

    void FixedUpdate()
    {
        // 处理旋转
        if (!isDead && (currentState == EnemyState.Chasing || Vector3.Distance(transform.position, targetPos) > 0.5f))
        {
            // 根据状态确定目标方向
            Vector3 targetDirection = currentState == EnemyState.Chasing ?
                new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z).normalized :
                new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z).normalized;

            // 确保目标方向有效
            if (targetDirection != Vector3.zero)
            {
                // 计算目标旋转
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // 平滑旋转到目标方向
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }



        // 位置更新 
        if (!isDead && moveDirection != Vector3.zero)
        {
            // 检测前方是否有障碍物
            RaycastHit hit;
            float rayDistance = 1.0f; // 射线长度
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // 从腰部高度发射

            if (Physics.Raycast(rayOrigin, moveDirection, out hit, rayDistance, obstacleLayers))
            {
                // 计算避让方向
                Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                // 随机选择左右避让方向
                if (Random.value > 0.5f)
                {
                    avoidDirection *= -1;
                }

                // 应用避让方向
                moveDirection = avoidDirection;
            }


            // 执行物理移动
            Vector3 moveOffset = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveOffset);
        }

        //Debug.DrawRay(transform.position, moveDirection * 2, Color.red);
        //Debug.Log($"实际速度: {rb.velocity.magnitude}");
    }

    // 更新血条UI
    void UpdateHealthBar()
    {
        if (healthBarCanvas != null)
        {
            // 保持血条始终面向摄像机
            healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    // 设置新的巡逻目标点
    void SetNewTarget()
    {
        // 在巡逻范围内随机生成点
        Vector2 randomPoint = Random.insideUnitCircle * roamRange;
        // 转换为3D位置
        targetPos = startPos + new Vector3(randomPoint.x, 0, randomPoint.y);
    }

    // 碰撞检测
    void OnTriggerEnter(Collider other)
    {
        // 检测到子弹
        if (other.CompareTag("Bullet"))
        {
            // 受到伤害
            TakeDamage(1);
            // 销毁子弹
            Destroy(other.gameObject);
        }
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        // 如果已死亡则忽略
        if (isDead) return;

        // 减少生命值
        currentHealth -= damage;

        // 更新血条显示
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        // 播放受击效果
        StartCoroutine(HitEffect());

        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 受击效果协程
    IEnumerator HitEffect()
    {
        if (enemyRenderer != null)
        {
            // 变红表示受伤
            enemyRenderer.material.color = Color.red;
            // 等待0.1秒
            yield return new WaitForSeconds(0.1f);
            // 恢复原色
            enemyRenderer.material.color = Color.white;
        }
    }

    // 死亡处理
    public void Die()
    {
        // 防止重复死亡
        if (isDead) return;

        // 设置死亡状态
        isDead = true;
        // 重置为巡逻状态
        currentState = EnemyState.Roaming;

        // 禁用渲染和碰撞
        enemyRenderer.enabled = false;
        enemyCollider.enabled = false;

        // 禁用血条
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        // 通知游戏管理器敌人被击败
        GameManager.Instance.EnemyKilled(scoreValue);

        // 开始复活协程
        StartCoroutine(Respawn());
    }

    
    // 复活协程
    IEnumerator Respawn()
    {
        // 等待复活时间
        yield return new WaitForSeconds(respawnTime);

        // 重置位置和状态
        transform.position = startPos;
        currentHealth = maxHealth;
        isDead = false;

        // 启用渲染和碰撞
        enemyRenderer.enabled = true;
        enemyCollider.enabled = true;
        enemyRenderer.material.color = Color.white;


        // 启用并重置血条
        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(true);
            healthBarSlider.value = currentHealth;
        }

        // 设置新的巡逻目标
        SetNewTarget();
    }

    // 场景编辑器中绘制辅助线
    void OnDrawGizmosSelected()
    {
        // 绘制目标点（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetPos);

        // 绘制巡逻范围（青色）
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPos, roamRange);

        // 绘制追逐范围（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}