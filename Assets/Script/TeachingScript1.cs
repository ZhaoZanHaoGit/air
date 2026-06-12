using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HighlightPlus;
using UnityEngine.Rendering;



public class TeachingScript1 : MonoBehaviour
{

    public Questions questions;
    // public UIDocument UIDocument;
    public Camera MainCamera;
    public PlayableDirector teachingDirector;
    public LoginManager loginManager;
    public UIScript1 script1;
    public GameObject hide, show,hideimage;
    public GameObject AnimUI, PlayUI,liang,guan;
    private Quaternion defultAngle;
    public Transform anserOptions, scoreShow;
    public TextMeshProUGUI hideBTNtext, scoreText, ProgressInfoText, WarningText, questionText, examTittleText;
    //  private UIDocument document;
    public Button finishButton, NextBtn, lastBtn, submitButton, hideBtn;
    //  private RadioButtonGroup anwserGroup;
    // private Label WarningText;
    private int currentQuestionIdx = 0;
    private int LastFinishIndex = 0;

    private int[] userAnswers = null;
    private int userTmpAnswer = -1;
    private PlayableDirector currentDirector = null;
    /// <summary>
    /// 答题菜单显示状态
    /// </summary>
    private bool panelstate;
    int currentscore;
    public PhpDataManager phpDataManager;

    private void Awake()
    {

        Debug.Log("绑定了21212121");
        foreach (Toggle toggle in anserOptions.GetComponentsInChildren<Toggle>())
        {
            Debug.Log("绑定了：" + toggle.name + "事件");
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    int selectvalue = int.Parse(toggle.name);
                    onChangeAnswer(selectvalue);
                    Debug.Log("选中了：" + toggle.name);
                    submitButton.interactable = true;
                }
            });
        }
        submitButton.onClick.AddListener(OnSubmitClicked);
        NextBtn.onClick.AddListener(OnNextClicked);
        lastBtn.onClick.AddListener(onLastClicked);
        finishButton.onClick.AddListener(HidePanel);
        finishButton.onClick.AddListener(finish);
    }

    void Start()
    {
Debug.Log(DateTime.Now.ToString());
    }
    void finish()
    {

        scoreShow.gameObject.SetActive(true);
        scoreShow.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentscore.ToString();
        LoginManager.user.Score= currentscore.ToString();
        LoginManager.user.Date=DateTime.Now.ToString();
        StartCoroutine(phpDataManager.SubmitData(LoginManager.user.name, LoginManager.user.id, LoginManager.user.department, LoginManager.user.phonenumber,currentscore));

    }
    public void togglepanelShow()
    {
        if (this.enabled)
        {
            HidePanel();
        }
        else
        {
            showpanel();
        }
    }


    void OnEnable()
    {
        // document = GetComponent<UIDocument>();
        // document.enabled = true;
        teachingDirector.time = teachingDirector.initialTime;
        teachingDirector.Evaluate();

        // reset
        currentQuestionIdx = 0;
        MainCamera.GetComponent<CameraMove>().maxHeight = 80.0f; // 修改相机可移动的最高位置


        // reset user answers
        userAnswers = new int[questions.getLength()];
        for (int i = 0; i < userAnswers.Length; i++)
        {
            userAnswers[i] = -1;
        }


        // 
        /*
                var root = document.rootVisualElement;
                submitButton = root.Q<Button>("SubmitBtn");
                submitButton.clicked += OnSubmitClicked;

                finishButton = root.Q<Button>("FinishBtn");
                finishButton.clicked += OnFinishClicked;

                NextBtn = root.Q<Button>("NextBtn");
                NextBtn.clicked += OnNextClicked;

                anwserGroup = root.Q<RadioButtonGroup>("Anwsers");
                anwserGroup.RegisterValueChangedCallback(evt => onChangeAnswer(evt.newValue));

                WarningText = root.Q<Label>("WarningText");
        */
        renderScore();
        renderQuestion(currentQuestionIdx);
    }
    //-166.29,173.7
    void showpanel()
    {
        show.gameObject.SetActive(true);
        hideimage.gameObject.SetActive(false);
        hide.gameObject.SetActive(false);
        AnimUI.SetActive(false);
        PlayUI.SetActive(false);
        liang.SetActive(true); 
        guan.SetActive(true);
        //que7Go.SetActive(false);
        //显示面板的时候记录当前的角度，并且修改角度为CLosed
       // defultAngle = doorAngle.transform.rotation;
        //doorAngle.transform.eulerAngles = new Vector3(-90,0,0);
        hideBTNtext.text = "收起";
        script1.OnTeachingClicked();
        transform.DOBlendableLocalMoveBy(new Vector3(-173.7f - 166.29f, 0, 0), 0.5f);
        this.enabled = true;

    }
    void HidePanel()
    {
        show.gameObject.SetActive(false);
        hide.gameObject.SetActive(true);
        hideimage.gameObject.SetActive(true);
        hideBTNtext.text = "答题";
        AnimUI.SetActive(true);
        PlayUI.SetActive(true);
        liang.SetActive(false);
        guan.SetActive(false);
        //que7Go.SetActive(true);
        //doorAngle.transform.rotation= defultAngle;
        transform.DOBlendableLocalMoveBy(new Vector3(173.7f + 166.29f, 0, 0), 0.5f); ;
        OnFinishClicked();
        currentQuestionIdx = 0;
        LastFinishIndex = 0;
        Array.Clear(userAnswers, 0, userAnswers.Length);
    }


    public void setNoTeachAnimate()
    {
        teachingDirector.time = 0;
        teachingDirector.Evaluate();
    }

    void OnDisable()
    {
        MainCamera.GetComponent<CameraMove>().maxHeight = 20.0f; // 修改相机可移动的最高位置
        setNoTeachAnimate();
        // document.enabled = false;
    }


    void OnSubmitClicked()
    {


        if (currentQuestionIdx == LastFinishIndex)
        {
            userAnswers[currentQuestionIdx] = userTmpAnswer;
            renderScore();
        }
        closeAllHighlight();
        refreshStatus();


        var q = questions.getIndex(currentQuestionIdx);

        // timeline
        playTimeline(q.correctDirector);
        //   if (LastFinishIndex == questions.getLength() - 1)
        // { Invoke("finish", 2f); }

    }

    void OnFinishClicked()
    {
        //   UIDocument.GetComponent<UIScript>().enabled = true;
        // script1.enabled = true;
        this.enabled = false;
    }

    QuestionStruct renderQuestion(int idx)
    {
        //  var root = document.rootVisualElement;
        var q = questions.getIndex(idx);

        //  anwserGroup.value = userAnswers[idx]; // default -1
        refreshStatus();

        // timeline
        playTimeline(q.showDirector);


        // render Process
        // TextElement progressText = root.Q<Label>("ProgressText");
        ProgressInfoText.text = "进度：" + (idx + 1) + " / " + questions.getLength();
        examTittleText.text = $"选择题  已答数量{(idx + 1)} 个（共{questions.getLength()}题）";
        // render Question
        // TextElement questionText = root.Q<Label>("QuestionText");
        questionText.text = "问题" + (idx + 1) + "：" + q.question;

        // var block = root.Q<VisualElement>("unity-radio-button-group__container");
        //  block.Clear();
        for (int i = 0; i < anserOptions.childCount; i++)
        {
            anserOptions.GetChild(i).gameObject.SetActive(false);
            anserOptions.GetChild(i).GetComponent<Toggle>().isOn = false;
        }
        for (int i = 0; i < q.answers.Length; i++)
        {
            anserOptions.GetChild(i).gameObject.SetActive(true);
            anserOptions.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ((char)('A' + i)).ToString() + ". " + q.answers[i];
            //anserOptions.GetChild(i).name= "radio" + i;
            /*
            var radio = new RadioButton();
            radio.text = ((char)('A' + i)).ToString() + ". " + q.answers[i];
            radio.name = "radio" + i;
            radio.value = userAnswers[idx] == i;
            block.Add(radio);*/
        }

        return q;
    }

    void renderScore()
    {
        //  var root = document.rootVisualElement;

        int score = 0;
        for (int i = 0; i < userAnswers.Length; i++)
        {
            if (userAnswers[i] == questions.getIndex(i).correctAnswer)
            {
                if (i < 20)
                { score += 4; }
                else
                { score += 2; }

            }
        }
        currentscore = score;
        // var scoreText = root.Q<Label>("ScoreText");
        scoreText.text = "得分:" + score + " / " + 100;
        // ProgressInfoText.text = "进度：" + currentQuestionIdx + " / " + questions.getLength();
    }

    void refreshStatus()
    {
        var q = questions.getIndex(currentQuestionIdx);
        bool isSubmitted = userAnswers[currentQuestionIdx] != -1;
        bool isLastQuestion = currentQuestionIdx == questions.getLength() - 1;
        bool isFinished = isLastQuestion && isSubmitted;
        bool isCanSubmit = userTmpAnswer != -1 && !isSubmitted;
        bool isCorrect = userAnswers[currentQuestionIdx] == questions.getIndex(currentQuestionIdx).correctAnswer;

        // NextBtn style
        //  NextBtn.SetEnabled(isSubmitted && !isLastQuestion);
        NextBtn.interactable = (isSubmitted && !isLastQuestion);
        // submitButton style
        // submitButton.SetEnabled(isCanSubmit);

        // WarningText
        submitButton.interactable = (isCanSubmit);
        if (!isSubmitted)
        {
            setTip();
        }
        else
        {
            var text = isCorrect ? "回答正确" : "回答错误";
            text += "，" + (q.explain == null ? "请看正确演示。" : q.explain);
            if (isFinished)
            {
                text += "\n已完成所有题目，点击退出按钮返回主界面。";
            }
            setTip(text, isCorrect ? "info" : "warn");
        }

    }


    void closeAllHighlight()
    {
        var scene = SceneManager.GetActiveScene();
        var results = new List<HighlightEffect>(1024);

        foreach (var root in scene.GetRootGameObjects())
        {
            // true = 包含未激活物体
            results.AddRange(root.GetComponentsInChildren<HighlightEffect>(true));
        }
        foreach (var root in results)
        {
            root.highlighted = false;

        }
    }


    public void onChangeAnswer(int index)
    {
        userTmpAnswer = index;
        refreshStatus();
    }
    void onLastClicked()
    {
        if (currentQuestionIdx > 0)
        {
            closeAllHighlight();
            currentQuestionIdx--;
            renderQuestion(currentQuestionIdx);
        }


    }
    void OnNextClicked()
    {

        currentQuestionIdx++;
        if (LastFinishIndex < currentQuestionIdx)
        { LastFinishIndex = currentQuestionIdx; }
        renderQuestion(currentQuestionIdx);
        submitButton.interactable = false;
        closeAllHighlight();
    }

    void playTimeline(string objectName)
    {
        // first at all, stop last director
        if (currentDirector)
        {
            currentDirector.time = 0;
            currentDirector.Evaluate();
        }

        PlayableDirector director = null;

        // find director
        try
        {
            director = GameObject.Find(objectName).GetComponent<PlayableDirector>();
        }
        catch (Exception e)
        {
            director = null;
            Debug.Log("<color=red>Error 1010: </color>" + e + "[" + objectName + "]");
        }

        if (director == null) return;

        currentDirector = director;
        director.time = director.initialTime;
        director.Evaluate();
    }

    void setTip(string text = null, string type = "info")
    {
        if (text == null)
        {
            //    WarningText.style.display = DisplayStyle.None;
            WarningText.text = "";
            return;
        }

        //  WarningText.style.display = DisplayStyle.Flex;
        WarningText.text = text;
        if (type == "info")
        {
            WarningText.color = Color.green;
        }
        else { WarningText.color = Color.red; }
        //  WarningText.ClearClassList();
        //  WarningText.AddToClassList(type + "_text");
    }
}
