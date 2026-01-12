using UnityEngine;
using DG.Tweening;

public class DoorMover : MonoBehaviour
{
    [Header("物件 A / B")]
    public Transform objectA;
    public Transform objectB;

    [Header("平移量 (世界座標)")]
    public float moveA = 1f;
    public float moveB = 1f;

    [Header("動畫時間")]
    public float duration = 0.5f;

    // ⭐ 門的狀態：true = 已開門；false = 已關門
    private bool isOpen = true;

    /// <summary>
    /// 開門：A 平移 +moveA；B 平移 +moveB
    /// </summary>
    public void OpenDoor()
    {
        if (isOpen)
        {
            Debug.Log("[DoorMover] 無法開門：門已經是開的");
            return;
        }

        if (objectA != null)
            objectA.DOMove(objectA.position + new Vector3(moveA, 0, 0), duration);

        if (objectB != null)
            objectB.DOMove(objectB.position + new Vector3(moveB, 0, 0), duration);

        isOpen = true;
        Debug.Log("[DoorMover] 開門成功");
    }

    /// <summary>
    /// 關門：A 平移 -moveA；B 平移 -moveB
    /// </summary>
    public void CloseDoor()
    {
        if (!isOpen)
        {
            Debug.Log("[DoorMover] 無法關門：門已經是關的");
            return;
        }

        if (objectA != null)
            objectA.DOMove(objectA.position - new Vector3(moveA, 0, 0), duration);

        if (objectB != null)
            objectB.DOMove(objectB.position - new Vector3(moveB, 0, 0), duration);

        isOpen = false;
        Debug.Log("[DoorMover] 關門成功");
    }
}
