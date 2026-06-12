
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.音频管理单例
 *
 *  Author:			
 *       
 *  Date:              
 * 
 ******************************************************************************/


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : DDOLSingleton<AudioManager>
{
    private float _bgmVolumScale = 1f;                                          //背景音乐声音大小比例
    private float _effVolumScale = 0.5f;                                        //音效声音大小比例
    public List<AudioSource> AudioSourceList = new List<AudioSource>();         //音效播放器
    private int _audioSourceNum = 20;                                           //音效播放器个数
    private AudioSource _musicAudio;                                            //背景音乐播放器
    private Dictionary<string, AudioClip> _audioDic;                            //音效文件缓存
    AudioClip bgclip ;
    public override void Init()
    {

    }

    private void Awake()
    {
        _audioDic = new Dictionary<string, AudioClip>();
        gameObject.AddComponent<AudioListener>();
        if (PlayerPrefs.HasKey("bgmVolumScale"))
            _bgmVolumScale = PlayerPrefs.GetFloat("bgmVolumScale");
        if (PlayerPrefs.HasKey("effVolumScale"))
            _effVolumScale = PlayerPrefs.GetFloat("effVolumScale");
        if (PlayerPrefs.HasKey(Defines.AUDIOVOLUME))
            _effVolumScale = PlayerPrefs.GetFloat(Defines.AUDIOVOLUME);
        _musicAudio = gameObject.AddComponent<AudioSource>();
        _musicAudio.playOnAwake = false;
        _musicAudio.loop = true;
        _musicAudio.volume = _bgmVolumScale;
        initAudioSources();
        bgclip = Resources.Load<AudioClip>("Audio/bg/bg");
      
    }

    void Start()
    {
        //Debug.Log(_bgmVolumScale);
        PlayMusic(bgclip, _bgmVolumScale);
    }

    public void PlayBgMusic()
    {
        PlayMusic(bgclip, _bgmVolumScale);
    }

    //初始化as组件播放器
    private void initAudioSources()
    {
        for (var i = 0; i < _audioSourceNum; i++)
        {
            var obj = new GameObject("AudioSource" + (i + 1));
            obj.transform.SetParent(transform);
            var audio = obj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            AudioSourceList.Add(audio);
        }
    }

    //解暂停
    public void UnPause()
    {
        _musicAudio.UnPause();
        foreach (var item in AudioSourceList)
            item.UnPause();
    }

    //暂停所有声音
    public void Pause()
    {
        _musicAudio.Pause();
        foreach (var item in AudioSourceList)
            item.Pause();
    }

    //停止所有声音
    public void Stop()
    {
        _musicAudio.Stop();
        foreach (var item in AudioSourceList)
            item.Stop();
    }

    //设置静音
    public void SetMute(bool isMute)
    {
        _musicAudio.mute = isMute;
        foreach (var item in AudioSourceList)
            item.mute = isMute;
    }

    /// <summary>
    /// 设置背景音乐
    /// </summary>
    /// <param name="scale">音乐大小[0,1]</param>
    public void SetBGMVolumScale(float scale)
    {
        _bgmVolumScale = scale;
        _musicAudio.volume = _bgmVolumScale;
        PlayerPrefs.SetFloat("bgmVolumScale", _bgmVolumScale);
    }

    /// <summary>
    /// 设置音效
    /// </summary>
    /// <param name="scale">音效大小[0,1]</param>
    public void SetEFFVolumScale(float scale)
    {
        _effVolumScale = scale;
        PlayerPrefs.SetFloat("effVolumScale", _effVolumScale);
    }

    //获取背景音乐音量[0,1]
    public float GetBGMVolumScale()
    {
        return _bgmVolumScale;
    }

    //获取音效音量[0,1]
    public float GetEFFVolumScale()
    {
        return _effVolumScale;
    }

    //是否正在播放music
    public bool IsPlayMusic(string music)
    {
        if (_musicAudio.isPlaying)
            return _musicAudio.clip.name == music;
        return false;
    }

    //是否正在播放背景音乐
    public bool IsPlayMusic()
    {
        return _musicAudio.isPlaying;
    }

    //是否正在播放音效
    public bool IsPlaySoundEff(string eff)
    {
        return AudioSourceList.Any(audio => audio.isPlaying && audio.clip.name == eff);
    }

    //停止背景音乐
    public void StopMusic()
    {
        _musicAudio.Stop();
    }

    //停止所有音效
    public void StopAllSoudEff()
    {
        foreach (var item in AudioSourceList)
            item.Stop();
    }

    /// <summary>
    /// 停止某音效
    /// </summary>
    /// <param name="eff">音效名称</param>
    public void StopSoundEff(string eff)
    {
        foreach (var item in AudioSourceList)
        {
            if (item.isPlaying && item.clip.name == eff)
                item.Stop();
        }
    }

    /// <summary>
    /// 停止某音效
    /// </summary>
    /// <param name="clip">音效</param>
    public void StopSoundEff(AudioClip clip)
    {
        StopSoundEff(clip.name);
    }

    //播放背景音乐
    public void PlayMusic(string music, float volume = 1)
    {
        if (IsPlayMusic(music))
            return;
        var data = GetClipByName(music, false);
        if (ReferenceEquals(null, data)) return;
        PlayMusic(data, volume);
    }


    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip">音频</param>
    public void PlayMusic(AudioClip clip, float volume)
    {
        _musicAudio.clip = clip;
        _musicAudio.volume =  volume;
        _musicAudio.Play();
    }

    /// <summary>
    /// 播放2d音效
    /// </summary>
    /// <param name="eff">音效名称</param>
    /// <param name="loop">是否循环</param>
    public void PlaySoundEff2D(string eff, bool loop = false, float volume = 1)
    {
        var data = GetClipByName(eff, true);
        if (ReferenceEquals(null, data)) return;
        PlaySoundEff2D(data, loop, volume);
    }

    /// <summary>
    /// 播放2d音效
    /// </summary>
    /// <param name="clip">音频</param>
    /// <param name="loop">是否循环</param>
    public void PlaySoundEff2D(AudioClip clip, bool loop = false, float volume = 1)
    {
        if (clip == null)
            return;
        AudioSource audioSrc = GetCurrentAudioSource();
        audioSrc.loop = loop;
        audioSrc.clip = clip;
        audioSrc.volume = _effVolumScale * volume;
        audioSrc.Play();
    }

    //获取当前可用播放器
    private AudioSource GetCurrentAudioSource()
    {
        AudioSource audioSrc = null;
        for (int i = 0, imax = AudioSourceList.Count; i < imax; i++)
        {
            if (AudioSourceList[i].isPlaying == false)
                audioSrc = AudioSourceList[i];
        }

        return audioSrc ?? (audioSrc = AudioSourceList[0]);
    }

    /// <summary>
    /// 根据字符串找到clip
    /// </summary>
    /// <param name="path">音效地址</param>
    /// <param name="cache">是否缓存</param>
    /// <returns></returns>
    public AudioClip GetClipByName(string path, bool cache)
    {
        if (_audioDic.ContainsKey(path))
            return _audioDic[path];
        var clip = ResManager.Instance.Load<AudioClip>(path);
        if (!clip) return null;
        _audioDic.Add(path, clip);
        return clip;
    }
}

