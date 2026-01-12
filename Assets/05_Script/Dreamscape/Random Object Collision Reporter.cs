using UnityEngine;

public class ObjectACollisionReporter : MonoBehaviour
{
    [HideInInspector]
    public RandomObjectASpawner spawner;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.gameObject == spawner.targetFigure)
        {
            hasTriggered = true;

            Debug.Log($"[ObjectA] 🟢 Triggered by Figure: {gameObject.name}");

            spawner.OnObjectAHitFromChild(gameObject);
        }
    }
}
