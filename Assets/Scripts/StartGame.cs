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

    // ������������
    private const int GAME_SCENE_INDEX = 1;    //��Ϸ��������
    private const int RANKING_SCENE_INDEX = 2; // ���а񳡾�����

    private void Awake()
    {
        m_progress = ImagePr.GetComponent<Image>();
    }

    public void StartScene()
    {
        StartCoroutine(LoadScene(GAME_SCENE_INDEX));
    }

    // �������а���ת����
    public void GoToRanking()
    {
        StartCoroutine(LoadScene(RANKING_SCENE_INDEX));
    }
    IEnumerator LoadScene(int sceneIndex)
    {
        //Image����������ֵ
        int disableProgress = 0;
        //���������ļ�����ֵ
        int toprogress = 0;

        //�������Ƚϵ�epsilonֵ,��������������ֵ
        const float epsilon = 0.0001f;

        //�л�����
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        //��ʱ���л�
        op.allowSceneActivation = false;
        //�������������ٷ�֮90
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

        //������ʣ��ٷ�֮10�ļ���
        toprogress = 100;
        while(disableProgress < toprogress)
        {
            ++disableProgress;
            m_progress.fillAmount = disableProgress / 100.0f;
            yield return new WaitForEndOfFrame();
        }


        // ȷ����������ʾΪ100%
        m_progress.fillAmount = 1.0f;

        // �ȴ�һС��ʱ�����û�����100%�Ľ�����
        yield return new WaitForSeconds(0.1f);

        //�л�����
        op.allowSceneActivation = true;
    }
}
