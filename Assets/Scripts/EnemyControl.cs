using System.Collections; // ʹ��Э��
using UnityEngine; // Unity������Ŀ�
using UnityEngine.UI; // UIϵͳ

public class EnemyControl : MonoBehaviour
{
    // �����ƶ�����
    public float moveSpeed = 2f; // �ƶ��ٶ�
    public float roamRange = 5f; // Ѳ�߷�Χ�뾶
    public float chaseRange = 8f; // ׷����ҵķ�Χ
    public float rotationSpeed = 5f; // ��ת�ٶ�

    // ����ս�����÷���
    public int maxHealth = 3; // �������ֵ
    public int currentHealth; // ��ǰ����ֵ
    public float respawnTime = 3f; // ����ʱ��
    public int scoreValue = 100; // ���ܺ��õķ���

    // UI���÷���
    public Slider healthBarSlider; // Ѫ��Slider���
    public Canvas healthBarCanvas; // Ѫ��Canvas���

    // �������
    private Transform player; // ���λ������
    private Renderer enemyRenderer; // ��Ⱦ�����
    private Collider enemyCollider; // ��ײ�����
    private Rigidbody rb; // �������

    // ״̬����
    public Vector3 startPos; // ��ʼλ��
    private Vector3 targetPos; // Ѳ��Ŀ��λ��
    private bool isDead = false; // �Ƿ�������־
    private bool isChasing = false; // �Ƿ���׷�����
    private EnemyState currentState = EnemyState.Roaming; // ��ǰ״̬��Ĭ��Ѳ�ߣ�

    // Ѫ����ת����
    private Quaternion healthBarRotation; // Ѫ����ʼ��תֵ

    // ����״̬ö��
    private enum EnemyState
    {
        Roaming, // Ѳ��״̬
        Chasing  // ׷�����״̬
    }



    void Start()
    {
        // ������Ҷ���
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // ��ȡ���
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        //// ���ȱ��������Զ����
        //if (rb == null)
        //{
        //    Debug.LogWarning($"Rigidbody missing on {gameObject.name}, adding one.");
        //    rb = gameObject.AddComponent<Rigidbody>();

        //    // ����Ĭ����������
        //    rb.mass = 1f;
        //    rb.drag = 0f;
        //    rb.angularDrag = 0.05f;
        //    rb.useGravity = false; // �����������
        //    rb.isKinematic = false;
        //}

        // ��ʼ��λ�ú�����ֵ
        startPos = transform.position;
        currentHealth = maxHealth;
        // ����Ѳ��Ŀ���
        SetNewTarget();

        // ��ʼ��Ѫ��
        if (healthBarSlider != null)
        {
            // ����Ѫ�����ֵ�͵�ǰֵ
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
            // ����Ѫ����ʼ��ת
            healthBarRotation = healthBarCanvas.transform.rotation;
        }
    }

    void Update()
    {
        // �����������������
        if (isDead) return;

        // ����Ѫ��λ�ú���ת
        UpdateHealthBar();

        // ��������ҵľ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ״̬������
        switch (currentState)
        {
            case EnemyState.Roaming: // Ѳ��״̬
                // �����ҽ���׷��Χ
                if (distanceToPlayer <= chaseRange)
                {
                    // �л���׷��״̬
                    currentState = EnemyState.Chasing;
                    break;
                }

                // ��Ŀ����ƶ�
                MoveToTarget();
                // ����Ƿ񵽴�Ŀ���
                if (Vector3.Distance(transform.position, targetPos) < 0.5f)
                {
                    // ������Ŀ���
                    SetNewTarget();
                }
                break;

            case EnemyState.Chasing: // ׷��״̬
                // �����ҳ���׷��Χ
                if (distanceToPlayer > chaseRange)
                {
                    // �л���Ѳ��״̬
                    currentState = EnemyState.Roaming;
                    // ������Ŀ���
                    SetNewTarget();
                    break;
                }

                // ׷�����
                ChasePlayer();
                break;
        }
    }

    void FixedUpdate()
    {
        // ������ת��ʹ��FixedUpdate��֤�����ȶ��ԣ�
        if (!isDead && (currentState == EnemyState.Chasing || Vector3.Distance(transform.position, targetPos) > 0.5f))
        {
            // ����״̬ȷ��Ŀ�귽��
            Vector3 targetDirection = currentState == EnemyState.Chasing ?
                (player.position - transform.position).normalized :
                (targetPos - transform.position).normalized;

            // ȷ��Ŀ�귽����Ч
            if (targetDirection != Vector3.zero)
            {
                // ����Ŀ����ת
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // ƽ����ת��Ŀ�귽��
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // ����Ѫ��UI
    void UpdateHealthBar()
    {
        if (healthBarCanvas != null)
        {
            // ����Ѫ��ʼ�����������
            healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    // ��Ŀ����ƶ�
    void MoveToTarget()
    {
        // �����ƶ�����
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        // ʹ�ø����ƶ�������ȫ��
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    // ׷�����
    void ChasePlayer()
    {
        // ���㳯����ҵķ���
        Vector3 moveDirection = (player.position - transform.position).normalized;
        // ������ƶ�
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    // �����µ�Ѳ��Ŀ���
    void SetNewTarget()
    {
        // ��Ѳ�߷�Χ��������ɵ�
        Vector2 randomPoint = Random.insideUnitCircle * roamRange;
        // ת��Ϊ3Dλ��
        targetPos = startPos + new Vector3(randomPoint.x, 0, randomPoint.y);
    }

    // ��ײ���
    void OnTriggerEnter(Collider other)
    {
        // ��⵽�ӵ�
        if (other.CompareTag("Bullet"))
        {
            // �ܵ��˺�
            TakeDamage(1);
            // �����ӵ�
            Destroy(other.gameObject);
        }
    }

    // �ܵ��˺�
    public void TakeDamage(int damage)
    {
        // ��������������
        if (isDead) return;

        // ��������ֵ
        currentHealth -= damage;

        // ����Ѫ����ʾ
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        // �����ܻ�Ч��
        StartCoroutine(HitEffect());

        // ����Ƿ�����
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // �ܻ�Ч��Э��
    IEnumerator HitEffect()
    {
        if (enemyRenderer != null)
        {
            // ����ʾ����
            enemyRenderer.material.color = Color.red;
            // �ȴ�0.1��
            yield return new WaitForSeconds(0.1f);
            // �ָ�ԭɫ
            enemyRenderer.material.color = Color.white;
        }
    }

    // ��������
    public void Die()
    {
        // ��ֹ�ظ�����
        if (isDead) return;

        // ��������״̬
        isDead = true;
        // ����ΪѲ��״̬
        currentState = EnemyState.Roaming;

        // ������Ⱦ����ײ
        enemyRenderer.enabled = false;
        enemyCollider.enabled = false;

        // ����Ѫ��
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(false);

        //// ֪ͨ��Ϸ���������˱�����
        //GameManager.Instance.EnemyKilled(scoreValue);

        // ��ʼ����Э��
        StartCoroutine(Respawn());
    }

    // ����Э��
    IEnumerator Respawn()
    {
        // �ȴ�����ʱ��
        yield return new WaitForSeconds(respawnTime);

        // ����λ�ú�״̬
        transform.position = startPos;
        currentHealth = maxHealth;
        isDead = false;

        // ������Ⱦ����ײ
        enemyRenderer.enabled = true;
        enemyCollider.enabled = true;
        enemyRenderer.material.color = Color.white;

        // ���ò�����Ѫ��
        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(true);
            healthBarSlider.value = currentHealth;
        }

        // �����µ�Ѳ��Ŀ��
        SetNewTarget();
    }

    // �����༭���л��Ƹ�����
    void OnDrawGizmosSelected()
    {
        // ����Ŀ��㣨��ɫ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetPos);

        // ����Ѳ�߷�Χ����ɫ��
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPos, roamRange);

        // ����׷��Χ����ɫ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}