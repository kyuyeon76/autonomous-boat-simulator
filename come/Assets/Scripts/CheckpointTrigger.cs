using UnityEngine;

// 체크포인트 링 오브젝트에 붙이는 스크립트
public class CheckpointTrigger : MonoBehaviour
{
    public int level = 1; // Inspector에서 1 또는 2로 설정

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Simulation_Manager.Instance?.OnCheckpointPassed(level);
    }
}
