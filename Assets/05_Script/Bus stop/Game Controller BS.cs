using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class TimerEventController : MonoBehaviour
{
    [Header("UI 文字物件 (顯示倒數時間)")]
    public Text timerText;

    [Header("事件設定")]
    public UnityEvent onEventA;              // 事件A
    public UnityEvent onEventB;              // 事件B
    public UnityEvent onTenSecondsLeft;     // ⭐ 剩 5 秒時觸發
    public UnityEvent onTimerCompleted;      // ⭐ 計時完成後觸發

    private Coroutine timerRoutine = null;

    private float totalTime = 90f;          // 3分鐘
    private float interval = 10f;            // 起始：10秒
    private float minInterval = 6f;          // 最快：6秒
    private float intervalDecrease = 0.3f;   // 每次縮短量

    private bool isRunning = false;
    private bool hasTriggered = false;
    private bool hasTriggeredFiveSecondsEvent = false; // ⭐ 防止重複觸發

    /// <summary>
    /// 外部呼叫此方法觸發計時器（只會觸發一次，除非 Reset）
    /// </summary>
    public void _TriggerTimer()
    {
        if (hasTriggered)
        {
            Debug.Log("[TimerEventController] ⚠ 已經觸發過，請先 Reset");
            return;
        }

        Debug.Log("[TimerEventController] ▶ 外部觸發 TimerEventController");
        hasTriggered = true;
        StartTimer();
    }

    /// <summary>
    /// 重置計時器
    /// </summary>
    public void _ResetTimer()
    {
        Debug.Log("[TimerEventController] 🔄 Reset Timer");

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
        timerRoutine = null;

        isRunning = false;
        hasTriggered = false;
        hasTriggeredFiveSecondsEvent = false;

        totalTime = 90f;
        interval = 10f;
        UpdateTimerUI(totalTime);

        _TriggerTimer();
    }

    /// <summary>
    /// 啟動計時
    /// </summary>
    private void StartTimer()
    {
        if (isRunning)
            return;

        isRunning = true;
        totalTime = 90f;
        interval = 10f;
        hasTriggeredFiveSecondsEvent = false;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(TimerProcess());
    }

    /// <summary>
    /// 核心倒數流程
    /// </summary>
    private IEnumerator TimerProcess()
    {
        UpdateTimerUI(totalTime);

        float eventTimer = 0f;
        bool triggerA = true;

        while (totalTime > 0f)
        {
            float delta = Time.deltaTime;
            totalTime -= delta;
            eventTimer += delta;

            // ⭐ 剩 5 秒事件（只觸發一次）
            if (!hasTriggeredFiveSecondsEvent && totalTime <= 10f)
            {
                hasTriggeredFiveSecondsEvent = true;
                Debug.Log("[TimerEventController] ⚠ 剩下 10 秒");
                onTenSecondsLeft?.Invoke();
            }

            UpdateTimerUI(totalTime);

            if (eventTimer >= interval)
            {
                if (triggerA)
                {
                    onEventA?.Invoke();
                }
                else
                {
                    onEventB?.Invoke();

                    if (interval > minInterval)
                        interval -= intervalDecrease;
                }

                triggerA = !triggerA;
                eventTimer = 0f;
            }

            yield return null;
        }

        Debug.Log("[TimerEventController] 🟥 計時結束");
        isRunning = false;

        onTimerCompleted?.Invoke();
    }

    /// <summary>
    /// 更新 UI
    /// </summary>
    private void UpdateTimerUI(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(Mathf.Max(timeLeft, 0f) / 60f);
        int seconds = Mathf.FloorToInt(Mathf.Max(timeLeft, 0f) % 60f);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
