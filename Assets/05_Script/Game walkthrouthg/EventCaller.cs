using UnityEngine;
using UnityEngine.Events;

public class ImmediateEventCaller : MonoBehaviour
{
    [Header("在腳本啟用當下立即呼叫的 Events")]
    public UnityEvent onImmediateCall;

    private void OnEnable()
    {
        onImmediateCall?.Invoke();
    }
}
