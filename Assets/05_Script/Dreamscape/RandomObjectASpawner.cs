using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class RandomObjectASpawner : MonoBehaviour
{
    [Header("生成設定")]
    public GameObject objectAPrefab;
    public int spawnCount = 5;
    public float fixedY = 1f;

    [Header("指定可觸發的角色物件（Figure）")]
    public GameObject targetFigure;

    [Header("計數設定")]
    public float targetValue = 5f;
    public Text counterText;
    public UnityEvent onTargetReached;

    private float currentCount = 0f;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Bounds cubeBounds;

    void Start()
    {
        if (objectAPrefab == null)
            Debug.LogError("[Spawner] ❌ 尚未指定 Object A Prefab");

        if (targetFigure == null)
            Debug.LogError("[Spawner] ❌ 尚未指定 targetFigure");

        Collider cubeCol = GetComponent<Collider>();
        if (cubeCol == null)
        {
            Debug.LogError("[Spawner] ❌ Spawner 本身需要 Collider");
            return;
        }

        cubeBounds = cubeCol.bounds;

        // Figure 必須有 Rigidbody（非 Kinematic）
        Rigidbody figRB = targetFigure.GetComponent<Rigidbody>();
        if (figRB == null)
        {
            figRB = targetFigure.AddComponent<Rigidbody>();
            figRB.isKinematic = false;
        }

        SpawnObjects();
        UpdateUI();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(cubeBounds.min.x, cubeBounds.max.x),
                fixedY,
                Random.Range(cubeBounds.min.z, cubeBounds.max.z)
            );

            GameObject obj = Instantiate(objectAPrefab, randomPos, Quaternion.identity);
            obj.name = $"ObjectA_{i}";
            spawnedObjects.Add(obj);

            // ===== Collider =====
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
                col = obj.AddComponent<BoxCollider>();

            col.isTrigger = false;

            // ===== Rigidbody =====
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = obj.AddComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;

            // ===== 碰撞回報腳本 =====
            ObjectACollisionReporter reporter = obj.AddComponent<ObjectACollisionReporter>();
            reporter.spawner = this;
        }

        Debug.Log($"[Spawner] ✅ 已生成 {spawnCount} 個 Object A（含 Collider + Rigidbody）");
    }

    // 🔔 由 Object A 主動回報
    public void OnObjectAHitFromChild(GameObject objectA)
    {
        if (!spawnedObjects.Contains(objectA)) return;

        currentCount += 1f;
        UpdateUI();

        spawnedObjects.Remove(objectA);
        Destroy(objectA);

        Debug.Log($"[Spawner] 📈 Count = {currentCount}");

        if (currentCount >= targetValue)
        {
            Debug.Log("[Spawner] 🎯 Target Reached！");
            onTargetReached?.Invoke();
        }
    }

    void UpdateUI()
    {
        if (counterText != null)
            counterText.text = currentCount.ToString("0");
    }
}
