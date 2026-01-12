using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ARScreenRaycaster : MonoBehaviour
{
    [Header("AR Camera")]
    public Camera arCamera;

    [Header("互動物件（有 Collider，順序很重要）")]
    public List<GameObject> targets = new List<GameObject>();

    [Header("對應事件（index 對 index）")]
    public List<UnityEvent> events = new List<UnityEvent>();

    void Update()
    {
        Vector2? screenPos = null;

#if UNITY_EDITOR
        // Editor 或 PC 滑鼠
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPos = Mouse.current.position.ReadValue();
        }
#else
        // 手機觸控
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
#endif

        if (screenPos == null)
            return;

        Ray ray = arCamera.ScreenPointToRay(screenPos.Value);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;

        // 遞迴往父層找，避免 collider 在子物件
        Transform t = hit.collider.transform;
        while (t != null)
        {
            int index = targets.IndexOf(t.gameObject);
            if (index != -1)
            {
                if (index < events.Count)
                    events[index]?.Invoke();
                return;
            }
            t = t.parent;
        }
    }
}
