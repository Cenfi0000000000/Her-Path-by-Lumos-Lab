using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ARObjectButton : MonoBehaviour
{
    [Header("按下時要觸發的事件清單")]
    public List<UnityEvent> onPressedEvents = new List<UnityEvent>();

    public void OnPressed()
    {
        Debug.Log("3D Object Button Pressed: " + gameObject.name);

        foreach (var evt in onPressedEvents)
        {
            if (evt != null)
                evt.Invoke();
        }
    }
}
