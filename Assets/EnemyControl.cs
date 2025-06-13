using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float moveSpeed = 2f;       // �ƶ��ٶ�
    public float roamRange = 5f;       // �ε���Χ
    public float respawnTime = 3f;    // ����ʱ��
    public int maxHealth = 3;          // ���Ѫ��
    public int currentHealth;          // ��ǰѪ��

    // �������
    private Renderer enemyRenderer;   // ��Ⱦ���
    private Collider enemyCollider;    // ��ײ���

    private Vector3 startPos;          // ��ʼλ��
    private Vector3 targetPos;         // ��ǰĿ��λ��
    private bool isDead = false;       // �Ƿ�����

    // Start is called before the first frame update
    void Start()
    {

        // ��ȡ�������
        enemyRenderer = GetComponent<Renderer>();
        enemyCollider = GetComponent<Collider>();

        // ������ʼλ��
        startPos = transform.position;
        // ��ʼ��Ѫ��
        currentHealth = maxHealth;
        // ���õ�һ�����Ŀ���
        SetNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        // �����ƶ�����
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        // �ƶ�
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // ����Ƿ񵽴�Ŀ���
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            SetNewTarget();
        }
    }

    // �����µ����Ŀ���
    void SetNewTarget()
    {
        // ���ε���Χ�����ѡ��һ����
        Vector2 randomPoint = Random.insideUnitCircle * roamRange;
        targetPos = startPos + new Vector3(randomPoint.x, 0, randomPoint.y);
    }


    // ��ײ��⣨�ɵ������м���Ƿ��ӵ����У�
    void OnTriggerEnter(Collider other)
    {
        // ����Ƿ��ӵ�����
        if (other.CompareTag("Bullet"))
        {
            // Ӧ���˺���ÿ�λ��м�1��Ѫ��
            TakeDamage(1);

            // �����ӵ�
            Destroy(other.gameObject);
        }
    }

    // �����˺�
    public void TakeDamage(int damage)
    {
        // ����������򲻴���
        if (isDead) return;

        // ����Ѫ��
        currentHealth -= damage;

        // �����ܻ�Ч������˸��
        StartCoroutine(HitEffect());

        // ����Ƿ�����
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // �ܻ�Ч��������˸��
    IEnumerator HitEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = Color.white;
        }
    }

    // ��������
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // ���ÿɼ��Ժ���ײ
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // ��������Э��
        StartCoroutine(Respawn());
    }

    // ����Э��
    IEnumerator Respawn()
    {
        // �ȴ�����ʱ��
        yield return new WaitForSeconds(respawnTime);

        // ���õ���ʼλ��
        transform.position = startPos;

        // ����Ѫ��
        currentHealth = maxHealth;

        // ��������
        isDead = false;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        // �ָ���ɫ
        if (enemyRenderer != null)
            enemyRenderer.material.color = Color.white;

        // ������Ŀ��
        SetNewTarget();
    }
    // ������ʾ
    void OnDrawGizmos()
    {
        // ��ʾ�ƶ�Ŀ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, 0.3f);
        Gizmos.DrawLine(transform.position, targetPos);

        // ��ʾ�ε���Χ
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPos, roamRange);
    }
}
