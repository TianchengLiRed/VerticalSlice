using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [Header("Collect Settings")]
    [SerializeField] protected int amount = 1;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        OnCollect();

        Destroy(gameObject);
    }
    protected abstract void OnCollect();
}
