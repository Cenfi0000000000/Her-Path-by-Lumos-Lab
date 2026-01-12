using UnityEngine;
using System.Collections.Generic;

public class ElevatorMaterialToggle : MonoBehaviour
{
    [Header("要控制的根物件")]
    public GameObject targetRoot;

    private Renderer[] renderers;
    private Dictionary<Renderer, Color[]> originalColors =
        new Dictionary<Renderer, Color[]>();

    private bool isDark = false;

    void Awake()
    {
        if (targetRoot == null)
            targetRoot = gameObject;

        renderers = targetRoot.GetComponentsInChildren<Renderer>(true);

        // 記錄每個 Renderer 的原始顏色
        foreach (var r in renderers)
        {
            Material[] mats = r.materials;
            Color[] colors = new Color[mats.Length];

            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].HasProperty("_Color"))
                    colors[i] = mats[i].color;
                else
                    colors[i] = Color.white;
            }

            originalColors[r] = colors;
        }
    }

    public void ChangeElevator()
    {
        isDark = !isDark;

        foreach (var r in renderers)
        {
            Material[] mats = r.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                if (!mats[i].HasProperty("_Color"))
                    continue;

                mats[i].color = isDark
                    ? Color.black
                    : originalColors[r][i];
            }
        }
    }
}
