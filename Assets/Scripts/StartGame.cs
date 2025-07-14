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

    private void Awake()
    {
        m_progress = ImagePr.GetComponent<Image>();
    }

    public void StartScene()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        //Image加载条的数值
        int disableProgress = 0;
        //真正场景的加载条值
        int toprogress = 0;

        //切换场景
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        //暂时不切换
        op.allowSceneActivation = false;
        //进度条加载至百分之90
        while(op.progress < 0.9)
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

        //切换场景
        op.allowSceneActivation = true;
    }
}
