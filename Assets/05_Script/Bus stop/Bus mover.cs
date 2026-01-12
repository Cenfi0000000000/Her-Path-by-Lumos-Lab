using UnityEngine;
using DG.Tweening;

public class BusMover : MonoBehaviour
{
    [Header("要移動的物件")]
    public Transform targetObject;

    [Header("移動設定")]
    public float moveDistance = 11f;   // 移動距離
    public float duration = 5f;        // 移動時間（秒）

    private bool hasMoved = false;      // 防止重複觸發

    /// <summary>
    /// Bus 進站：往 X 軸負向移動
    /// </summary>
    public void BusIn()
    {
        if (hasMoved)
        {
            Debug.Log("[BusMover] BusIn 已執行過，忽略重複呼叫");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogWarning("[BusMover] targetObject 未指定");
            return;
        }

        Vector3 targetPos = targetObject.position - new Vector3(moveDistance, 0f, 0f);

        targetObject.DOMove(targetPos, duration)
            .SetEase(Ease.Linear);

        hasMoved = true;
        Debug.Log("[BusMover] BusIn 執行：物件開始移動");
    }
}
