using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChaserController : MonoBehaviour
{
    [Header("角色 A (Chaser) Prefab")]
    public GameObject chaserPrefab;

    [Header("座標 A Reference (通常是 XR Origin 的某個物件)")]
    public Transform targetA;

    [Header("事件：角色成功靠近 A")]
    public UnityEvent onGotChased;

    [Header("UI RawImage Prefab")]
    public RawImage rawImagePrefab;

    [Header("UI Canvas")]
    public RectTransform canvasRect;

    private GameObject chaserInstance;
    private GameObject previousChaser;
    private RawImage currentImage;

    private bool isMoving = false;
    private bool isSpotted = false;

    private float moveDuration = 5f;
    private float moveTimer = 0f;
    private Vector3 startPos;

    private float imageStartDistance = 400f;
    private float imageAngle = 0f;

    void Update()
    {
        if (chaserInstance == null || targetA == null)
            return;

        // 移動階段
        if (isMoving && !isSpotted)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);

            // 移動 Chaser
            chaserInstance.transform.position =
                Vector3.Lerp(startPos, targetA.position, t);

            float dist = Vector3.Distance(chaserInstance.transform.position, targetA.position);
            Debug.Log($"[ChaserController] 移動中，距離 A = {dist:F2}");

            if (dist < 0.5f)
            {
                Debug.Log("[ChaserController] 距離 < 0.5 → 觸發 _gotChased()");
                _gotChased();
            }

            // Chaser Z 軸朝向 targetA
            Vector3 lookDir = (targetA.position - chaserInstance.transform.position).normalized;
            if (lookDir != Vector3.zero)
                chaserInstance.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

            // 更新 RawImage
            if (currentImage != null)
            {
                // 計算 XY 平面方向
                Vector3 dir = (chaserInstance.transform.position - targetA.position).normalized;
                float distance = Mathf.Lerp(imageStartDistance, 0f, t);
                Vector2 imagePos = new Vector2(dir.x, dir.z) * distance;

                currentImage.rectTransform.anchoredPosition = imagePos;

                // 旋轉: 以 canvas 為圓心，旋轉 imageAngle
                Vector2 pivotToImage = imagePos;
                float rad = imageAngle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                Vector2 rotatedPos = new Vector2(
                    pivotToImage.x * cos - pivotToImage.y * sin,
                    pivotToImage.x * sin + pivotToImage.y * cos
                );
                currentImage.rectTransform.anchoredPosition = rotatedPos;

                // local Y 軸朝向 canvas
                Vector2 toCanvas = -rotatedPos.normalized;
                float localAngle = Mathf.Atan2(toCanvas.y, toCanvas.x) * Mathf.Rad2Deg - 90f;
                currentImage.rectTransform.localRotation = Quaternion.Euler(0, 0, localAngle);
            }
        }

        // 判斷 targetA 是否對準 chaserInstance
        if (!isSpotted && chaserInstance != null)
        {
            Vector3 toChaser = (chaserInstance.transform.position - targetA.position).normalized;
            Vector3 forward = targetA.forward;
            forward.y = 0;
            toChaser.y = 0;

            float angle = Vector3.Angle(forward, toChaser);
            if (angle < 10f)
            {
                Debug.Log($"[ChaserController] targetA 對準 chaserInstance，觸發 _spotChaser() (angle={angle:F1})");
                _spotChaser();
            }
            else
            {
                imageAngle = angle;
            }
        }

        // Spot 後持續 Look-at
        if (isSpotted)
        {
            Vector3 lookDir = (targetA.position - chaserInstance.transform.position).normalized;
            if (lookDir != Vector3.zero)
                chaserInstance.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
    }

    // -------------------------------------------------------------
    public void _spawnChaser()
    {
        Debug.Log("[ChaserController] _spawnChaser() 呼叫");

        if (chaserInstance != null)
            previousChaser = chaserInstance;

        Vector2 randomDir = Random.insideUnitCircle.normalized * 3f;
        Vector3 spawnPos = targetA.position + new Vector3(randomDir.x, 0, randomDir.y);

        chaserInstance = Instantiate(chaserPrefab, spawnPos, Quaternion.identity);
        startPos = spawnPos;

        SetRendererVisible(chaserInstance, false);

        Vector3 toTarget = (targetA.position - spawnPos).normalized;
        chaserInstance.transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);

        chaserInstance.transform.Rotate(0f, 0f, 180f, Space.Self);

        // RawImage
        if (rawImagePrefab != null && canvasRect != null)
        {
            currentImage = Instantiate(rawImagePrefab, canvasRect);
            currentImage.rectTransform.anchoredPosition = new Vector2(0, imageStartDistance);
            currentImage.rectTransform.rotation = Quaternion.identity;
        }

        moveTimer = 0f;
        isMoving = true;
        isSpotted = false;
    }

    // -------------------------------------------------------------
    public void _spotChaser()
    {
        Debug.Log("[ChaserController] _spotChaser() 呼叫");

        if (chaserInstance == null)
            return;

        if (previousChaser != null && previousChaser != chaserInstance)
            SetRendererVisible(previousChaser, false);

        SetRendererVisible(chaserInstance, true);

        isMoving = false;
        isSpotted = true;

        chaserInstance.transform.Rotate(0f, 0f, 180f, Space.Self);

        // 刪除 RawImage
        if (currentImage != null)
        {
            Destroy(currentImage.gameObject);
            currentImage = null;
        }
    }

    // -------------------------------------------------------------
    public void _gotChased()
    {
        isMoving = false;
        isSpotted = false;
        onGotChased?.Invoke();
    }

    // -------------------------------------------------------------
    private void SetRendererVisible(GameObject obj, bool visible)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }
}
