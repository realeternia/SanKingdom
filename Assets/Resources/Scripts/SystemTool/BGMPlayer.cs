using UnityEngine;


public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance { get; private set; }
    public AudioSource audioSBGM;
    public AudioSource audioSSound;

    private void Awake()
    {
        Instance = this;
    }

    private string lastBGMPath = "";
    private AudioClip lastBGMClip = null;

    public void PlayBGM(string path)
    {
        AudioSource audioSource = audioSBGM;
        if (lastBGMPath != path)
        {
            lastBGMPath = path;
            lastBGMClip = Resources.Load<AudioClip>(path);
            if (lastBGMClip != null)
            {
                audioSource.clip = lastBGMClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    // 静态变量记录上次播放路径和 clip
    private string lastPath = "";
   private AudioClip lastClip = null;

    private int lastSoundPriority = -1;
    private float lastSoundTime = 0f;

    public void PlaySound(string path, int prioty = 3)
    {
        float currentTime = Time.time;
        // 如果当前优先级低于上一次且时间间隔小于1秒，则跳过播放
        if (prioty < lastSoundPriority && currentTime - lastSoundTime < 1.5f)
        {
            return;
        }

        // 更新上次播放信息
        lastSoundPriority = prioty;
        lastSoundTime = currentTime;
    
        AudioSource audioSource = audioSSound;
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

      
}