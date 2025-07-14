using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    public GameObject ImagePr;
    //��ȡ���
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
        //Image����������ֵ
        int disableProgress = 0;
        //���������ļ�����ֵ
        int toprogress = 0;

        //�л�����
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        //��ʱ���л�
        op.allowSceneActivation = false;
        //�������������ٷ�֮90
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

        //������ʣ��ٷ�֮10�ļ���
        toprogress = 100;
        while(disableProgress < toprogress)
        {
            ++disableProgress;
            m_progress.fillAmount = disableProgress / 100.0f;
            yield return new WaitForEndOfFrame();
        }

        //�л�����
        op.allowSceneActivation = true;
    }
}
