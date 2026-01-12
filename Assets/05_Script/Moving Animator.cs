using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class DOTweenObjectMover : MonoBehaviour
{
    public enum MoveAxis
    {
        X,
        Y,
        Z
    }

    public enum MoveDirection
    {
        Positive,
        Negative
    }

    [Header("要移動的物件清單")]
    public List<Transform> targetObjects = new List<Transform>();

    [Header("移動設定")]
    public MoveAxis moveAxis = MoveAxis.X;
    public MoveDirection moveDirection = MoveDirection.Positive;
    public float moveDistance = 1f;
    public float moveDuration = 1f;

    [Header("Tween 設定")]
    public Ease easeType = Ease.OutCubic;
    public bool useLocalPosition = true;

    public void MoveObject()
    {
        Vector3 moveVector = Vector3.zero;
        float dir = (moveDirection == MoveDirection.Positive) ? 1f : -1f;

        switch (moveAxis)
        {
            case MoveAxis.X:
                moveVector = Vector3.right * moveDistance * dir;
                break;
            case MoveAxis.Y:
                moveVector = Vector3.up * moveDistance * dir;
                break;
            case MoveAxis.Z:
                moveVector = Vector3.forward * moveDistance * dir;
                break;
        }

        foreach (Transform obj in targetObjects)
        {
            if (obj == null) continue;

            Vector3 targetPos = useLocalPosition
                ? obj.localPosition + moveVector
                : obj.position + moveVector;

            if (useLocalPosition)
            {
                obj.DOLocalMove(targetPos, moveDuration).SetEase(easeType);
            }
            else
            {
                obj.DOMove(targetPos, moveDuration).SetEase(easeType);
            }
        }
    }
}
