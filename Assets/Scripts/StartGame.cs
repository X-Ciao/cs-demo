using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    public GameObject ImagePr;
    //获取组件
    private Image m_progress;

    // 场景索引常量
    private const int GAME_SCENE_INDEX = 1;    //游戏场景索引
    private const int RANKING_SCENE_INDEX = 2; // 排行榜场景索引

    private void Awake()
    {
        m_progress = ImagePr.GetComponent<Image>();
    }

    public void StartScene()
    {
        StartCoroutine(LoadScene(GAME_SCENE_INDEX));
    }

    // 新增排行榜跳转方法
    public void GoToRanking()
    {
        StartCoroutine(LoadScene(RANKING_SCENE_INDEX));
    }
    IEnumerator LoadScene(int sceneIndex)
    {
        //Image加载条的数值
        int disableProgress = 0;
        //真正场景的加载条值
        int toprogress = 0;

        //浮点数比较的epsilon值,可以自行设置阈值
        const float epsilon = 0.0001f;

        //切换场景
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        //暂时不切换
        op.allowSceneActivation = false;
        //进度条加载至百分之90
        while (Mathf.Abs(op.progress - 0.9f) > epsilon)
        {
            toprogress = (int)(op.progress * 100);
            while (disableProgress < toprogress)
            {
                ++disableProgress;
                m_progress.fillAmount = disableProgress / 100.0f;
                yield return new WaitForEndOfFrame();
            }
        }

        //进度条剩余百分之10的加载
        toprogress = 100;
        while(disableProgress < toprogress)
        {
            ++disableProgress;
            m_progress.fillAmount = disableProgress / 100.0f;
            yield return new WaitForEndOfFrame();
        }


        // 确保进度条显示为100%
        m_progress.fillAmount = 1.0f;

        // 等待一小段时间让用户看到100%的进度条
        yield return new WaitForSeconds(0.1f);

        //切换场景
        op.allowSceneActivation = true;
    }
}
