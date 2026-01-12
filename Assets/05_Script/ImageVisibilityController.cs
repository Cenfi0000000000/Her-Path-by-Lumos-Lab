using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageVisibilityController : MonoBehaviour
{
    [Header("Target RawImages")]
    public List<RawImage> targetRawImages = new List<RawImage>();

    [Header("Target Buttons")]
    public List<Button> targetButtons = new List<Button>();

    void Awake()
    {
        HideAll();
    }

    /// <summary>
    /// 顯示所有 RawImage 與 Button
    /// </summary>
    public void ShowAll()
    {
        foreach (var rawImage in targetRawImages)
        {
            if (rawImage != null)
                rawImage.gameObject.SetActive(true);
        }

        foreach (var button in targetButtons)
        {
            if (button != null)
                button.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隱藏所有 RawImage 與 Button
    /// </summary>
    public void HideAll()
    {
        foreach (var rawImage in targetRawImages)
        {
            if (rawImage != null)
                rawImage.gameObject.SetActive(false);
        }

        foreach (var button in targetButtons)
        {
            if (button != null)
                button.gameObject.SetActive(false);
        }
    }
}
