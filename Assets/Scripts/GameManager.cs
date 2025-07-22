using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    //����UIԪ��
    public GameObject resultPanel; // �������
    public TMP_Text finalScoreText; // ���յ÷���ʾ
    public TMP_InputField nameInputField; // ������������
    public TMP_Text[] rankEntries = new TMP_Text[3]; // ���а�ǰ������ʾ

    // ��Ϸ״̬
    private float currentTime; // ��ǰʣ��ʱ��
    private int score; // ��ǰ����
    private int activeEnemies; // ��ǰ�������
    private bool isGameActive = true; // ��Ϸ�Ƿ������


    // ���а����ݽṹ
    [System.Serializable]
    public class RankEntry
    {
        public string playerName;
        public int score;
    }

    private List<RankEntry> rankings = new List<RankEntry>();
    private const string RankingsKey = "TopRankings"; // �洢����


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

        // �������а�
        LoadRankings();
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
        //// ���ٻ������
        //activeEnemies--;
        // ����UI
        UpdateUI();

        //// �ӳ������µ���
        //StartCoroutine(RespawnEnemy());
    }

    // �������ɵ��˵�Э��
    //IEnumerator RespawnEnemy()
    //{
    //    // ����ȴ�3-7��
    //    yield return new WaitForSeconds(Random.Range(3f, 7f));
    //    // ����һ���µ���
    //    SpawnEnemy();
    //}

    // ����ָ�������ĵ���
    void SpawnEnemies(int count)
    {
        for (int i = 1; i < count; i++)
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

        //// ���ӻ������
        //activeEnemies++;
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

        isGameActive = false;

        // ������Ϸ��壬��ʾ�������
        gameOverPanel.SetActive(false);
        resultPanel.SetActive(true);

        // ��ʾ���յ÷�
        finalScoreText.text = $"Final Score: {score}";

        // ��������
        nameInputField.text = "";
    }

    //�ύ���������а�
    public void SubmitScore()
    {
        string playerName = nameInputField.text;

        // �������Ϊ�գ�ʹ��Ĭ����
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        // ��ӵ����а�
        AddToRankings(playerName, score);

        // �������а�
        SaveRankings();

        // ������ʾ
        ShowRankings();
    }

    // ��ӷ��������а�
    void AddToRankings(string name, int newScore)
    {
        // ��������Ŀ
        RankEntry newEntry = new RankEntry
        {
            playerName = name,
            score = newScore
        };

        // ��ӵ��б�
        rankings.Add(newEntry);

        // ��������������
        rankings.Sort((a, b) => b.score.CompareTo(a.score));

        // ֻ����ǰ����
        if (rankings.Count > 3)
        {
            rankings = rankings.GetRange(0, 3);
        }
    }

    // �������а�����
    void SaveRankings()
    {
        // ���б�ת��ΪJSON
        string json = JsonUtility.ToJson(new RankingsWrapper { entries = rankings });

        // ���浽PlayerPrefs
        PlayerPrefs.SetString(RankingsKey, json);
        PlayerPrefs.Save();
    }

    // �������а�����
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
            // û������ʱ�������б�
            rankings = new List<RankEntry>();
        }

        // ����UI
        ShowRankings();
    }

    // ��UI����ʾ���а�
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

    // ��װ������JSON���л�
    [System.Serializable]
    private class RankingsWrapper
    {
        public List<RankEntry> entries;
    }
}