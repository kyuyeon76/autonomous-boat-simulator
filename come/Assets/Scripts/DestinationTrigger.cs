using UnityEngine;

// 목적지 오브젝트에 붙이는 스크립트
public class DestinationTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Simulation_Manager.Instance?.OnDestinationReached();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>()?.radius ?? 5f);
    }
}
