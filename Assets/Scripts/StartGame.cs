using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    public GameObject ImagePr;
    //获取组件
    private Image m_progress;

    // 对战模式选择UI
    public GameObject multiplayerPanel; // 对战模式选择面板
    public Button hostButton;          // 作为Host进入按钮
    public Button serverButton;        // 作为Server进入按钮
    public Button clientButton;        // 作为Client进入按钮
    public Button backButton;          // 返回主菜单按钮

    // 场景索引常量
    private const int GAME_SCENE_INDEX = 1;    //游戏场景索引
    private const int RANKING_SCENE_INDEX = 2; // 排行榜场景索引
    private const int MULTIPLAYER_SCENE_INDEX = 3; // 对战模式场景索引

    //// 网络设置
    //public static NetworkMode SelectedNetworkMode { get; private set; } // 选择的网络模式
    //public enum NetworkMode { None, Host, Server, Client } // 网络模式枚举

    private void Awake()
    {
        m_progress = ImagePr.GetComponent<Image>();

        // 确保对战模式选择面板初始隐藏
        if (multiplayerPanel != null)
            multiplayerPanel.SetActive(false);

        //// 设置按钮回调
        //if (hostButton != null) hostButton.onClick.AddListener(StartAsHost);
        //if (serverButton != null) serverButton.onClick.AddListener(StartAsServer);
        //if (clientButton != null) clientButton.onClick.AddListener(StartAsClient);
        //if (backButton != null) backButton.onClick.AddListener(ReturnToMainMenu);

        //// 初始化网络模式
        //SelectedNetworkMode = NetworkMode.None;
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

    // 进入对战模式选择
    public void StartMultiplayer()
    {

        StartCoroutine(LoadScene(MULTIPLAYER_SCENE_INDEX));
        //if (multiplayerPanel != null)
        //{
        //    multiplayerPanel.SetActive(true);
        //}
    }

    // 新增：作为Host进入
    //public void StartAsHost()
    //{
    //    SelectedNetworkMode = NetworkMode.Host;
    //    StartCoroutine(LoadScene(MULTIPLAYER_SCENE_INDEX));
    //}

    //// 作为Server进入
    //public void StartAsServer()
    //{
    //    SelectedNetworkMode = NetworkMode.Server;
    //    StartCoroutine(LoadScene(MULTIPLAYER_SCENE_INDEX));
    //}

    //// 作为Client进入
    //public void StartAsClient()
    //{
    //    //NetworkManager.Singleton.StartClient();
    //    SelectedNetworkMode = NetworkMode.Client;
    //    StartCoroutine(LoadScene(MULTIPLAYER_SCENE_INDEX));
    //}

    //// 返回主菜单
    //public void ReturnToMainMenu()
    //{
    //    if (multiplayerPanel != null)
    //    {
    //        multiplayerPanel.SetActive(false);
    //    }
    //}


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
