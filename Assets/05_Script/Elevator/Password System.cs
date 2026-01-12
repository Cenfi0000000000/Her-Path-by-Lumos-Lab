using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class UIButtonManager : MonoBehaviour
{
    [Header("按鈕列表")]
    public List<Button> buttons = new List<Button>();

    [Header("按鈕對應的圖片 (1對1)")]
    public List<Sprite> pressedSprites = new List<Sprite>();

    [Header("成功達成時的事件")]
    public List<UnityEvent> onSuccessEvents = new List<UnityEvent>();

    private List<Sprite> originalSprites = new List<Sprite>();
    private List<Color> originalColors = new List<Color>(); // 記錄原本顏色
    private List<bool> isPressedList = new List<bool>();

    private int correctCount = 0;

    void Awake()
    {
        // 初始化按鈕、狀態、原始圖片與顏色
        for (int i = 0; i < buttons.Count; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            originalSprites.Add(img.sprite);
            originalColors.Add(img.color); // 儲存原顏色
            isPressedList.Add(false);

            int index = i; // 避免閉包問題
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));

            // 初始透明度為 0
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }

    void OnButtonPressed(int index)
    {
        if (isPressedList[index])
            return; // 已按下就不重複

        isPressedList[index] = true;

        Image img = buttons[index].GetComponent<Image>();

        // 換成對應圖片
        if (pressedSprites != null && index < pressedSprites.Count && pressedSprites[index] != null)
            img.sprite = pressedSprites[index];

        // 將透明度調到 100%
        Color c = img.color;
        c.a = 1f;
        img.color = c;
    }

    /// <summary>
    /// 延遲 reset，1 秒後恢復所有按鈕狀態
    /// </summary>
    public void ResetButton()
    {
        correctCount = 0; // 計數歸零
        StartCoroutine(DelayedReset(1f));
    }

    private IEnumerator DelayedReset(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < buttons.Count; i++)
        {
            isPressedList[i] = false;
            buttons[i].GetComponent<Image>().sprite = originalSprites[i];

            // 還原原本顏色 (包含透明度)
            buttons[i].GetComponent<Image>().color = originalColors[i];
        }
    }

    /// <summary>
    /// 當按鈕被確認正確時呼叫
    /// </summary>
    public void CorrectButton()
    {
        correctCount++;

        if (correctCount >= 4)
        {
            // 成功，觸發事件
            foreach (var evt in onSuccessEvents)
            {
                evt?.Invoke();
            }

            correctCount = 0; // 可選：重置計數
        }
    }
}
