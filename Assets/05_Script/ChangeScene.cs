using UnityEngine;
using UnityEngine.SceneManagement; // 必須引用這個命名空間

public class _SceneSwitcher : MonoBehaviour
{
    [Header("目標場景名稱")]
    public string targetSceneName = "SceneA";

    // 你可以呼叫這個方法來切換場景
    public void _SwitchToTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("目標場景名稱未設定！");
        }
    }

    // 例如按下按鈕觸發場景切換
    void Update()
    {
       
    }
}
