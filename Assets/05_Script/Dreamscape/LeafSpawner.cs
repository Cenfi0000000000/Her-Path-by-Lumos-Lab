using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LeafSpawnerWithCollision : MonoBehaviour
{
    [Header("生成設定")]
    public List<GameObject> leafPrefabs;
    public int spawnCount = 10;
    public float spawnHeight = 5f;
    public float fallDuration = 3f;

    [Header("物件 A（被碰撞者）")]
    public GameObject objectA;

    [Header("計數與事件")]
    public int targetValue = 10;
    public Text counterText;
    public UnityEvent onTargetReached;

    [Header("生成節奏")]
    public float minSpawnDelay = 0.3f;
    public float maxSpawnDelay = 1.2f;

    [Header("單次下落數量（一波）")]
    public int minBatchCount = 5;
    public int maxBatchCount = 6;


    private int currentCount = 0;
    private List<GameObject> spawnedLeaves = new List<GameObject>();
    private Bounds spawnBounds;

    void Start()
    {
        // Spawner 本身一定要有 Collider（用來定義範圍）
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("[Spawner] ❌ Spawner 本身需要 Collider");
            return;
        }
        spawnBounds = col.bounds;

        // ObjectA 必須有 Rigidbody（非 Kinematic）
        Rigidbody rb = objectA.GetComponent<Rigidbody>();
        if (rb == null)
            rb = objectA.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;

        // ObjectA 一定要有 Collider（非 Trigger）
        Collider aCol = objectA.GetComponent<Collider>();
        if (aCol == null)
            aCol = objectA.AddComponent<BoxCollider>();

        aCol.isTrigger = false;

        StartCoroutine(SpawnLeavesSequentially());
        UpdateUI();
    }

    IEnumerator SpawnLeavesSequentially()
    {
        while (true) // ⭐ 無限生成波次
        {
            int batchCount = Random.Range(minBatchCount, maxBatchCount + 1);

            for (int i = 0; i < batchCount; i++)
            {
                if (leafPrefabs.Count == 0)
                    yield break;

                GameObject prefab =
                    leafPrefabs[Random.Range(0, leafPrefabs.Count)];

                Vector3 spawnPos = new Vector3(
                    Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                    spawnHeight,
                    Random.Range(spawnBounds.min.z, spawnBounds.max.z)
                );

                GameObject leaf =
                    Instantiate(prefab, spawnPos, Random.rotation);

                spawnedLeaves.Add(leaf);

                // Collider（Trigger）
                Collider leafCol = leaf.GetComponent<Collider>();
                if (leafCol == null)
                    leafCol = leaf.AddComponent<BoxCollider>();
                leafCol.isTrigger = true;

                // Rigidbody
                Rigidbody rb = leaf.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = leaf.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;

                // Trigger Reporter
                LeafTriggerReporter reporter =
                    leaf.AddComponent<LeafTriggerReporter>();
                reporter.spawner = this;

                FallLeaf(leaf);
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }



    void FallLeaf(GameObject leaf)
    {
        Vector3 targetPos = new Vector3(
            leaf.transform.position.x,
            spawnBounds.min.y + 0.1f,
            leaf.transform.position.z
        );

        leaf.transform
            .DOMove(targetPos, fallDuration)
            .SetEase(Ease.InQuad);

        leaf.transform
            .DORotate(Random.insideUnitSphere * 360f, fallDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InQuad);
    }

    // 🔔 由 Trigger 回報
    public void OnLeafHit(GameObject leaf)
    {
        if (!spawnedLeaves.Contains(leaf))
            return;

        currentCount++;
        UpdateUI();

        spawnedLeaves.Remove(leaf);
        Destroy(leaf);

        if (currentCount >= targetValue)
        {
            Debug.Log("[Spawner] 🎯 Target Reached!");
            onTargetReached?.Invoke();
        }
    }

    void UpdateUI()
    {
        if (counterText != null)
            counterText.text = currentCount.ToString();
    }
}
