using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System;
using Controls.Utils;

public class VideoPanelManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    public GameObject videoPanel;

    void Start()
    {
        GameLog.Debug("初始化视频播放器...");
        
        try
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.SetDirectAudioVolume(0, 1.0f);
            
            GameLog.Debug("音频输出模式: " + videoPlayer.audioOutputMode);
            
            if (rawImage != null)
            {
                GameLog.Debug("配置渲染目标...");
                
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                
                int defaultWidth = 1280;
                int defaultHeight = 720;
                
                RenderTexture renderTexture = new RenderTexture(
                    defaultWidth, 
                    defaultHeight, 
                    16, 
                    RenderTextureFormat.ARGB32);
                
                videoPlayer.targetTexture = renderTexture;
                
                rawImage.texture = renderTexture;
                
                GameLog.Debug("渲染目标配置完成，使用默认分辨率: " + defaultWidth + "x" + defaultHeight);
                GameLog.Debug("VideoPlayer渲染模式: " + videoPlayer.renderMode);
                GameLog.Debug("VideoPlayer目标纹理: " + (videoPlayer.targetTexture != null ? "已设置" : "未设置"));
            }
            else
            {
                GameLog.Warn("RawImage组件未指定且不允许自动创建，视频将不会显示画面");
                
                videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
                GameLog.Debug("使用CameraFarPlane渲染模式");
            }
            
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.prepareCompleted += OnVideoReady;
            videoPlayer.started += OnVideoStarted;
            videoPlayer.loopPointReached += OnVideoFinished;
            
            GameLog.Debug("视频播放器初始化完成");
        }
        catch (System.Exception e)
        {
            GameLog.Error("视频播放器初始化异常: " + e.ToString());
            GameLog.Error("异常堆栈: " + e.StackTrace);
        }
    }
    
    private void OnVideoError(VideoPlayer vp, string message)
    {
        GameLog.Error("视频播放错误: " + message);
    }
    
    private void OnVideoReady(VideoPlayer vp)
    {
        GameLog.Debug("视频准备完成，开始播放...");
        GameLog.Debug("视频实际分辨率: " + vp.width + "x" + vp.height);
        
        if (rawImage != null && vp.targetTexture != null)
        {
            Destroy(vp.targetTexture);
            
            RenderTexture newRenderTexture = new RenderTexture(
                (int)vp.width, 
                (int)vp.height, 
                16, 
                RenderTextureFormat.ARGB32);
            
            vp.targetTexture = newRenderTexture;
            rawImage.texture = newRenderTexture;
            
            GameLog.Debug("RenderTexture已更新为视频实际分辨率");
        }
        
        vp.Play();
    }
    
    private void OnVideoStarted(VideoPlayer vp)
    {
        GameLog.Debug("视频开始播放");
        GameLog.Debug("当前渲染模式: " + vp.renderMode);
        GameLog.Debug("是否有音频: " + vp.audioOutputMode);
        GameLog.Debug("音频轨道数: " + vp.audioTrackCount);

        if (rawImage != null)
        {
            GameLog.Debug("RawImage存在: " + rawImage.name);
            GameLog.Debug("RawImage是否激活: " + rawImage.gameObject.activeInHierarchy);
            GameLog.Debug("RawImage纹理: " + (rawImage.texture != null ? rawImage.texture.name : "null"));
        }
        else
        {
            GameLog.Debug("RawImage不存在");
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        GameLog.Debug("视频播放完成");
        videoPanel.SetActive(false);
        Hide();
    }

    void Update()
    {

    }

    public void Play(string path)
    {
        InitVideo(path);
    }

    private void InitVideo(string path)
    {
        try
        {
            string videoPath;
            string fullVideoPath;

#if UNITY_ANDROID
            videoPath = Application.streamingAssetsPath + "/Videos/" + path;
            fullVideoPath = videoPath;
#elif UNITY_IPHONE
                videoPath = "file://" + Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = Application.streamingAssetsPath + "/Videos/" + path;
#elif UNITY_STANDALONE_WIN
                videoPath = Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = videoPath;
#else
                videoPath = Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = videoPath;
#endif

            GameLog.Debug("当前平台: " + Application.platform);
            GameLog.Debug("视频文件路径: " + videoPath);
            GameLog.Debug("完整文件路径: " + fullVideoPath);

            videoPlayer.clip = null;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoPath;

            GameLog.Debug("准备播放视频...");
            videoPanel.SetActive(true);
            videoPlayer.Prepare();
        }
        catch (System.Exception e)
        {
            GameLog.Error("视频播放过程中发生异常: " + e.ToString());
            GameLog.Error("异常堆栈: " + e.StackTrace);
        }
    }

    private System.Collections.IEnumerator HideVideoPanelAfterDelay(float delaySeconds)
    {
        GameLog.Debug("开始等待隐藏videoPanel，延迟时间: " + delaySeconds + "秒");

        yield return new WaitForSeconds(delaySeconds);

        videoPanel.SetActive(false);
        Hide();
    }

    private void Hide()
    {
        GameLog.Debug("隐藏结果面板，停止视频播放...");
        
        try
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                GameLog.Debug("视频已停止播放");
            }
            
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = null;
            rawImage.texture = null;

            GameLog.Debug("视频资源已清理");
        }
        catch (System.Exception e)
        {
            GameLog.Error("停止视频播放时发生异常: " + e.ToString());
            GameLog.Error("异常堆栈: " + e.StackTrace);
        }
    }    
}
