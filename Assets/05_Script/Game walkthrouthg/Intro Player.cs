using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    [Header("影片播放器 (UI Canvas 上的 VideoPlayer)")]
    public VideoPlayer introVideoPlayer;

    [Header("背景模糊 Panel (UI Image)")]
    public Image blurPanel;

    [Header("模糊強度")]
    [Range(0f, 1f)]
    public float blurAlpha = 0.5f;

    void Awake()
    {
        // 一開始隱藏影片與模糊背景
        if (introVideoPlayer != null)
            introVideoPlayer.gameObject.SetActive(false);

        if (blurPanel != null)
        {
            var color = blurPanel.color;
            color.a = 0f;
            blurPanel.color = color;
            blurPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 播放 Intro 並模糊背景
    /// </summary>
    public void ShowIntro()
    {
        // 顯示模糊 Panel
        if (blurPanel != null)
        {
            blurPanel.gameObject.SetActive(true);
            var color = blurPanel.color;
            color.a = blurAlpha;
            blurPanel.color = color;
        }

        // 播放影片
        if (introVideoPlayer != null)
        {
            introVideoPlayer.gameObject.SetActive(true);
            introVideoPlayer.Play();
        }
    }

    /// <summary>
    /// 結束 Intro
    /// </summary>
    public void HideIntro()
    {
        if (introVideoPlayer != null)
        {
            introVideoPlayer.Stop();
            introVideoPlayer.gameObject.SetActive(false);
        }

        if (blurPanel != null)
        {
            var color = blurPanel.color;
            color.a = 0f;
            blurPanel.color = color;
            blurPanel.gameObject.SetActive(false);
        }
    }
}
