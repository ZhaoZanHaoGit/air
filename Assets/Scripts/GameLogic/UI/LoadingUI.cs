using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using TMPro;

public class LoadingUI : BaseUI
{
    public override bool EscapeClose => false;
    protected override int UIOrder => 110;
    private float loadingSpeed = 5f;
    #region UI组件

    private GameObject _rectLoadScene, _rectLoadUI;
    private GameObject rectLoadScene => _rectLoadScene ??= UnityHelper.FindTheChild(gameObject, "rectLoadScene").gameObject;
    private GameObject rectLoadUI => _rectLoadUI ??= UnityHelper.FindTheChild(gameObject, "rectLoadUI").gameObject;
    private Animator _loadUIAnimator;
    private Animator loadUIAnimator =>
        _loadUIAnimator ??= UnityHelper.GetTheChildComponent<Animator>(gameObject, "rectLoadUI");

    private Slider _sliderUI;
    public Slider sliderUI => _sliderUI ??= UnityHelper.GetTheChildComponent<Slider>(gameObject, "slider");
    private TextMeshProUGUI _txtLoading, _txtTip;
    public TextMeshProUGUI txtLoading => _txtLoading ??= UnityHelper.GetTheChildComponent<TextMeshProUGUI>(gameObject, "txtLoading");
    public TextMeshProUGUI txtTip => _txtTip ??= UnityHelper.GetTheChildComponent<TextMeshProUGUI>(gameObject, "tip");

    #endregion
    private AsyncOperation oper;
    private float sliderValue = 0;
    private EnumUIType openUI = EnumUIType.None;
    private string sceneName;
    private bool isLoadScene;
    private string loadStr = "Loading";
    private bool firstOpen = true;
    private int openCloudId = Animator.StringToHash("openCloud");
    private float delay = 0f;
    private string tempParameter;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.LoadingUI;
    }

    protected override void OnStart()
    {
        base.OnStart();
        EventTriggerListener.IsLoading = true;
        if (isLoadScene)
        {
            rectLoadScene.SetActive(true);
            StartCoroutine(loadScene());
            StartCoroutine(refreshLoadingTxt());
        }
        else
        {
            rectLoadUI.SetActive(true);
            StartCoroutine(loadUI());
        }
        AudioManager.Instance.StopAllSoudEff();
    }

    private IEnumerator loadScene()
    {
        oper = SceneManager.LoadSceneAsync(sceneName);      
        yield return oper;
    }
    private IEnumerator refreshLoadingTxt()
    {
        var count = 1;
        while (true)
        {
            var str = "";
            for (var i = 0; i < count; i++)
            {
                str += ".";
            }
            txtLoading.text = loadStr + str;
            count = (count + 1) % 4;
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator loadSceneUI()
    {
        while (true)
        {
            if (isLoadScene)
            {
                if (oper != null && oper.isDone && sliderValue == 100)
                    break;
            }
            else if (sliderValue == 100)
            {
                break;
            }
            if (firstOpen && sliderValue > 10 && openUI != EnumUIType.None)
            {
                firstOpen = false;           
            }       
            if (sliderValue < 94)
                sliderValue += UnityEngine.Random.Range(3, 5);
            else
                sliderValue = 100;
            sliderUI.value = sliderValue;

            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(delay);
        CloseUIToBeOpenUI(openUI);
    }
   
    void Update()
    {
        if (oper == null)
        {
            return;
        }
        sliderValue = oper.progress;
        if (oper.progress >= 0.9f)
        {
            sliderValue = 1.0f;
        }
        if (sliderValue != sliderUI.value)
        {
            sliderUI.value = Mathf.Lerp(sliderUI.value, sliderValue, Time.deltaTime * loadingSpeed);
            if (Mathf.Abs(sliderUI.value - sliderValue) < 0.01f)
            {
                sliderUI.value = sliderValue;
            }
        }
        //numberText.text = ((int)(processBar.value * 100)).ToString() + "%";
        if ((int)(sliderUI.value * 100) >= 100)
        {
            //允许异步加载完毕后自动切换场景
            oper.allowSceneActivation = true;
            CloseUIToBeOpenUI(openUI, tempParameter);
        }
    }
    private IEnumerator loadUI()
    {
        var animClips = loadUIAnimator.runtimeAnimatorController.animationClips;
        yield return new WaitForSeconds(animClips[0].length);
        closeOthersUI();
        UIManager.Instance.OpenUI(openUI);
        yield return UIManager.Instance.FindUIByUIType(openUI);
        loadUIAnimator.SetBool(openCloudId, true);
        yield return new WaitForSeconds(animClips[1].length);
        CloseUI();
    }

    private void closeOthersUI()
    {
        var openUIs = UIManager.Instance.GetDicOpenUIs().Keys.ToList();
        foreach (var key in openUIs)
        {
            if (key != EnumUIType.LoadingUI)
                UIManager.Instance.CloseUI(key);
        }
    }

    //
    protected override void SetUI(params object[] uiParams)
    {
        if (uiParams.Length > 0)
        {
            isLoadScene = (bool)uiParams[0];
            openUI = (EnumUIType)uiParams[1];
            if (uiParams.Length>3)
            {
                tempParameter = (string)uiParams[3];
            }
            if (isLoadScene)
                sceneName = uiParams[2].ToString();
        }
        else
            CloseUI();
        base.SetUI(uiParams);
    }


    protected override void OnRelease()
    {

        EventTriggerListener.IsLoading = false;
    }
}
