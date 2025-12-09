using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PopResultPanelManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage; // 用于显示视频画面的RawImage组件

    public Button closeBtn;

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopResultPanel();
          //  CardShopManager.Instance.OnShow();
        });

        Debug.Log("初始化视频播放器...");
        
        try
        {
            // 配置视频播放器的基本属性
            videoPlayer.playOnAwake = false; // 不要在唤醒时自动播放
            videoPlayer.isLooping = false; // 不循环播放
            
            // 配置音频输出
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.SetDirectAudioVolume(0, 1.0f); // 设置音量为100%
            
            Debug.Log("音频输出模式: " + videoPlayer.audioOutputMode);
            
            // 配置渲染目标
            if (rawImage != null)
            {
                Debug.Log("配置渲染目标...");
                
                // 尝试使用不同的渲染模式
                // 先尝试RenderTexture模式
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                
                // 创建一个默认大小的RenderTexture (可以根据需要调整)
                int defaultWidth = 1280;
                int defaultHeight = 720;
                
                // 创建RenderTexture
                RenderTexture renderTexture = new RenderTexture(
                    defaultWidth, 
                    defaultHeight, 
                    16, 
                    RenderTextureFormat.ARGB32);
                
                // 设置VideoPlayer的目标RenderTexture
                videoPlayer.targetTexture = renderTexture;
                
                // 将RenderTexture设置到RawImage上
                rawImage.texture = renderTexture;
                
                Debug.Log("渲染目标配置完成，使用默认分辨率: " + defaultWidth + "x" + defaultHeight);
                Debug.Log("VideoPlayer渲染模式: " + videoPlayer.renderMode);
                Debug.Log("VideoPlayer目标纹理: " + (videoPlayer.targetTexture != null ? "已设置" : "未设置"));
            }
            else
            {
                Debug.LogWarning("RawImage组件未指定且不允许自动创建，视频将不会显示画面");
                
                // 如果没有RawImage，尝试使用CameraFarPlane渲染模式
                videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
                Debug.Log("使用CameraFarPlane渲染模式");
            }
            
            // 添加事件监听器
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.prepareCompleted += OnVideoReady;
            videoPlayer.started += OnVideoStarted;
            videoPlayer.loopPointReached += OnVideoFinished;
            
            Debug.Log("视频播放器初始化完成");
        }
        catch (System.Exception e)
        {
            Debug.LogError("视频播放器初始化异常: " + e.ToString());
            Debug.LogError("异常堆栈: " + e.StackTrace);
        }
    }
    
    // 视频错误事件
    private void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError("视频播放错误: " + message);
    }
    
    // 视频准备完成事件
    private void OnVideoReady(VideoPlayer vp)
    {
        Debug.Log("视频准备完成，开始播放...");
        Debug.Log("视频实际分辨率: " + vp.width + "x" + vp.height);
        
        // 动态调整RenderTexture大小以匹配视频实际分辨率
        if (rawImage != null && vp.targetTexture != null)
        {
            // 销毁旧的RenderTexture
            Destroy(vp.targetTexture);
            
            // 创建与视频分辨率匹配的新RenderTexture
            RenderTexture newRenderTexture = new RenderTexture(
                (int)vp.width, 
                (int)vp.height, 
                16, 
                RenderTextureFormat.ARGB32);
            
            // 更新VideoPlayer和RawImage的RenderTexture
            vp.targetTexture = newRenderTexture;
            rawImage.texture = newRenderTexture;
            
            Debug.Log("RenderTexture已更新为视频实际分辨率");
        }
        
        vp.Play();
    }
    
    // 视频开始播放事件
    private void OnVideoStarted(VideoPlayer vp)
    {
        Debug.Log("视频开始播放");
        Debug.Log("当前渲染模式: " + vp.renderMode);
        Debug.Log("是否有音频: " + vp.audioOutputMode);
        Debug.Log("音频轨道数: " + vp.audioTrackCount);

        // 检查RawImage状态
        if (rawImage != null)
        {
            Debug.Log("RawImage存在: " + rawImage.name);
            Debug.Log("RawImage是否激活: " + rawImage.gameObject.activeInHierarchy);
            Debug.Log("RawImage纹理: " + (rawImage.texture != null ? rawImage.texture.name : "null"));
        }
        else
        {
            Debug.Log("RawImage不存在");
        }
    }
    
    // 视频播放完成事件
    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("视频播放完成");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnShow(string path)
    {
        Debug.Log("显示结果面板，开始加载视频...");
        
        try
        {
            // 在Unity中，Resources.Load<VideoClip>无法直接加载.mp4文件
            // 需要使用VideoPlayer的url属性加载本地视频文件
            
            string videoPath;
            string fullVideoPath;
            
            // 根据不同平台构建正确的视频路径
            #if UNITY_ANDROID
                // 安卓平台：直接使用Application.streamingAssetsPath（已包含jar:file:///前缀）
                videoPath = Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = videoPath;
            #elif UNITY_IPHONE
                // iOS平台：使用StreamingAssets路径，需要file://前缀
                videoPath = "file://" + Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = Application.streamingAssetsPath + "/Videos/" + path;
            #elif UNITY_STANDALONE_WIN
                // Windows平台：使用StreamingAssets路径
                videoPath = Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = videoPath;
            #else
                // 默认平台：使用StreamingAssets路径
                videoPath = Application.streamingAssetsPath + "/Videos/" + path;
                fullVideoPath = videoPath;
            #endif
            
            Debug.Log("当前平台: " + Application.platform);
            Debug.Log("视频文件路径: " + videoPath);
            Debug.Log("完整文件路径: " + fullVideoPath);
            
            videoPlayer.clip = null;
            // 设置视频源类型为URL
            videoPlayer.source = VideoSource.Url;
            // 设置视频URL
            videoPlayer.url = videoPath;
            
            // 准备视频
            Debug.Log("准备播放视频...");
            videoPlayer.Prepare();
        }
        catch (System.Exception e)
        {
            Debug.LogError("视频播放过程中发生异常: " + e.ToString());
            Debug.LogError("异常堆栈: " + e.StackTrace);
        }
    }

    public void OnHide()
    {
        Debug.Log("隐藏结果面板，停止视频播放...");
        
        try
        {
            // 停止视频播放
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                Debug.Log("视频已停止播放");
            }
            
            // 清理视频资源
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = null;
            rawImage.texture = null;

            Debug.Log("视频资源已清理");
        }
        catch (System.Exception e)
        {
            Debug.LogError("停止视频播放时发生异常: " + e.ToString());
            Debug.LogError("异常堆栈: " + e.StackTrace);
        }
    }    
}
