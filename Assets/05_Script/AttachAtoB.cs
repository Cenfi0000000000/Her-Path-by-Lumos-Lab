using UnityEngine;

public class AttacheAtoB : MonoBehaviour
{
    [Header("被附著的物件 (A)")]
    public Transform objectA;

    [Header("跟隨的目標 (B)")]
    public Transform objectB;

    [Header("位置是否跟隨")]
    public bool followPosition = true;

    [Header("是否跟隨 Rotation (使用詳盡軸控制)")]
    public bool enableAxisRotationFollow = true;

    [Header("A 的軸要不要跟隨 B 的軸")]
    public bool followAX = false;
    public bool followAY = false;
    public bool followAZ = false;

    [Header("A 軸對應 B 的哪個軸（0=X, 1=Y, 2=Z）")]
    public int mapAXtoB = 0; // A.X 跟 B.X
    public int mapAYtoB = 1; // A.Y 跟 B.Y
    public int mapAZtoB = 2; // A.Z 跟 B.Z

    [Header("A軸是否反轉 Rotation（乘上 -1）")]
    public bool invertAX = false;
    public bool invertAY = false;
    public bool invertAZ = false;

    private Vector3 initialEuler;

    void Start()
    {
        if (objectA != null)
            initialEuler = objectA.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        if (objectA == null || objectB == null)
            return;

        // ============================
        //        跟隨位置
        // ============================
        if (followPosition)
            objectA.position = objectB.position;

        // ============================
        //       跟隨 Rotation
        // ============================
        if (!enableAxisRotationFollow)
        {
            objectA.rotation = Quaternion.Euler(initialEuler);
            return;
        }

        Vector3 aEuler = initialEuler;
        Vector3 bEuler = objectB.rotation.eulerAngles;

        // ----------------------------
        // A.X 跟隨 + 可反轉
        // ----------------------------
        if (followAX)
        {
            float value = SelectAxis(bEuler, mapAXtoB);
            if (invertAX) value = -value;
            aEuler.x = value;
        }

        // ----------------------------
        // A.Y 跟隨 + 可反轉
        // ----------------------------
        if (followAY)
        {
            float value = SelectAxis(bEuler, mapAYtoB);
            if (invertAY) value = -value;
            aEuler.y = value;
        }

        // ----------------------------
        // A.Z 跟隨 + 可反轉
        // ----------------------------
        if (followAZ)
        {
            float value = SelectAxis(bEuler, mapAZtoB);
            if (invertAZ) value = -value;
            aEuler.z = value;
        }

        // 套用 rotation
        objectA.rotation = Quaternion.Euler(aEuler);
    }

    // mapAXtoB、mapAYtoB、mapAZtoB 的軸選擇工具
    private float SelectAxis(Vector3 axis, int index)
    {
        switch (index)
        {
            case 0: return axis.x;
            case 1: return axis.y;
            case 2: return axis.z;
        }
        return axis.x;
    }
}
