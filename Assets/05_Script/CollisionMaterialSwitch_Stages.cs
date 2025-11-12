using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ARShowOnCollision : MonoBehaviour
{
    [Header("目標物件")]
    public List<GameObject> Stages;  // 物件 B 多個

    [Header("觸發物件")]
    public GameObject Zone; // 物件 C

    void Awake()
    {
        // 一開始隱藏 Stages
        if (Stages != null)
        {
            SetRenderersEnabled(Stages, false);
        }

        // 配置鏡頭 Collider
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (col is SphereCollider sc)
        {
            sc.radius = 0.05f; // 可根據需求調整
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Zone)
        {
            // 碰撞到 C，顯示 Stages
            if (Stages != null)
            {
                SetRenderersEnabled(Stages, true);

                // 輸出所有 Stage 名稱
                string stageNames = string.Join(", ", Stages.ConvertAll(s => s.name));
                Debug.Log("Trigger Enter: " + other.gameObject.name + " -> Show " + stageNames);
            }
        }
    }

    // 幫助函數：設定多個物件及其子物件的 Renderer
    private void SetRenderersEnabled(List<GameObject> objects, bool enabled)
    {
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.enabled = enabled;
            }
        }
    }
}
