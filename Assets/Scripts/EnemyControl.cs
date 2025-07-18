using System.Collections; // 使用协程
using UnityEngine; // Unity引擎核心库
using UnityEngine.UI; // UI系统

public class EnemyControl : MonoBehaviour
{
    // 敌人移动分组
    public float moveSpeed = 2f; // 移动速度
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

    // 敌人状态枚举
    private enum EnemyState
    {
        Roaming, // 巡逻状态
        Chasing  // 追逐玩家状态
    }



    void Start()
    {
        // 查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // 获取组件
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        //// 如果缺少组件，自动添加
        //if (rb == null)
        //{
        //    Debug.LogWarning($"Rigidbody missing on {gameObject.name}, adding one.");
        //    rb = gameObject.AddComponent<Rigidbody>();

        //    // 配置默认物理属性
        //    rb.mass = 1f;
        //    rb.drag = 0f;
        //    rb.angularDrag = 0.05f;
        //    rb.useGravity = false; // 根据需求调整
        //    rb.isKinematic = false;
        //}

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

                // 向目标点移动
                MoveToTarget();
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

                // 追逐玩家
                ChasePlayer();
                break;
        }
    }

    void FixedUpdate()
    {
        // 处理旋转（使用FixedUpdate保证物理稳定性）
        if (!isDead && (currentState == EnemyState.Chasing || Vector3.Distance(transform.position, targetPos) > 0.5f))
        {
            // 根据状态确定目标方向
            Vector3 targetDirection = currentState == EnemyState.Chasing ?
                (player.position - transform.position).normalized :
                (targetPos - transform.position).normalized;

            // 确保目标方向有效
            if (targetDirection != Vector3.zero)
            {
                // 计算目标旋转
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // 平滑旋转到目标方向
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
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

    // 向目标点移动
    void MoveToTarget()
    {
        // 计算移动方向
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        // 使用刚体移动（物理安全）
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    // 追逐玩家
    void ChasePlayer()
    {
        // 计算朝向玩家的方向
        Vector3 moveDirection = (player.position - transform.position).normalized;
        // 向玩家移动
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
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

        //// 通知游戏管理器敌人被击败
        //GameManager.Instance.EnemyKilled(scoreValue);

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