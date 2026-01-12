using UnityEngine;

public class LeafTriggerReporter : MonoBehaviour
{
    [HideInInspector] public LeafSpawnerWithCollision spawner;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == spawner.objectA)
        {
            spawner.OnLeafHit(gameObject);
        }
    }
}
