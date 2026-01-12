using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CollisionTrigger : MonoBehaviour
{
    [Header("Trigger 名稱（Inspector 用）")]
    public string triggerName = "CollisionTrigger";

    [Header("Mesh A（帶有 Collider 的目標）")]
    public Transform meshATransform;

    [Header("可觸發的目標物件")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("需停留秒數")]
    public float requiredSeconds = 2f;

    [Header("觸發事件")]
    public UnityEvent OnHoldEvent;

    private bool triggered = false;
    private bool resetUsed = false;
    private Dictionary<GameObject, float> stayTimers = new Dictionary<GameObject, float>();

    private void Awake()
    {
        SetupMeshColliders();
    }

    /// <summary>
    /// 設定 Mesh A 及其子物件的 Collider 為 Trigger，並添加事件轉發
    /// </summary>
    private void SetupMeshColliders()
    {
        if (meshATransform == null)
        {
            Debug.LogError($"[{triggerName}] ❌ Mesh A Transform 未指定");
            return;
        }

        Collider[] colliders = meshATransform.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogError($"[{triggerName}] ❌ Mesh A 或其子物件沒有 Collider");
            return;
        }

        foreach (var col in colliders)
        {
            col.isTrigger = true;

            // 為每個 Collider 添加輔助觸發腳本，將事件傳回父物件
            var helper = col.gameObject.AddComponent<ChildColliderTrigger>();
            helper.parentTrigger = this;
        }

        Debug.Log($"[{triggerName}] ✅ 設定完成，總共 {colliders.Length} 個 Collider 被設為 Trigger");
    }

    // 事件處理
    public void HandleTriggerEnter(Collider other)
    {
        Debug.Log($"[{triggerName}] 🔹 OnTriggerEnter: {other.name}");

        if (triggered) return;
        if (targetObjects.Contains(other.gameObject) && !stayTimers.ContainsKey(other.gameObject))
        {
            stayTimers[other.gameObject] = 0f;
            Debug.Log($"[{triggerName}] 🔸 目標物件 {other.name} 進入 Trigger，開始計時");
        }
    }

    public void HandleTriggerStay(Collider other)
    {
        if (triggered) return;
        if (!targetObjects.Contains(other.gameObject)) return;

        if (!stayTimers.ContainsKey(other.gameObject))
            stayTimers[other.gameObject] = 0f;

        stayTimers[other.gameObject] += Time.deltaTime;

        Debug.Log($"[{triggerName}] ⏱ OnTriggerStay: {other.name}, 停留時間 {stayTimers[other.gameObject]:F2}s");

        if (stayTimers[other.gameObject] >= requiredSeconds)
        {
            triggered = true;
            Debug.Log($"[{triggerName}] ⭐ Hold 達成，觸發事件: {other.name}");
            OnHoldEvent?.Invoke();
        }
    }

    public void HandleTriggerExit(Collider other)
    {
        Debug.Log($"[{triggerName}] ❌ OnTriggerExit: {other.name}");

        if (targetObjects.Contains(other.gameObject))
        {
            stayTimers.Remove(other.gameObject);
            Debug.Log($"[{triggerName}] 🔹 目標物件 {other.name} 離開 Trigger，計時清除");
        }
    }

    /// <summary>
    /// Reset（只能使用一次）
    /// </summary>
    public void ResetTriggered()
    {
        if (resetUsed)
        {
            triggered = false;
            stayTimers.Clear();
            Debug.Log($"[{triggerName}] 🔄 ResetTriggered() 已用過，僅重置狀態");
            return;
        }

        triggered = false;
        stayTimers.Clear();
        resetUsed = true;

        Debug.Log($"[{triggerName}] 🔄 ResetTriggered() 第一次使用，狀態重置完成");
    }

    /// <summary>
    /// 強制 Reset（不限次數）
    /// </summary>
    public void ForcedResetTriggered()
    {
        triggered = false;
        stayTimers.Clear();
        resetUsed = false;

        Debug.Log($"[{triggerName}] 🔁 ForcedResetTriggered() 強制重置完成");
    }

    // 子物件 Collider 的輔助類別
    private class ChildColliderTrigger : MonoBehaviour
    {
        public CollisionTrigger parentTrigger;

        private void OnTriggerEnter(Collider other)
        {
            parentTrigger.HandleTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            parentTrigger.HandleTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            parentTrigger.HandleTriggerExit(other);
        }
    }
}
