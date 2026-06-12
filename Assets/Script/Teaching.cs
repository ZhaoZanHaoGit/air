using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using System;

public class Teaching : MonoBehaviour
{

    public Questions questions;
    public UIDocument UIDocument;
    public Camera MainCamera;
    public PlayableDirector teachingDirector;
    public GameObject AnimUI, PlayUI;
    private UIDocument document;
    private Button finishButton, NextBtn, submitButton;
    private RadioButtonGroup anwserGroup;
    private Label WarningText;
    private int currentQuestionIdx = 0;

    private int[] userAnswers = null;
    private int userTmpAnswer = -1;
    private PlayableDirector currentDirector = null;

    void Start()
    { }

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        document.enabled = true;
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

        renderScore();
        renderQuestion(currentQuestionIdx);
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
        document.enabled = false;
    }


    void OnSubmitClicked()
    {
        userAnswers[currentQuestionIdx] = userTmpAnswer;
        renderScore();
        refreshStatus();


        var q = questions.getIndex(currentQuestionIdx);

        // timeline
        playTimeline(q.correctDirector);
    }

    void OnFinishClicked()
    {
        UIDocument.GetComponent<UIScript>().enabled = true;
        this.enabled = false;
    }

    QuestionStruct renderQuestion(int idx)
    {
        var root = document.rootVisualElement;
        var q = questions.getIndex(idx);

        anwserGroup.value = userAnswers[idx]; // default -1
        refreshStatus();

        // timeline
        playTimeline(q.showDirector);


        // render Process
        TextElement progressText = root.Q<Label>("ProgressText");
        progressText.text = (idx + 1) + " / " + questions.getLength();

        // render Question
        TextElement questionText = root.Q<Label>("QuestionText");
        questionText.text = "问题" + (idx + 1) + "：" + q.question;

        var block = root.Q<VisualElement>("unity-radio-button-group__container");
        block.Clear();
        for (int i = 0; i < q.answers.Length; i++)
        {
            var radio = new RadioButton();
            radio.text = ((char)('A' + i)).ToString() + ". " + q.answers[i];
            radio.name = "radio" + i;
            radio.value = userAnswers[idx] == i;
            block.Add(radio);
        }

        return q;
    }

    void renderScore()
    {
        var root = document.rootVisualElement;

        int score = 0;
        for (int i = 0; i < userAnswers.Length; i++)
        {
            if (userAnswers[i] == questions.getIndex(i).correctAnswer)
            {
                score++;
            }
        }

        var scoreText = root.Q<Label>("ScoreText");
        scoreText.text = score + " / " + questions.getLength();
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
        NextBtn.SetEnabled(isSubmitted && !isLastQuestion);

        // submitButton style
        submitButton.SetEnabled(isCanSubmit);

        // WarningText
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

    void onChangeAnswer(int index)
    {
        userTmpAnswer = index;
        refreshStatus();
    }

    void OnNextClicked()
    {
        currentQuestionIdx++;
        renderQuestion(currentQuestionIdx);
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
            WarningText.style.display = DisplayStyle.None;
            return;
        }

        WarningText.style.display = DisplayStyle.Flex;
        WarningText.text = text;
        WarningText.ClearClassList();
        WarningText.AddToClassList(type + "_text");
    }
}
