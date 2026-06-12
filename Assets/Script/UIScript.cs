using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
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

  [SerializeField]
  private UIDocument teachingDocument;

  UIDocument m_UIDocument;
  Button playButton, pauseButton, teachingBtn;

  [SerializeField]
  private PlayableDirector battleModeTimeline;

  [SerializeField]
  private PlayableDirector battleOverTimeline;

  Button ventilateRejectButton, ventilateClearButton, ventilateFilterButton, ventilateDefaultButton;

  Button battleModeButton, battleOverButton;


  void OnEnable()
  {
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
    var teachingScript = teachingDocument.GetComponent<Teaching>();
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
    battleOverButton.clicked += OnBattleOverClicked;
  }

  void OnDisable()
  {
    m_UIDocument.enabled = false;
  }

  void resetAll()
  {
    stopAllVentilateTimelines(); // 通风模式关闭
    MainCamera.orthographic = false; // 地图正交模式关闭
    OnPauseClicked(); //浏览模式暂停
  }
  void OnMapIconClicked()
  {
    resetAll();

    var Overview = CameraGroup.transform.Find("Overview");
    MainCamera.transform.position = Overview.position;
    MainCamera.transform.rotation = Overview.rotation;
    MainCamera.orthographic = true;
    MainCamera.orthographicSize = 300;
  }

  void OnPlayClicked()
  {
    resetAll();

    MainCamera.orthographic = false;
    director.Play();
    playButton.style.display = DisplayStyle.None;
    pauseButton.style.display = DisplayStyle.Flex;
  }

  void OnPauseClicked()
  {
    director.Pause();
    playButton.style.display = DisplayStyle.Flex;
    pauseButton.style.display = DisplayStyle.None;
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
    ventilateRejectTimeline.Play();
    setVentilateBtnHighlight(ventilateRejectButton);
    ventilateCameraTimeline.Play();
  }

  // 通风：清洁
  void OnVentilateClearClicked()
  {
    resetAll();
    ventilateClearTimeline.Play();
    setVentilateBtnHighlight(ventilateClearButton);
    ventilateCameraTimeline.Play();
  }

  // 通风：滤毒
  void OnVentilateFilterClicked()
  {
    resetAll();
    ventilateFilterTimeline.Play();
    setVentilateBtnHighlight(ventilateFilterButton);
    ventilateCameraTimeline.Play();
  }

  void OnTeachingClicked()
  {
    resetAll();
    var teachingScript = teachingDocument.GetComponent<Teaching>();
    teachingScript.enabled = true;

    this.enabled = false;
  }
  
  void stopAllBattleTimeline()
  {
    battleModeTimeline.Stop();
    battleOverTimeline.Stop();
  }

  void setBattleButton(Button button)
  {
    battleModeButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleModeButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleOverButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
    battleOverButton.style.backgroundColor = new Color(0f, 0f, 0f, 0f);

    button.style.backgroundColor = new Color(0.2117f, 0.5647f, 0.9137f, 1f);
    button.style.color = new Color(1f, 1f, 1f, 1f);
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
