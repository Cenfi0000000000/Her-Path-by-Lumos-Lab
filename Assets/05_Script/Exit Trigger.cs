using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CollisionExitTrigger : MonoBehaviour
{
    [Header("Trigger 名稱（Inspector 用）")]
    public string triggerName = "CollisionExitTrigger";

    [Header("Mesh A（帶有 Collider 的目標）")]
    public Transform meshATransform;

    [Header("可觸發的目標物件")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("離開後觸發事件")]
    public UnityEvent OnExitEvent;

    private bool triggered = false;
    private bool resetUsed = false;

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

            var helper = col.gameObject.AddComponent<ChildColliderTrigger>();
            helper.parentTrigger = this;
        }

        Debug.Log($"[{triggerName}] ✅ 設定完成，總共 {colliders.Length} 個 Collider 被設為 Trigger");
    }

    // ================= 事件處理 =================

    public void HandleTriggerExit(Collider other)
    {
        Debug.Log($"[{triggerName}] ❌ OnTriggerExit: {other.name}");

        if (triggered) return;
        if (!targetObjects.Contains(other.gameObject)) return;

        triggered = true;

        Debug.Log($"[{triggerName}] ⭐ 目標物件 {other.name} 離開 Trigger，觸發事件");
        OnExitEvent?.Invoke();
    }

    /// <summary>
    /// Reset（只能使用一次）
    /// </summary>
    public void ResetTriggered()
    {
        if (resetUsed)
        {
            triggered = false;
            Debug.Log($"[{triggerName}] 🔄 ResetTriggered() 已用過，僅重置狀態");
            return;
        }

        triggered = false;
        resetUsed = true;

        Debug.Log($"[{triggerName}] 🔄 ResetTriggered() 第一次使用，狀態重置完成");
    }

    /// <summary>
    /// 強制 Reset（不限次數）
    /// </summary>
    public void ForcedResetTriggered()
    {
        triggered = false;
        resetUsed = false;

        Debug.Log($"[{triggerName}] 🔁 ForcedResetTriggered() 強制重置完成");
    }

    // ================= 子物件 Collider 輔助類別 =================

    private class ChildColliderTrigger : MonoBehaviour
    {
        public CollisionExitTrigger parentTrigger;

        private void OnTriggerExit(Collider other)
        {
            parentTrigger.HandleTriggerExit(other);
        }
    }
}
