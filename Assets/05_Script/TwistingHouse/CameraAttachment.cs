using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class MirrorCameraManager_Simple : MonoBehaviour
{
    [Header("鏡子鏡頭列表")]
    public List<Camera> mirrorCameras;

    [Header("附著平面")]
    public Transform planeA;

    [Header("鏡頭相對平面的偏移")]
    public Vector3 localOffset = Vector3.zero;

    void LateUpdate()
    {
        if (planeA == null) return;

        foreach (var cam in mirrorCameras)
        {
            if (cam == null) continue;

            // 脫離 XR Origin（只執行一次即可，也可放 Awake）
            cam.transform.SetParent(null, true);

            // 設定位置
            cam.transform.position = planeA.TransformPoint(localOffset);

            // 設定旋轉：鏡頭 +Z 朝向平面 +Y，Y 軸朝上
            cam.transform.rotation = Quaternion.LookRotation(planeA.up, Vector3.up);
        }
    }
}
