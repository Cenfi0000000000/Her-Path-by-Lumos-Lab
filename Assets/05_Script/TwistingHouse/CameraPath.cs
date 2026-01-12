using UnityEngine;

public class DualCameraFollower : MonoBehaviour
{
    [Header("Main Camera（XR Origin控制，用於取得座標與旋轉）")]
    public Transform mainCamera;

    [Header("Follower Camera（實際渲染畫面）")]
    public Transform followerCamera;

    private Quaternion initialRotation;

    void Start()
    {
        if (followerCamera == null)
            followerCamera = this.transform;

        // 記錄 Follower Camera 初始 rotation
        initialRotation = followerCamera.rotation;

        // 確保 Main Camera 不渲染畫面
        if (mainCamera != null)
        {
            Camera mainCamComp = mainCamera.GetComponent<Camera>();
            if (mainCamComp != null) mainCamComp.enabled = false;
        }

        // 確保 Follower Camera 可以渲染
        if (followerCamera.GetComponent<Camera>() == null)
            followerCamera.gameObject.AddComponent<Camera>();
    }

    void LateUpdate()
    {
        if (mainCamera == null || followerCamera == null) return;

        // 跟隨 Main Camera 的位置（世界座標）
        followerCamera.position = mainCamera.position;

        // Follower Camera 保持自身 rotation（可改成跟 Main Camera rotation）
        followerCamera.rotation = initialRotation;

        // 如果你想讓 Follower Camera rotation 跟 Main Camera 一樣
        // followerCamera.rotation = mainCamera.rotation;
    }
}
