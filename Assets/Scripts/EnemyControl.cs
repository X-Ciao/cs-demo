using System.Collections; // ʹ��Э��
using UnityEngine; // Unity������Ŀ�
using UnityEngine.EventSystems;
using UnityEngine.UI; // UIϵͳ

public class EnemyControl : MonoBehaviour
{
    // �����ƶ�����
    public float moveSpeed;// �ƶ��ٶ�
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

    //�����ƶ�����
    private Vector3 moveDirection;

    public LayerMask obstacleLayers; // �����ϰ���㼶����ҡ��������������ˣ�
    public PhysicMaterial enemyPhysicMaterial; // ��Ħ�������������

    // ����״̬ö��
    private enum EnemyState
    {
        Roaming, // Ѳ��״̬
        Chasing  // ׷�����״̬
    }



    void Start()
    {
        moveSpeed = 2f;
        // ������Ҷ���
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // ��ȡ���
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();


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

        // ��������������
        if (enemyCollider != null)
        {
            enemyCollider.material = enemyPhysicMaterial;
        }

        // ���ø�����ײ���ģʽ
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        
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


                moveDirection = (targetPos - transform.position).normalized;

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


                moveDirection = (player.position - transform.position).normalized;
                moveDirection.y = 0;
                break;
        }
    }

    void FixedUpdate()
    {
        // ������ת
        if (!isDead && (currentState == EnemyState.Chasing || Vector3.Distance(transform.position, targetPos) > 0.5f))
        {
            // ����״̬ȷ��Ŀ�귽��
            Vector3 targetDirection = currentState == EnemyState.Chasing ?
                new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z).normalized :
                new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z).normalized;

            // ȷ��Ŀ�귽����Ч
            if (targetDirection != Vector3.zero)
            {
                // ����Ŀ����ת
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // ƽ����ת��Ŀ�귽��
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }



        // λ�ø��� 
        if (!isDead && moveDirection != Vector3.zero)
        {
            // ���ǰ���Ƿ����ϰ���
            RaycastHit hit;
            float rayDistance = 1.0f; // ���߳���
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // �������߶ȷ���

            if (Physics.Raycast(rayOrigin, moveDirection, out hit, rayDistance, obstacleLayers))
            {
                // ������÷���
                Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                // ���ѡ�����ұ��÷���
                if (Random.value > 0.5f)
                {
                    avoidDirection *= -1;
                }

                // Ӧ�ñ��÷���
                moveDirection = avoidDirection;
            }


            // ִ�������ƶ�
            Vector3 moveOffset = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveOffset);
        }

        //Debug.DrawRay(transform.position, moveDirection * 2, Color.red);
        //Debug.Log($"ʵ���ٶ�: {rb.velocity.magnitude}");
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

        // ֪ͨ��Ϸ���������˱�����
        GameManager.Instance.EnemyKilled(scoreValue);

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