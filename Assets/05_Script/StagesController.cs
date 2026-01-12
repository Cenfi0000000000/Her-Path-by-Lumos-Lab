using UnityEngine;
using System.Collections.Generic;

public class StageController : MonoBehaviour
{
    [Header("Stage Objects（依順序）")]
    public List<GameObject> stageObjects = new List<GameObject>();

    [Header("目前 Stage 編號（從 1 開始）")]
    [SerializeField]
    private int currentStageIndex = 1;

    // 是否允許再 Add（Add 後需先 Remove）
    private bool canAddStage = true;

    private void Start()
    {
        InitializeStages();
    }

    /// <summary>
    /// 初始化：
    /// 全部 false，只開第一個，Stage = 1
    /// </summary>
    private void InitializeStages()
    {
        for (int i = 0; i < stageObjects.Count; i++)
        {
            if (stageObjects[i] != null)
                stageObjects[i].SetActive(false);
        }

        if (stageObjects.Count > 0 && stageObjects[0] != null)
        {
            stageObjects[0].SetActive(true);
            currentStageIndex = 1;
        }

        canAddStage = true;

        Debug.Log("[StageController] Initialized → Stage 1 active");
    }

    /// <summary>
    /// Add Stage
    /// 例： (true,false,false) → (true,true,false)
    /// 編號 +1，且不可連續呼叫
    /// </summary>
    public void _AddStage()
    {
        if (!canAddStage)
        {
            Debug.Log("[StageController] _AddStage blocked (need Remove first)");
            return;
        }

        int nextIndex = currentStageIndex; // 0-based = currentStageIndex

        if (nextIndex >= stageObjects.Count)
        {
            Debug.Log("[StageController] _AddStage failed (reach max stage)");
            return;
        }

        if (stageObjects[nextIndex] != null)
            stageObjects[nextIndex].SetActive(true);

        currentStageIndex++;
        canAddStage = false;

        Debug.Log($"[StageController] AddStage → Stage {currentStageIndex}");
    }

    /// <summary>
    /// Remove Stage
    /// 例： (true,true,false) → (false,true,false)
    /// 編號不變，並允許再次 Add
    /// </summary>
    public void _RemoveStage()
    {
        if (currentStageIndex <= 1)
        {
            Debug.Log("[StageController] _RemoveStage failed (already at first stage)");
            return;
        }

        int removeIndex = currentStageIndex - 2; // 要關掉前一個

        if (stageObjects[removeIndex] != null)
            stageObjects[removeIndex].SetActive(false);

        canAddStage = true;

        Debug.Log($"[StageController] RemoveStage → Stage {currentStageIndex} (index unchanged)");
    }
}
