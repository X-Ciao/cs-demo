using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 单例实例

     // 游戏设置分组
    public float gameDuration = 10f; // 游戏总时长（秒）
    public int initialEnemyCount = 5; // 初始敌人数
    public GameObject enemyPrefab; // 敌人预制体
    public Transform[] spawnPoints; // 敌人出生点数组

    // UI引用分组
    public TMP_Text timerText; // 计时器文本
    public TMP_Text scoreText; // 分数文本
    //public GameObject gameOverPanel; // 游戏结束面板

    //结算UI元素
    public GameObject resultPanel; // 结算面板
    public TMP_Text finalScoreText; // 最终得分显示
    public TMP_InputField nameInputField; // 玩家名字输入框
    public TMP_Text[] rankEntries = new TMP_Text[3]; // 排行榜前三名显示

    // 游戏状态
    private float currentTime; // 当前剩余时间
    private int score; // 当前分数
    private int activeEnemies; // 当前活动敌人数
    private bool isGameActive = true; // 游戏是否进行中




    // 排行榜数据结构
    [System.Serializable]
    public class RankEntry
    {
        public string playerName;
        public int score;
    }

    private List<RankEntry> rankings = new List<RankEntry>();
    private const string RankingsKey = "TopRankings"; // 存储键名


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

        //隐藏结算界面
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // 加载排行榜
        LoadRankings();


        // 游戏开始时锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        // 更新UI
        UpdateUI();


    }



    // 生成指定数量的敌人
    void SpawnEnemies(int count)
    {
        for (int i = 1; i < count; i++)
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

        //// 增加活动敌人数
        //activeEnemies++;
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

        // 隐藏游戏面板，显示结算面板
        //gameOverPanel.SetActive(false);
        resultPanel.SetActive(true);

        // 显示最终得分
        finalScoreText.text = $"Final Score: {score}";

        // 清空输入框
        nameInputField.text = "";

        // === 停止游戏活动 ===
        StopAllCoroutines();
        DisablePlayer();
        DisableEnemies();

        // 解锁鼠标用于UI操作
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 切换到UI控制模式
        EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
    }

    // 禁用玩家控制
    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 完全禁用玩家控制脚本
            MonoBehaviour playerControl = player.GetComponent<PlayerControl>();
            if (playerControl != null)
            {
                playerControl.enabled = false;
                // 同时禁用物理运动
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // 完全停止物理模拟
                }
            }

            // 禁用枪械控制脚本
            MonoBehaviour gunControl = player.GetComponent<GunControl>();
            if (gunControl != null) gunControl.enabled = false;
        }
    }

    // 禁用所有敌人
    void DisableEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        if (enemyControl != null)
        {
            enemyControl.enabled = false;

            // 完全停止敌人的物理运动
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // 禁用碰撞体
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider != null) enemyCollider.enabled = false;
        }
    }

    //提交分数到排行榜
    public void SubmitScore()
    {
        string playerName = nameInputField.text;

        // 如果名字为空，使用默认名
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        // 添加到排行榜
        AddToRankings(playerName, score);

        // 保存排行榜
        SaveRankings();

        // 跳转场景
        ShowLeaderboard();
    }

    // 添加分数到排行榜
    void AddToRankings(string name, int newScore)
    {
        // 创建新条目
        RankEntry newEntry = new RankEntry
        {
            playerName = name,
            score = newScore
        };

        // 添加到列表
        rankings.Add(newEntry);

        // 按分数降序排序
        rankings.Sort((a, b) => b.score.CompareTo(a.score));

        // 只保留前三名
        if (rankings.Count > 3)
        {
            rankings = rankings.GetRange(0, 3);
        }
    }

    // 保存排行榜数据
    void SaveRankings()
    {
        // 将列表转换为JSON
        string json = JsonUtility.ToJson(new RankingsWrapper { entries = rankings });

        // 保存到PlayerPrefs
        PlayerPrefs.SetString(RankingsKey, json);
        PlayerPrefs.Save();
    }

    // 加载排行榜数据
    void LoadRankings()
    {
        if (PlayerPrefs.HasKey(RankingsKey))
        {
            string json = PlayerPrefs.GetString(RankingsKey);
            RankingsWrapper wrapper = JsonUtility.FromJson<RankingsWrapper>(json);
            rankings = wrapper.entries;
        }
        else
        {
            // 没有数据时创建空列表
            rankings = new List<RankEntry>();
        }

        // 更新UI
        ShowRankings();
    }

    // 在UI上显示排行榜
    void ShowRankings()
    {
        for (int i = 0; i < rankEntries.Length; i++)
        {
            if (i < rankings.Count)
            {
                rankEntries[i].text = $"{i + 1}. {rankings[i].playerName} - {rankings[i].score}";
            }
            else
            {
                rankEntries[i].text = $"{i + 1}. ---";
            }
        }
    }

   

    //跳转场景
    public void ShowLeaderboard()
    {

        // 加载排行榜场景
        SceneManager.LoadScene("LeaderboardScene");
    }

    // 包装类用于JSON序列化
    [System.Serializable]
    public class RankingsWrapper
    {
        public List<RankEntry> entries;
    }
}