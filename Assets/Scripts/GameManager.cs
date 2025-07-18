using UnityEngine;
using TMPro;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 单例实例

     // 游戏设置分组
    public float gameDuration = 60f; // 游戏总时长（秒）
    public int initialEnemyCount = 5; // 初始敌人数
    public GameObject enemyPrefab; // 敌人预制体
    public Transform[] spawnPoints; // 敌人出生点数组

    // UI引用分组
    public TMP_Text timerText; // 计时器文本
    public TMP_Text scoreText; // 分数文本
    public GameObject gameOverPanel; // 游戏结束面板

    // 游戏状态
    private float currentTime; // 当前剩余时间
    private int score; // 当前分数
    private int activeEnemies; // 当前活动敌人数
    private bool isGameActive = true; // 游戏是否进行中

    void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 确保只有一个实例
    }

    void Start()
    {
        // 初始化游戏状态
        currentTime = gameDuration;
        score = 0;
        // 更新UI显示
        UpdateUI();

        // 生成初始敌人
        SpawnEnemies(initialEnemyCount);
    }

    void Update()
    {
        // 如果游戏结束则跳过更新
        if (!isGameActive) return;

        // 更新游戏时间
        currentTime -= Time.deltaTime;
        // 更新UI
        UpdateUI();

        // 检查游戏是否结束
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver();
        }
    }

    // 敌人被击败时的处理
    public void EnemyKilled(int value)
    {
        // 增加分数
        score += value;
        // 减少活动敌人数
        activeEnemies--;
        // 更新UI
        UpdateUI();

        // 延迟生成新敌人
        StartCoroutine(RespawnEnemy());
    }

    // 重新生成敌人的协程
    IEnumerator RespawnEnemy()
    {
        // 随机等待3-7秒
        yield return new WaitForSeconds(Random.Range(3f, 7f));
        // 生成一个新敌人
        SpawnEnemy();
    }

    // 生成指定数量的敌人
    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
        }
    }

    // 生成单个敌人
    void SpawnEnemy()
    {
        // 检查是否有出生点
        if (spawnPoints.Length == 0) return;

        // 随机选择一个出生点
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // 实例化敌人
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // 设置敌人的初始位置
        EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        if (enemyControl != null)
        {
            enemyControl.startPos = spawnPoint.position;
        }

        // 增加活动敌人数
        activeEnemies++;
    }

    // 更新UI显示
    void UpdateUI()
    {
        // 更新计时器和分数显示
        timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        scoreText.text = $"Score: {score}";
    }

    // 游戏结束处理
    void GameOver()
    {
        // 设置游戏状态为非活动
        isGameActive = false;
    }


}