using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DelayEventCaller : MonoBehaviour
{
    [Header("Delay Settings")]
    [Tooltip("©µ¿ð¬í¼Æ")]
    public float delaySeconds = 1f;

    [Header("Event to Invoke After Delay")]
    public UnityEvent onDelayFinished;

    /// <summary>
    /// ©I¥s«á¡A©µ¿ð delaySeconds ¬í¦A°õ¦æ onDelayFinished
    /// </summary>
    public void DelayCall()
    {
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(delaySeconds);
        onDelayFinished?.Invoke();
    }
}
