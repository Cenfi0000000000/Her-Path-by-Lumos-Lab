using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("要切換到的 Scene 名稱（需加入 Build Settings）")]
    public string targetSceneName;

    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("SceneSwitcher: targetSceneName 未設定！");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
