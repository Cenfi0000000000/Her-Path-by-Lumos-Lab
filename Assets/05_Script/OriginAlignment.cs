using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

public class OriginAlignmentPolling : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform content;
    [SerializeField] private XROrigin xrOrigin;

    private bool originSet = false;

    void Update()
    {
        foreach (var trackedImage in trackedImageManager.trackables)
        {
            if (!originSet && trackedImage.trackingState == TrackingState.Tracking)
            {
                AlignContent(trackedImage);
                originSet = true;
                break; // 設定一次即可
            }
        }
    }

    private void AlignContent(ARTrackedImage trackedImage)
    {
        // 對齊 content 到圖片
        content.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);

        // 將 XR Origin 位置偏移到圖片
        Vector3 offset = trackedImage.transform.position - xrOrigin.Camera.transform.position;
        xrOrigin.transform.position += offset;

        Debug.Log("✅ 已將世界原點對齊到圖片位置");
    }
}
