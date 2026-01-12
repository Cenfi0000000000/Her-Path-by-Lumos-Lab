using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ARCameraFreezeZoneController : MonoBehaviour
{
    [Header("AR Camera")]
    public Transform arCamera;

    [Header("觸發物件 Root（自動抓子 Collider）")]
    public Transform triggerRoot;

    [Header("是否啟用凍結系統")]
    [SerializeField] private bool freezerEnabled = true;

    [Header("Freeze / Unfreeze Events")]
    public UnityEvent onCameraFreezed;     // 當凍結成功時呼叫
    public UnityEvent onCameraUnfreezed;   // 當解除凍結時呼叫

    private Transform originalCameraParent;
    private Transform freezeAnchor;

    private bool isCameraFrozen = false;

    private HashSet<Collider> triggerColliders = new HashSet<Collider>();

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (arCamera != null)
            originalCameraParent = arCamera.parent;

        CacheTriggerColliders();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!freezerEnabled) return;
        if (!IsTriggerObject(other)) return;

        Debug.Log("ENTER Zone → Unfreeze");
        UnfreezeCamera();
    }

    void OnTriggerExit(Collider other)
    {
        if (!freezerEnabled) return;
        if (!IsTriggerObject(other)) return;

        Debug.Log("EXIT Zone → Freeze");
        TryFreezeCamera();
    }

    // ------------------------
    // Freeze Logic
    // ------------------------

    void TryFreezeCamera()
    {
        if (isCameraFrozen) return;

        freezeAnchor = new GameObject("CameraFreezeAnchor").transform;
        freezeAnchor.position = arCamera.position;
        freezeAnchor.rotation = arCamera.rotation;

        arCamera.SetParent(null, true);
        arCamera.SetParent(freezeAnchor, true);

        isCameraFrozen = true;

        Debug.Log("Camera FREEZED");

        // 🔔 Event Callback
        onCameraFreezed?.Invoke();
    }

    void UnfreezeCamera()
    {
        if (!isCameraFrozen) return;

        arCamera.SetParent(originalCameraParent, true);
        isCameraFrozen = false;

        if (freezeAnchor != null)
            Destroy(freezeAnchor.gameObject);

        Debug.Log("Camera UNFREEZED");

        // 🔔 Event Callback
        onCameraUnfreezed?.Invoke();
    }

    // ------------------------
    // External Control
    // ------------------------

    public void FreezerSwitch(bool enable)
    {
        freezerEnabled = enable;

        if (!enable)
            UnfreezeCamera();
    }

    // ------------------------
    // Utilities
    // ------------------------

    void CacheTriggerColliders()
    {
        triggerColliders.Clear();

        if (triggerRoot == null) return;

        foreach (Collider col in triggerRoot.GetComponentsInChildren<Collider>())
            triggerColliders.Add(col);
    }

    bool IsTriggerObject(Collider col)
    {
        return triggerColliders.Contains(col);
    }
}
