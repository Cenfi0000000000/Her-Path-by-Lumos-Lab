using UnityEngine;

public class MonoMirror : MonoBehaviour
{
    public Transform arCamera;        // AR Camera
    public Camera mirrorCamera;       // 專屬反射 Camera
    public Transform mirrorPlane;     // 鏡子（法線 = mirrorPlane.up）

    void LateUpdate()
    {
        if (!arCamera || !mirrorCamera || !mirrorPlane) return;

        UpdateReflection();
    }

    void UpdateReflection()
    {
        Vector3 normal = mirrorPlane.up;          // ★ 以 +Y 為鏡面法線
        Vector3 planePoint = mirrorPlane.position;

        // ========= 位置反射 =========
        Vector3 camToPlane = arCamera.position - planePoint;
        Vector3 reflectedPos =
            arCamera.position - 2f * Vector3.Dot(camToPlane, normal) * normal;

        mirrorCamera.transform.position = reflectedPos;

        // ========= rotation 反射 =========
        Vector3 forward =
            arCamera.forward - 2f * Vector3.Dot(arCamera.forward, normal) * normal;

        Vector3 up =
            arCamera.up - 2f * Vector3.Dot(arCamera.up, normal) * normal;

        mirrorCamera.transform.rotation = Quaternion.LookRotation(forward, up);

        // ========= 讓反射相機對著鏡子 =========
        mirrorCamera.transform.LookAt(
            mirrorPlane.position - normal,
            mirrorCamera.transform.up
        );
    }
}
