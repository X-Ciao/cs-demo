using UnityEngine;
using TMPro;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ����ʵ��

     // ��Ϸ���÷���
    public float gameDuration = 60f; // ��Ϸ��ʱ�����룩
    public int initialEnemyCount = 5; // ��ʼ������
    public GameObject enemyPrefab; // ����Ԥ����
    public Transform[] spawnPoints; // ���˳���������

    // UI���÷���
    public TMP_Text timerText; // ��ʱ���ı�
    public TMP_Text scoreText; // �����ı�
    public GameObject gameOverPanel; // ��Ϸ�������

    // ��Ϸ״̬
    private float currentTime; // ��ǰʣ��ʱ��
    private int score; // ��ǰ����
    private int activeEnemies; // ��ǰ�������
    private bool isGameActive = true; // ��Ϸ�Ƿ������

    void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // ȷ��ֻ��һ��ʵ��
    }

    void Start()
    {
        // ��ʼ����Ϸ״̬
        currentTime = gameDuration;
        score = 0;
        // ����UI��ʾ
        UpdateUI();

        // ���ɳ�ʼ����
        SpawnEnemies(initialEnemyCount);
    }

    void Update()
    {
        // �����Ϸ��������������
        if (!isGameActive) return;

        // ������Ϸʱ��
        currentTime -= Time.deltaTime;
        // ����UI
        UpdateUI();

        // �����Ϸ�Ƿ����
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }
    }

    // ���˱�����ʱ�Ĵ���
    public void EnemyKilled(int value)
    {
        // ���ӷ���
        score += value;
        // ���ٻ������
        activeEnemies--;
        // ����UI
        UpdateUI();

        // �ӳ������µ���
        StartCoroutine(RespawnEnemy());
    }

    // �������ɵ��˵�Э��
    IEnumerator RespawnEnemy()
    {
        // ����ȴ�3-7��
        yield return new WaitForSeconds(Random.Range(3f, 7f));
        // ����һ���µ���
        SpawnEnemy();
    }

    // ����ָ�������ĵ���
    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
        }
    }

    // ���ɵ�������
    void SpawnEnemy()
    {
        // ����Ƿ��г�����
        if (spawnPoints.Length == 0) return;

        // ���ѡ��һ��������
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // ʵ��������
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ���õ��˵ĳ�ʼλ��
        EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        if (enemyControl != null)
        {
            enemyControl.startPos = spawnPoint.position;
        }

        // ���ӻ������
        activeEnemies++;
    }

    // ����UI��ʾ
    void UpdateUI()
    {
        // ���¼�ʱ���ͷ�����ʾ
        timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        scoreText.text = $"Score: {score}";
    }

    // ��Ϸ��������
    void GameOver()
    {
        // ������Ϸ״̬Ϊ�ǻ
        isGameActive = false;
    }


}