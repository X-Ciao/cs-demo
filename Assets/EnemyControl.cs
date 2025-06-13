using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float moveSpeed = 2f;       // 移动速度
    public float roamRange = 5f;       // 游荡范围
    public float respawnTime = 3f;    // 重生时间
    public int maxHealth = 3;          // 最大血量
    public int currentHealth;          // 当前血量

    // 组件引用
    private Renderer enemyRenderer;   // 渲染组件
    private Collider enemyCollider;    // 碰撞组件

    private Vector3 startPos;          // 起始位置
    private Vector3 targetPos;         // 当前目标位置
    private bool isDead = false;       // 是否死亡

    // Start is called before the first frame update
    void Start()
    {

        // 获取组件引用
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();

        // 保存起始位置
        startPos = transform.position;
        // 初始化血量
        currentHealth = maxHealth;
        // 设置第一个随机目标点
        SetNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        // 计算移动方向
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        // 移动
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 检查是否到达目标点
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            SetNewTarget();
        }
    }

    // 设置新的随机目标点
    void SetNewTarget()
    {
        // 在游荡范围内随机选择一个点
        Vector2 randomPoint = Random.insideUnitCircle * roamRange;
        targetPos = startPos + new Vector3(randomPoint.x, 0, randomPoint.y);
    }


    // 碰撞检测（由敌人自行检测是否被子弹击中）
    void OnTriggerEnter(Collider other)
    {
        // 检查是否被子弹击中
        if (other.CompareTag("Bullet"))
        {
            // 应用伤害（每次击中减1点血）
            TakeDamage(1);

            // 销毁子弹
            Destroy(other.gameObject);
        }
    }

    // 承受伤害
    public void TakeDamage(int damage)
    {
        // 如果已死亡则不处理
        if (isDead) return;

        // 减少血量
        currentHealth -= damage;

        // 播放受击效果（闪烁）
        StartCoroutine(HitEffect());

        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 受击效果（简单闪烁）
    IEnumerator HitEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = Color.white;
        }
    }

    // 敌人死亡
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // 禁用可见性和碰撞
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 启动重生协程
        StartCoroutine(Respawn());
    }

    // 重生协程
    IEnumerator Respawn()
    {
        // 等待重生时间
        yield return new WaitForSeconds(respawnTime);

        // 重置到起始位置
        transform.position = startPos;

        // 重置血量
        currentHealth = maxHealth;

        // 重新启用
        isDead = false;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        // 恢复颜色
        if (enemyRenderer != null)
            enemyRenderer.material.color = Color.white;

        // 设置新目标
        SetNewTarget();
    }
    // 调试显示
    void OnDrawGizmos()
    {
        // 显示移动目标
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetPos);

        // 显示游荡范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPos, roamRange);
    }
}
