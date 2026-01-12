using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StageEventTH : MonoBehaviour
{
    [Header("Path 物件（需要被隱藏／顯示的物件）")]
    public GameObject pathObject;

    [Header("畫面遮罩 (Canvas Image)")]
    public Image fadeMask; // 黑色 Image，alpha=0

    [Header("提示文字 (顯示：請回到初始點重新開始)")]
    public Text warningText;

    [Header("OutOfPath 漸暗需要的秒數")]
    public float fadeDuration = 1.0f;

    [Header("EndStage 事件")]
    public UnityEvent onEndStageEvent;

    private List<Renderer> pathRenderers = new List<Renderer>();
    private bool stageEnded = false;
    private bool stageCleared = false; // 🔹 新增：關卡完成標誌
    private bool outOfPathTriggered = false;


    private Coroutine fadeRoutine = null;

    private void Start()
    {
        if (pathObject != null)
            pathRenderers.AddRange(pathObject.GetComponentsInChildren<Renderer>());

        SetPathVisible(false);

        if (fadeMask != null)
            fadeMask.color = new Color(0, 0, 0, 0);

        if (warningText != null)
            warningText.enabled = false;
    }

    // ---------------------------------------------------------
    //  OutOfPath(): 畫面漸暗 + 顯示提示
    // ---------------------------------------------------------
    public void OutOfPath()
    {
        Debug.Log("[StageEventTH] OutOfPath() 被觸發 — 進入離開路徑狀態");

        outOfPathTriggered = true; // ⭐ 標記已觸發

        // 若已有協程在跑，先停止
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        fadeRoutine = StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float t = 0;

        warningText.enabled = true;
        warningText.text = "請回到初始點重新開始";

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0, 0.8f, t / fadeDuration);

            if (fadeMask != null)
                fadeMask.color = new Color(0, 0, 0, a);

            yield return null;
        }

        Debug.Log("[StageEventTH] 畫面已完全變暗");

        fadeRoutine = null;
    }

    // ---------------------------------------------------------
    // ResetStage(): 讓流程回到初始狀態
    // ---------------------------------------------------------
    public void ResetStage()
    {
        if (!outOfPathTriggered)
        {
            Debug.Log("[StageEventTH] ResetStage() 無效 — 尚未觸發 OutOfPath()");
            return;
        }

        Debug.Log("[StageEventTH] ResetStage() — 畫面與流程已重置");

        stageEnded = false;
        outOfPathTriggered = false; // ⭐ 重置 flag，允許下一輪 OutOfPath

        // 重置 CollisionTrigger
        var collisionTrigger = GetComponent<CollisionTrigger>();
        if (collisionTrigger != null)
            collisionTrigger.ResetTriggered();

        // 停止所有淡出流程
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        // 移除黑幕
        if (fadeMask != null)
            fadeMask.color = new Color(0, 0, 0, 0);

        // 隱藏警告文字
        if (warningText != null)
            warningText.enabled = false;

        // Path 隱藏
        SetPathVisible(false);
    }


    // ---------------------------------------------------------
    // EndStage(): Path 顯示 + 觸發事件
    // ---------------------------------------------------------
    public void EndStage()
    {
        if (stageEnded) return;

        stageEnded = true;

        Debug.Log("[StageEventTH] EndStage() — Path 顯示 + 事件觸發");

        SetPathVisible(true);
        onEndStageEvent?.Invoke();
    }



    // ---------------------------------------------------------
    private void SetPathVisible(bool visible)
    {
        foreach (var r in pathRenderers)
            if (r != null) r.enabled = visible;
    }
}
