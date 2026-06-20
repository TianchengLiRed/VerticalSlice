using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [Header("Collect Settings")]
    [SerializeField] protected int amount = 1;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPick();
        }

        OnCollect();

        Destroy(gameObject);
    }
    protected abstract void OnCollect();
}
