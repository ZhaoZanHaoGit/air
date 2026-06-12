using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BaseUI
{
    private Slider _masterVolume;
    private Slider masterVolume => _masterVolume ??= UnityHelper.GetTheChildComponent<Slider>(CacheGameObject, "MasterVolumeSlider");
    private TMP_Dropdown _resolutionDropdown;
    private TMP_Dropdown resolutionDropdown => _resolutionDropdown ?? UnityHelper.GetTheChildComponent<TMP_Dropdown>(CacheGameObject, "ResolutionRatioDrop");
    private Toggle _fullScreenToggle;
    private Toggle fullScreenToggle => _fullScreenToggle ?? UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "FullScreenToggle");
    private Button _closedBut;
    private Button closedBut => _closedBut ?? UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "ClosedBut");
    private Button _saveBut;
    private Button saveBut => _saveBut ?? UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "saveButton");

    public Resolution[] resolutions;
    private List<string> resolutionOptions = new List<string>();
    private CanvasGroup _canvasGroup;
    private CanvasGroup canvasGroup=>_canvasGroup?? UnityHelper.GetTheChildComponent<CanvasGroup>(CacheGameObject, "canvasGrounp");
    private TMP_Text _masterVolumeText;
    private TMP_Text masterVolumeText=> _masterVolumeText ?? UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "MasterVolume");
    public override EnumUIType GetUIType()
    {
        return EnumUIType.SettingPanel;
    }
    protected override void OnStart()
    {
        base.OnStart();
        EventTriggerListener.Get(closedBut).SetEventHandle(EnumTouchEventType.OnClick, OnClosedPanel, null);
        EventTriggerListener.Get(saveBut).SetEventHandle(EnumTouchEventType.OnClick, OnSaveClick, null);
        EventTriggerListener.Get(fullScreenToggle).SetEventHandle(EnumTouchEventType.OnClick, OnFullScreenClick, null);
        masterVolume.onValueChanged.AddListener(value =>
        {
            OnMasterVolume(value);
        });
        resolutionDropdown.onValueChanged.AddListener(arg => {
            OnResolutionRatio(arg);
        });
        InitResolution();
        if (PlayerPrefs.HasKey(Defines.AUDIOVOLUME)) {
            OnMasterVolume(PlayerPrefs.GetFloat(Defines.AUDIOVOLUME));
            masterVolume.value = PlayerPrefs.GetFloat(Defines.AUDIOVOLUME);
        }
        if (PlayerPrefs.HasKey(Defines.FULLSCREEN)) {
            fullScreenToggle.isOn = PlayerPrefs.GetInt(Defines.FULLSCREEN) == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey(Defines.RESOLUTION))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt(Defines.RESOLUTION);
        }
    }

    private void OnFullScreenClick(GameObject listener, object eventData, object[] args)
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void OnSaveClick(GameObject listener, object eventData, object[] args)
    {
        PlayerPrefs.SetFloat(Defines.AUDIOVOLUME, masterVolume.value);
        int I= fullScreenToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(Defines.FULLSCREEN, I);
        PlayerPrefs.SetInt(Defines.RESOLUTION, resolutionDropdown.value);
        CloseUI();
    }

    private void OnClosedPanel(GameObject listener, object eventData, object[] args)
    {
        //Debug.Log("触发了关闭按钮行为");
        CloseUI();
    }
    /// <summary>
    /// 设置设置界面的音量
    /// </summary>
    private void OnMasterVolume(float volume)
    {
        masterVolumeText.text = (volume * 100).ToString("0") + "%";
        AudioManager.Instance.SetEFFVolumScale(volume);
    }
    /// <summary>
    /// 修改分辨率
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="eventData"></param>
    /// <param name="args"></param>
    private void OnResolutionRatio(int aValue)
    {
        Resolution resolution = resolutions[aValue];
        Debug.Log($"resolution.width :  {resolution.width}, resolution.height:  {resolution.height}");
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void InitResolution()
    {
        // 获取支持的分辨率
        Resolution r0 = new Resolution { width = 1280, height = 720 };
        Resolution r1 = new Resolution { width = 1600, height = 900 };
        Resolution r2 = new Resolution { width = 1920, height = 1080 };
        resolutions = new Resolution[] { r0, r1, r2 };
        // 清空 Dropdown 选项
        resolutionDropdown.ClearOptions();
        // 将分辨率添加到 Dropdown 选项
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);
        }
        // 更新 Dropdown 的选项列表
        resolutionDropdown.AddOptions(resolutionOptions);
        // 设置当前分辨率为默认选项
        int currentResolutionIndex = GetCurrentResolutionIndex();
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    // 获取当前分辨率的索引
    private int GetCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0; // 默认返回第一个分辨率
    }
    protected override void OnPlayOpenUIAnimation()
    {
        canvasGroup.DOFade(1, 0.25f);
    }
    protected override void OnPlayCloseUIAnimation()
    {
        canvasGroup.DOFade(0, 0.25f);
    }
}
