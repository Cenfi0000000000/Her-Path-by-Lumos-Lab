using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ARMultiCollisionHideRenderer : MonoBehaviour
{
    [Header("目標 Stage")]
    public GameObject targetStage; // 要隱藏/顯示的物件 D

    [Header("觸發 Zones")]
    public List<GameObject> Masks; // 多個 Mask
    public GameObject Zone;         // Camera 不再碰到 Zone → 顯示

    private bool isInsideZone = false;   // 是否在 Zone 內
    private bool hasTouchedMask = false; // 是否碰過任意 Mask

    void Awake()
    {
        // 自動配置 Camera Collider
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (col is SphereCollider sc)
        {
            sc.radius = 0.05f; // 可依需求調整
        }

        // 一開始隱藏 targetStage
        UpdateVisibility();
    }

    void OnTriggerEnter(Collider other)
    {
        if (Masks.Contains(other.gameObject))
        {
            if (!hasTouchedMask)
            {
                hasTouchedMask = true; // 碰過任意 Mask 就設為 true
                UpdateVisibility();
            }
            Debug.Log("Trigger Enter: Mask -> " + other.gameObject.name + " | hasTouchedMask: " + hasTouchedMask);
        }
        else if (other.gameObject == Zone)
        {
            isInsideZone = true;
            UpdateVisibility();
            Debug.Log("Trigger Enter: Zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Masks.Contains(other.gameObject))
        {
            // 離開 Mask 不影響 hasTouchedMask
            Debug.Log("Trigger Exit: Mask -> " + other.gameObject.name + " | hasTouchedMask: " + hasTouchedMask);
        }
        else if (other.gameObject == Zone)
        {
            isInsideZone = false;
            hasTouchedMask = false; // 離開 Zone 後才重置
            UpdateVisibility();
            Debug.Log("Trigger Exit: Zone | Reset hasTouchedMask");
        }
    }

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
