using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ����ʵ��

    // ��Ϸ���÷���
    public float gameDuration = 10f; // ��Ϸ��ʱ�����룩
    public int initialEnemyCount = 5; // ��ʼ������
    public GameObject enemyPrefab; // ����Ԥ����
    public Transform[] spawnPoints; // ���˳���������

    // UI���÷���
    public TMP_Text timerText; // ��ʱ���ı�
    public TMP_Text scoreText; // �����ı�

    //����UIԪ��
    public GameObject resultPanel; // �������
    public TMP_Text finalScoreText; // ���յ÷���ʾ
    public TMP_InputField nameInputField; // ������������
    public TMP_Text[] rankEntries = new TMP_Text[3]; // ���а�ǰ������ʾ

    // ��Ϸ״̬
    private float currentTime; // ��ǰʣ��ʱ��
    private int score; // ��ǰ����
    private int activeEnemies; // ��ǰ�������
    public bool isGameActive = true; // ��Ϸ�Ƿ������
    public bool isPaused = false; // ��Ϸ�Ƿ���ͣ

    // ������ǹе����
    private GunControl playerGun;

    //��ͣ�˵�
    public GameObject pauseMenu; // ��ͣ�˵����
    public string mainMenuScene = "MainMenu"; // ���˵���������



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
        {
            Instance = this;
        }
            
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

        //���ؽ������
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // �������а�
        LoadRankings();


        // ��Ϸ��ʼʱ�������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ��ȡ��ҵ�ǹе���
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerGun = player.GetComponentInChildren<GunControl>();
            if (playerGun == null)
                Debug.LogError("GunControl component not found on player!");
        }
        else Debug.LogError("Player object not found!");
    }

    void Update()
    {
        // �����Ϸ��������������
        if (!isGameActive) return;

        // �ӵ��ľ����
        if (playerGun != null && playerGun.IsOutOfAmmo())
        {
            GameOver();
        }

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

        // ���ESC������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // ��Ϸ��ͣʱ��������Ϸ�߼�
        if (isPaused || !isGameActive) return;
    }

    // ���˱�����ʱ�Ĵ���
    public void EnemyKilled(int value)
    {
        // ���ӷ���
        score += value;

        
        // ����UI
        UpdateUI();


    }



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

    }

    // ����UI��ʾ
    void UpdateUI()
    {
        // ���¼�ʱ���ͷ�����ʾ
        timerText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
        scoreText.text = $"Score: {score}";
    }

    // �л���ͣ״̬
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // ��ͣ��Ϸ
    void PauseGame()
    {
        Time.timeScale = 0f; // ֹͣ��Ϸʱ��
        pauseMenu.SetActive(true); // ��ʾ��ͣ�˵�

        // �������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    // ������Ϸ
    public void ResumeGame()
    {
        Time.timeScale = 1f; // �ָ���Ϸʱ��
        pauseMenu.SetActive(false); // ������ͣ�˵�

        // �����������
        if (isGameActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    // �������˵�
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // ȷ��ʱ��ָ�
        SceneManager.LoadScene(0);
    }

    // ��Ϸ��������
    void GameOver()
    {
        //������Ϸ״̬Ϊ�ǻ
        isGameActive = false;

        //������Ϸ��壬��ʾ�������
        resultPanel.SetActive(true);

        //��ʾ���յ÷�
        finalScoreText.text = $"Final Score: {score}";

        //��������
        nameInputField.text = "";

        //ֹͣ��Ϸ�
        StopAllCoroutines();
        DisablePlayer();
        DisableEnemies();

        // �����������UI����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // �л���UI����ģʽ
        EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
    }

    // ������ҿ���
    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // ���� PlayerControl
            MonoBehaviour playerControl = player.GetComponent<PlayerControl>();
            if (playerControl != null) playerControl.enabled = false;

            // ���� Head �ű�
            Head headControl = player.GetComponentInChildren<Head>();
            if (headControl != null) headControl.enabled = false;

            // ���� GunControl
            MonoBehaviour gunControl = player.GetComponent<GunControl>();
            if (gunControl != null) gunControl.enabled = false;

            // ֹͣ�����˶�
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    // �������е���
    void DisableEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        if (enemyControl != null)
        {
            enemyControl.enabled = false;

            // ��ȫֹͣ���˵������˶�
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // ������ײ��
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider != null) enemyCollider.enabled = false;
        }
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

        // ��ת����
        ShowLeaderboard();
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


    }

 

   

    //��ת����
    public void ShowLeaderboard()
    {

        // �������а񳡾�
        SceneManager.LoadScene("LeaderboardScene");
    }

    // ��װ������JSON���л�
    [System.Serializable]
    public class RankingsWrapper
    {
        public List<RankEntry> entries;
    }
}