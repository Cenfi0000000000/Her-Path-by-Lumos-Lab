using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnableLightWithButtons : MonoBehaviour
{
    [Header("Lights 對應列表（Spot Light）")]
    public List<Light> lightList = new List<Light>(); // 必須有 3 個

    [Header("UI Button 對應列表")]
    public List<Button> buttonList = new List<Button>(); // 對應 PopOutButton1~3

    [Header("長按秒數")]
    public float requiredHoldSeconds = 2f;

    [Header("開燈後的強度值")]
    public float targetIntensity = 6f;

    [Header("三個 Light 全開後事件")]
    public UnityEvent onAllLightsOn;

    private HashSet<int> completedIndices = new HashSet<int>();
    private int holdIndex = -1;
    private float holdTimer = 0f;
    private bool isHolding = false;

    private void Start()
    {
        if (lightList.Count != 3 || buttonList.Count != 3)
        {
            Debug.LogWarning("[EnableLightWithButtons] ⚠ 請確保 LightList 與 ButtonList 各有 3 個元素");
        }

        // 初始化：所有燈光強度設為 0（視為關燈）
        foreach (var light in lightList)
        {
            if (light != null)
                light.intensity = 0f;
        }

        // 隱藏所有按鈕
        TurnOffButton();
    }

    private void Update()
    {
        if (isHolding && holdIndex >= 0)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldSeconds)
            {
                TriggerLight(holdIndex);
                StopHolding();
            }
        }
    }

    // ================= 公開 PopOutButton =================
    public void PopOutButton1() { ShowButton(0); }
    public void PopOutButton2() { ShowButton(1); }
    public void PopOutButton3() { ShowButton(2); }

    // ================= 新增：關閉所有顯示中的按鈕 =================
    public void TurnOffButton()
    {
        foreach (var btn in buttonList)
        {
            if (btn != null && btn.gameObject.activeSelf)
                btn.gameObject.SetActive(false);
        }

        Debug.Log("[EnableLightWithButtons] 🔕 所有顯示中的 Button 已關閉");
    }

    private void ShowButton(int index)
    {
        if (index < 0 || index >= buttonList.Count) return;

        buttonList[index].gameObject.SetActive(true);

        // 綁定長按事件
        buttonList[index].onClick.RemoveAllListeners();
        buttonList[index].onClick.AddListener(() => StartHolding(index));

        Debug.Log($"[EnableLightWithButtons] Button[{index}] 顯示");
    }

    private void StartHolding(int index)
    {
        if (completedIndices.Contains(index)) return;

        isHolding = true;
        holdIndex = index;
        holdTimer = 0f;

        Debug.Log($"[EnableLightWithButtons] ▶ 開始長按 Button[{index}]");
    }

    private void StopHolding()
    {
        isHolding = false;
        holdIndex = -1;
        holdTimer = 0f;
    }

    private void TriggerLight(int index)
    {
        if (completedIndices.Contains(index)) return;

        if (lightList.Count > index && lightList[index] != null)
        {
            // ⭐ 開燈邏輯：設定 Spot Light 強度
            lightList[index].intensity = targetIntensity;

            Debug.Log($"[EnableLightWithButtons] ⭐ Light[{index}] intensity 設為 {targetIntensity}");
        }

        completedIndices.Add(index);

        // 隱藏對應按鈕
        if (buttonList.Count > index && buttonList[index] != null)
            buttonList[index].gameObject.SetActive(false);

        // 檢查是否所有 Light 都已開啟
        if (completedIndices.Count == 3)
        {
            Debug.Log("[EnableLightWithButtons] 🎉 三個 Light 全開啟，觸發事件");
            onAllLightsOn?.Invoke();
        }
    }
}
