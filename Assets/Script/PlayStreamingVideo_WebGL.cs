using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class PlayOnActive_WebGL_Min : MonoBehaviour
{
    [Tooltip("位于 Assets/StreamingAssets 的文件名，如 movie.mp4")]
    public string fileName = "movie.mp4";

    [Tooltip("是否循环播放")]
    public bool loop = true;

    private VideoPlayer vp;
    private Coroutine co;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        if (vp == null) vp = gameObject.AddComponent<VideoPlayer>();

        vp.source = VideoSource.Url;
        vp.playOnAwake = false;
        vp.renderMode = VideoRenderMode.RenderTexture; // 需要在UI里播就改为 RenderTexture
        vp.skipOnDrop = true;
        vp.isLooping = loop;

        // 不用 AudioSource，直接输出并开启音轨0（否则可能没声）
        vp.audioOutputMode = VideoAudioOutputMode.Direct;
        vp.EnableAudioTrack(0, true);
    }

    void OnEnable()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(PlayRoutine());
    }

    void OnDisable()
    {
        if (co != null) { StopCoroutine(co); co = null; }
        if (vp != null) vp.Stop();
    }

    IEnumerator PlayRoutine()
    {
        // WebGL: 直接用相对URL，不要加 file://
        string url = Path.Combine(Application.streamingAssetsPath, fileName).Replace("\\", "/");
        vp.url = url;
        vp.isLooping = loop;

        vp.Prepare();
        while (!vp.isPrepared) yield return null;

        vp.Play();
    }
}
