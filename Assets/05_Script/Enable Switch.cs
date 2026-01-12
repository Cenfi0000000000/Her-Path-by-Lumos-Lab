using UnityEngine;
using System.Collections.Generic;

public class GameObjectSwitchList : MonoBehaviour
{
    [Header("要控制的物件列表")]
    public List<GameObject> objects = new List<GameObject>();

    [Header("初始是否啟用")]
    public bool startEnabled = true;

    // 目前狀態
    private bool isEnabled;

    void Awake()
    {
        isEnabled = startEnabled;
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(isEnabled);
        }
    }

    /// <summary>
    /// 每次呼叫就切換狀態
    /// </summary>
    public void Switch()
    {
        isEnabled = !isEnabled; // 反轉狀態

        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(isEnabled);
        }
    }
}
