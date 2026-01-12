using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class _ARMultiCollisionHideRenderer : MonoBehaviour
{
    [Header("目標 Stage")]
    public GameObject targetStage; // 要隱藏/顯示的物件

    [Header("觸發 Masks")]
    public List<GameObject> Masks; // 多個 Mask

    [Header("觸發 Zones (改為多個 Zone)")]
    public List<GameObject> Zones; // 多個 Zone

    private bool isInsideZone = false;   // 是否在任一 Zone 內
    private bool hasTouchedMask = false; // 是否碰過任意 Mask

    void Awake()
    {
        // 配置 Camera Collider
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (col is SphereCollider sc)
        {
            sc.radius = 0.05f; // 可自行調整
        }

        // 初始隱藏 targetStage
        UpdateVisibility();
    }

    void OnTriggerEnter(Collider other)
    {
        // ➤ Mask 邏輯不變
        if (Masks.Contains(other.gameObject))
        {
            if (!hasTouchedMask)
            {
                hasTouchedMask = true;
                UpdateVisibility();
            }

            Debug.Log("Trigger Enter: Mask -> " + other.gameObject.name
                + " | hasTouchedMask: " + hasTouchedMask);
        }

        // ➤ Zones（改為 List）
        else if (Zones.Contains(other.gameObject))
        {
            isInsideZone = true;
            UpdateVisibility();
            Debug.Log("Trigger Enter: Zone -> " + other.gameObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // ➤ Mask 離開不清除 hasTouchedMask
        if (Masks.Contains(other.gameObject))
        {
            Debug.Log("Trigger Exit: Mask -> " + other.gameObject.name
                + " | hasTouchedMask: " + hasTouchedMask);
        }

        // ➤ 離開 Zone（改為 List）
        else if (Zones.Contains(other.gameObject))
        {
            // ⚠️ 當離開某 Zone 時，需要確認是否仍在其他 Zone 內
            isInsideZone = CheckStillInsideAnyZone();

            // 若完全不在任何 Zone 內 → 重置
            if (!isInsideZone)
            {
                hasTouchedMask = false;
                UpdateVisibility();
                Debug.Log("Trigger Exit: Zone -> " + other.gameObject.name + " | Reset hasTouchedMask");
            }
        }
    }

    /// <summary>
    /// 檢查是否仍在任何一個 Zone 的 Collider 內
    /// </summary>
    private bool CheckStillInsideAnyZone()
    {
        Collider selfCollider = GetComponent<Collider>();

        foreach (GameObject zone in Zones)
        {
            if (zone == null) continue;

            Collider zoneCol = zone.GetComponent<Collider>();
            if (zoneCol == null) continue;

            // 粗略檢查，若兩個 Collider 還在重疊中則視為仍在 Zone
            if (selfCollider.bounds.Intersects(zoneCol.bounds))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 控制 Stage Renderer 顯示/隱藏
    /// </summary>
    private void UpdateVisibility()
    {
        if (targetStage == null) return;

        Renderer[] renderers = targetStage.GetComponentsInChildren<Renderer>();
        bool shouldHide = hasTouchedMask && isInsideZone;

        foreach (Renderer rend in renderers)
        {
            rend.enabled = !shouldHide;
        }
    }
}
