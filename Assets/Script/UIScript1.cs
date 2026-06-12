using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class UIScript1 : MonoBehaviour
{
    [SerializeField]
    private Camera MainCamera;

    [SerializeField]
    private GameObject CameraGroup;

    [SerializeField]
    private PlayableDirector director;

    [SerializeField]
    private PlayableDirector ventilateFilterTimeline;

    [SerializeField]
    private PlayableDirector ventilateClearTimeline;

    [SerializeField]
    private PlayableDirector ventilateRejectTimeline;

    [SerializeField]
    private PlayableDirector ventilateDefaultTimeline;

    [SerializeField]
    private PlayableDirector ventilateCameraTimeline;



    //播放,暂停,考试菜单
    public Button playButton, pauseButton, teachingBtn, replayBtn, stopBtn, animcloseBtn;

    [SerializeField]
    private PlayableDirector battleModeTimeline;

    [SerializeField]
    private PlayableDirector battleOverTimeline;
    //通风的 隔绝，清除，过滤，默认
    [Header("通风的 隔绝，清除，过滤，默认")]
    public Button ventilateRejectButton, ventilateClearButton, ventilateFilterButton, ventilateDefaultButton;
    //排风的  隔绝，清除，过滤，默认
    [Header("排风的  隔绝，清除，过滤，默认")]
    public Button PFRejectButton, PFClearButton, PFFilterButton, PFDefaultButton;

    //战时，平时
    public Button battleModeButton, battleOverButton;
    public Button mapIcon;
    public Sprite sprite3D, sprite2D;
    public GameObject minimapUI;
    bool mapmode = false;
    public TongfengShow tongfengShow;
    public LvDuShow LvDuShow;
    public qingjieShow qingjieShow;
    public LvDu_P LvDu_P, gejue;
    public qingjie_p qingjie_P;
    public Animation tongfengStart, paifengStart;
    Vector3 cameratransform;
    Quaternion camerarotation;
    void OnEnable()
    {
        //绑定地图
        mapIcon.onClick.AddListener(OnMapIconClicked);
        //绑定播放
        playButton.onClick.AddListener(OnPlayClicked);
        pauseButton.onClick.AddListener(OnPauseClicked);
        replayBtn.onClick.AddListener(OnReplayClicked);
        stopBtn.onClick.AddListener(onstopplayClicked);
        //绑定通风
        ventilateRejectButton.onClick.AddListener(OnVentilateRejectClicked);
        ventilateFilterButton.onClick.AddListener(OnVentilateFilterClicked);
        ventilateClearButton.onClick.AddListener(OnVentilateClearClicked);
        ventilateDefaultButton.onClick.AddListener(OnVentilateDefaultClicked);
        //绑定排风
        PFRejectButton.onClick.AddListener(OnPFRejectClicked);
        PFClearButton.onClick.AddListener(OnPFClearClicked);
        PFFilterButton.onClick.AddListener(OnPFFilterClicked);

        //绑定战时
        battleModeButton.onClick.AddListener(OnBattleModeClicked);
        battleOverButton.onClick.AddListener(OnBattleOverClicked);
        //绑定关闭UI
        animcloseBtn.onClick.AddListener(stopandCloseAnim);
        //绑定答题
        // teachingBtn.onClick.AddListener(OnTeachingClicked);
        /*
    m_UIDocument = GetComponent<UIDocument>();
    m_UIDocument.enabled = true;
        

    var root = m_UIDocument.rootVisualElement;
    var mapIcon = root.Q<Button>("MapButton");
    mapIcon.clicked += OnMapIconClicked;

    playButton = root.Q<Button>("PlayButton");
    playButton.clicked += OnPlayClicked;

    pauseButton = root.Q<Button>("PauseButton");
    pauseButton.clicked += OnPauseClicked;

    var replayButton = root.Q<Button>("ReplayButton");
    replayButton.clicked += OnReplayClicked;

    // reset
    var teachingScript = teachingDocument.GetComponent<TeachingScript>();
    teachingScript.setNoTeachAnimate();


    // Ventilate
    ventilateRejectButton = root.Q<Button>("VentilateRejectButton");
    ventilateRejectButton.clicked += OnVentilateRejectClicked;

    ventilateClearButton = root.Q<Button>("VentilateClearButton");
    ventilateClearButton.clicked += OnVentilateClearClicked;

    ventilateFilterButton = root.Q<Button>("VentilateFilterButton");
    ventilateFilterButton.clicked += OnVentilateFilterClicked;

    ventilateDefaultButton = root.Q<Button>("VentilateDefaultButton");
    ventilateDefaultButton.clicked += OnVentilateDefaultClicked;

    // 进入教学演示模式
    teachingBtn = root.Q<Button>("TeachingBtn");
    teachingBtn.clicked += OnTeachingClicked;
    battleModeButton = root.Q<Button>("BattleModeButton");
    battleModeButton.clicked += OnBattleModeClicked;
    battleOverButton = root.Q<Button>("BattleOverButton");
    battleOverButton.clicked += OnBattleOverClicked;*/
    }

    void stopandCloseAnim()
    {
        animcloseBtn.transform.parent.gameObject.SetActive(false);
        tongfengStart.Stop();
        tongfengStart.gameObject.SetActive(false);
        paifengStart.Stop();
        paifengStart.gameObject.SetActive(false);
        for (int i = 0; i < animcloseBtn.transform.parent.childCount - 1; i++)
        {
            if (animcloseBtn.transform.parent.GetChild(i).gameObject.activeSelf)
            {
                if (animcloseBtn.transform.parent.GetChild(i).gameObject.TryGetComponent<IPaifengAnimSet>(out var target))
                { target.endplay(); }
            }
        }



    }

    void OnDisable()
    {

    }

    public void resetAll()
    {
        stopandCloseAnim();
        stopAllVentilateTimelines(); // 通风模式关闭
        MainCamera.orthographic = false; // 地图正交模式关闭
        OnPauseClicked();

        //浏览模式暂停
    }
   public void OnMapIconClicked()
    {
        resetAll();
        if (mapmode)
        {
            minimapUI.SetActive(false);
            mapIcon.GetComponent<Image>().sprite=sprite2D;
            mapmode = false;
            // MainCamera.transform.position = cameratransform;
            // MainCamera.transform.rotation = camerarotation;
        }
        else
        {
            minimapUI.SetActive(true);
            /*
            cameratransform = MainCamera.transform.position;
            camerarotation = MainCamera.transform.rotation;
            var Overview = CameraGroup.transform.Find("Overview");
            MainCamera.transform.position = Overview.position;
            MainCamera.transform.rotation = Overview.rotation;
            MainCamera.orthographic = true;
            MainCamera.orthographicSize = 300;*/
            mapmode = true;
            mapIcon.GetComponent<Image>().sprite = sprite3D;

        }

    }

    void OnPlayClicked()
    {
        resetAll();

        MainCamera.orthographic = false;
        director.Play();
        /*
    playButton.style.display = DisplayStyle.None;
    pauseButton.style.display = DisplayStyle.Flex;
        */
    }
   public void onstopplayClicked()
    {
        //director.Stop();
        pauseButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        //transform.GetChild(0).localPosition = new Vector3(-704.3247f, transform.GetChild(0).localPosition.y, transform.GetChild(0).localPosition.z);
       // transform.GetChild(2).localPosition = new Vector3(1133.7f, transform.GetChild(2).localPosition.y, transform.GetChild(2).localPosition.z);
    }


    void OnPauseClicked()
    {
        director.Pause();
        /*
    playButton.style.display = DisplayStyle.Flex;
    pauseButton.style.display = DisplayStyle.None;
        */
    }

    void OnReplayClicked()
    {
        director.time = 0;
        OnPlayClicked();
    }


    // 停止所有通风动画, 并重置按钮样式
    void stopAllVentilateTimelines()
    {
        setVentilateBtnHighlight(ventilateDefaultButton);

        ventilateFilterTimeline.time = 0;
        ventilateFilterTimeline.Stop();
        ventilateFilterTimeline.Evaluate();

        ventilateClearTimeline.time = 0;
        ventilateClearTimeline.Stop();
        ventilateClearTimeline.Evaluate();

        ventilateRejectTimeline.time = 0;
        ventilateRejectTimeline.Stop();
        ventilateRejectTimeline.Evaluate();

        ventilateDefaultTimeline.time = 0;
        ventilateDefaultTimeline.Stop();
        ventilateDefaultTimeline.Evaluate();

    }

    // 通风：修改高亮样式
    void setVentilateBtnHighlight(Button button)
    {
        /*
    ventilateRejectButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    ventilateClearButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    ventilateFilterButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    ventilateDefaultButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    ventilateRejectButton.style.color = new Color(0f, 0f, 0f, 1f);
    ventilateClearButton.style.color = new Color(0f, 0f, 0f, 1f);
    ventilateFilterButton.style.color = new Color(0f, 0f, 0f, 1f);
    ventilateDefaultButton.style.color = new Color(0f, 0f, 0f, 1f);

    button.style.backgroundColor = new Color(0.2117f, 0.5647f, 0.9137f, 1f);
    button.style.color = new Color(1f, 1f, 1f, 1f);
        */
    }

    // 通风：不开启
    void OnVentilateDefaultClicked()
    {
        resetAll();
        ventilateDefaultTimeline.Play();
        setVentilateBtnHighlight(ventilateDefaultButton);
        ventilateCameraTimeline.Play();
    }

    // 通风：隔绝
    void OnVentilateRejectClicked()
    {
        resetAll();
        // ventilateRejectTimeline.Play();
        setVentilateBtnHighlight(ventilateRejectButton);
        tongfengStart.gameObject.SetActive(true);
       tongfengStart.Play();
        tongfengShow.gameObject.SetActive(true);
        // ventilateCameraTimeline.Play(); 
        Invoke("funa", 4f);
        animcloseBtn.transform.parent.gameObject.SetActive(true);
    }

    void funa()
    {

        tongfengShow.doanim = true;

    }

    // 通风：清洁
    void OnVentilateClearClicked()
    {
        resetAll();
        animcloseBtn.transform.parent.gameObject.SetActive(true);
        // ventilateClearTimeline.Play();
        setVentilateBtnHighlight(ventilateClearButton);
        tongfengStart.gameObject.SetActive(true);
        tongfengStart.Play();
        qingjieShow.gameObject.SetActive(true);
        //ventilateCameraTimeline.Play();
        Invoke("funa1", 3f);

    }
    void funa1()
    {

        qingjieShow.doanim = true;

    }
    // 通风：滤毒
    void OnVentilateFilterClicked()
    {
        resetAll();
        animcloseBtn.transform.parent.gameObject.SetActive(true);
        //ventilateFilterTimeline.Play();//timelin动画
        setVentilateBtnHighlight(ventilateFilterButton);
        tongfengStart.gameObject.SetActive(true);
        tongfengStart.Play();
        LvDuShow.gameObject.SetActive(true);
        Invoke("funa2",3f);
        //ventilateCameraTimeline.Play();//timelin动画
    }
    void funa2()
    {

        LvDuShow.doanim = true;
        // animcloseBtn.transform.parent.gameObject.SetActive(true);
    }
    // 排风：隔绝
    void OnPFRejectClicked()
    {
        resetAll();
        animcloseBtn.transform.parent.gameObject.SetActive(true);
        // ventilateRejectTimeline.Play();
        setVentilateBtnHighlight(ventilateRejectButton);
        paifengStart.gameObject.SetActive(true);
        paifengStart.Play();
        gejue.gameObject.SetActive(true);
        // ventilateCameraTimeline.Play(); 
        Invoke("funa3", 3f);
    }
    void funa3()
    {

        gejue.doanim = true;
        // animcloseBtn.transform.parent.gameObject.SetActive(true);
    }
    // 排风：清洁
    void OnPFClearClicked()
    {
        resetAll();
        animcloseBtn.transform.parent.gameObject.SetActive(true);
        // ventilateClearTimeline.Play();
        setVentilateBtnHighlight(ventilateClearButton);
        paifengStart.gameObject.SetActive(true);
        paifengStart.Play();
        qingjie_P.gameObject.SetActive(true);
        //ventilateCameraTimeline.Play();
        Invoke("funa4", 3f);
    }
    void funa4()
    {
        //animcloseBtn.transform.parent.gameObject.SetActive(true);

        qingjie_P.doanim = true;

    }
    // 排风：滤毒
    void OnPFFilterClicked()
    {
        resetAll();
        animcloseBtn.transform.parent.gameObject.SetActive(true);
        //ventilateFilterTimeline.Play();//timelin动画
        setVentilateBtnHighlight(ventilateFilterButton);
        //ventilateCameraTimeline.Play();//timelin动画
        paifengStart.gameObject.SetActive(true);
        paifengStart.Play();
        LvDu_P.gameObject.SetActive(true);
        Invoke("funa5", 3f);
    }
    void funa5()
    {

        LvDu_P.doanim = true;
        // animcloseBtn.transform.parent.gameObject.SetActive(true);
    }
    public void OnTeachingClicked()
    {
        resetAll();
        /*
    var teachingScript = teachingDocument.GetComponent<TeachingScript>();
    teachingScript.enabled = true;
        */
        //this.enabled = false;
    }

    void stopAllBattleTimeline()
    {
        battleModeTimeline.Stop();
        battleOverTimeline.Stop();
    }

    void setBattleButton(Button button)
    {/*
    battleModeButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleModeButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleOverButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleOverButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);

    button.style.backgroundColor = new Color(0.2117f, 0.5647f, 0.9137f, 1f);
    button.style.color = new Color(1f, 1f, 1f, 1f);
        */
    }

    // 战时
    void OnBattleModeClicked()
    {
        OnPauseClicked(); //浏览模式暂停
        stopAllBattleTimeline();
        battleModeTimeline.Play();
        setBattleButton(battleModeButton);
    }

    void OnBattleOverClicked()
    {
        OnPauseClicked(); //浏览模式暂停
        stopAllBattleTimeline();
        battleOverTimeline.Play();
        setBattleButton(battleOverButton);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
public interface IPaifengAnimSet
{

    void endplay();

}