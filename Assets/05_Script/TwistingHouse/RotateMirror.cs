using UnityEngine;

public class FollowZRotationLocalAxisFixed : MonoBehaviour
{
    [Header("來源物件")]
    public Transform sourceObject;

    [Header("目標物件")]
    public Transform targetObject;

    [Header("操作因子")]
    public float offset = 0f;       // 加數值
    public float multiplier = 1f;   // 乘數值
    public bool invert = false;     // 是否反向

    [Header("旋轉中心偏移（單軸可調整）")]
    public float offsetX = 0f;      // local X 偏移
    public float offsetY = -3f;     // local Y 偏移
    public float offsetZ = 0f;      // local Z 偏移

    private float lastSourceZ;

    void Start()
    {
        if (sourceObject != null)
            lastSourceZ = sourceObject.localEulerAngles.z;
    }

    void Update()
    {
        if (sourceObject == null || targetObject == null) return;

        // 取得 source Z 軸旋轉 (local)
        float currentSourceZ = sourceObject.localEulerAngles.z;

        // 計算增量 (考慮 0-360 循環)
        float deltaZ = Mathf.DeltaAngle(lastSourceZ, currentSourceZ);

        // 應用操作因子
        deltaZ = deltaZ * multiplier + offset;
        if (invert) deltaZ *= -1f;

        // 組合旋轉中心偏移
        Vector3 localOffset = new Vector3(offsetX, offsetY, offsetZ);

        // 計算旋轉中心在世界座標
        Vector3 rotationCenter = targetObject.position + targetObject.TransformVector(localOffset);

        // 沿 target 自身 Z 軸旋轉
        targetObject.RotateAround(rotationCenter, targetObject.forward, deltaZ);

        // 更新 lastSourceZ
        lastSourceZ = currentSourceZ;
    }
}
