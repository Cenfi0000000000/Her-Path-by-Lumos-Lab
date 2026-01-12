using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIPanelController : MonoBehaviour
{
    [Header("目標 Canvas 或父物件")]
    public GameObject targetPanelRoot;

    private Graphic[] graphics;

    void Awake()
    {
        if (targetPanelRoot == null)
        {
            Debug.LogWarning("Target Panel Root 未指定，將使用自己物件");
            targetPanelRoot = this.gameObject;
        }

        // 抓取所有 Graphic 元件（Image, RawImage, Text, Button 也會有 Graphic）
        graphics = targetPanelRoot.GetComponentsInChildren<Graphic>(true);

        ClosePanel();
    }

    /// <summary>
    /// 打開整個 panel
    /// </summary>
    public void ShowPanel()
    {
        foreach (var g in graphics)
        {
            g.enabled = true;
        }

        // 如果有 CanvasGroup，讓互動也可用
        CanvasGroup cg = targetPanelRoot.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// 關閉整個 panel
    /// </summary>
    public void ClosePanel()
    {
        foreach (var g in graphics)
        {
            g.enabled = false;
        }

        CanvasGroup cg = targetPanelRoot.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}
