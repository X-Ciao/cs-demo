using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text[] rankEntries = new TMP_Text[3];

    void Start()
    {
        LoadAndDisplayRankings();
    }

    void LoadAndDisplayRankings()
    {
        List<GameManager.RankEntry> rankings = LoadRankings();

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

    public static List<GameManager.RankEntry> LoadRankings()
    {
        if (PlayerPrefs.HasKey("TopRankings"))
        {
            string json = PlayerPrefs.GetString("TopRankings");
            GameManager.RankingsWrapper wrapper = JsonUtility.FromJson<GameManager.RankingsWrapper>(json);
            return wrapper.entries;
        }
        return new List<GameManager.RankEntry>();
    }

    // 返回主菜单按钮调用
    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0); // 替换为你的主菜单场景名
    }
}