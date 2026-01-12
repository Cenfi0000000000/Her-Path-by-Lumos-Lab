using UnityEngine;

public class RotationReader : MonoBehaviour
{
    [Header("外部參考")]
    public Transform target;   // 外部 Transform

    [Header("輸出旋轉角度 (0~360)")]
    public float rotX;
    public float rotY;
    public float rotZ;

    public float rotXPlusY;
    public float rotYPlusZ;
    public float rotXPlusZ;
    public float rotXPlusYPlusZ;

    [Header("七個外部物件 (一對一對應)")]
    public Transform[] targetObjects = new Transform[7];

    private float[] rotationValues = new float[7];
    private float[] initialZOffset = new float[7]; // 每個物件的起始 Z offset
    private bool initialized = false;

    void Update()
    {
        if (target == null) return;

        // 轉成 Euler 角度 (Unity 自己會 clamp 0~360)
        Vector3 euler = target.rotation.eulerAngles;

        rotX = Normalize360(euler.x);
        rotY = Normalize360(euler.y);
        rotZ = Normalize360(euler.z);

        rotXPlusY = Normalize360(rotX + rotY);
        rotYPlusZ = Normalize360(rotY + rotZ);
        rotXPlusZ = Normalize360(rotX + rotZ);
        rotXPlusYPlusZ = Normalize360(rotX + rotY + rotZ);

        // 填入七個值（順序固定）
        rotationValues[0] = rotX;
        rotationValues[1] = rotY;
        rotationValues[2] = rotZ;
        rotationValues[3] = rotXPlusY;
        rotationValues[4] = rotYPlusZ;
        rotationValues[5] = rotXPlusZ;
        rotationValues[6] = rotXPlusYPlusZ;

        // 只在第一次時記錄初始 Z offset
        if (!initialized)
        {
            CacheInitialZOffsets();
            initialized = true;
        }

        // 套用 rotation 到 7 個物件
        ApplyRotations();
    }

    /// <summary>
    /// 只記錄一次每個物件的初始 Z 值
    /// </summary>
    void CacheInitialZOffsets()
    {
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] == null) continue;
            initialZOffset[i] = targetObjects[i].localEulerAngles.z;
        }
    }

    /// <summary>
    /// 將 rotation + 初始 Z offset 套用到 7 個物件
    /// </summary>
    void ApplyRotations()
    {
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] == null) continue;

            float value = rotationValues[i];      // 指定對應值
            float finalZ = Normalize360(value + initialZOffset[i]);  // 加上初始 Z 偏移

            Vector3 newRot = targetObjects[i].localEulerAngles;
            newRot.z = finalZ;
            targetObjects[i].localEulerAngles = newRot;
        }
    }

    float Normalize360(float angle)
    {
        angle %= 360f;
        return angle < 0 ? angle + 360f : angle;
    }
}
