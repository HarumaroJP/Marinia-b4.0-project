using UnityEngine;

public class DefaultPier : MonoBehaviour
{
    [SerializeField] private MeshCollider WallCollider;
    void Awake()
    {
        WallCollider = GameObject.FindWithTag("Wall").GetComponent<MeshCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            WallCollider.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            WallCollider.enabled = true;
    }
}
