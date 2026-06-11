using UnityEngine;

public class WindsurferMover : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("장애물이 이동할 목표 좌표(빈 오브젝트)들을 차례대로 넣으세요.")]
    public Transform[] waypoints; // 이동할 목표 지점들
    public float moveSpeed = 5f;  // 이동 속도
    public float turnSpeed = 5f;  // 회전 속도

    private int currentIndex = 0; // 현재 향하고 있는 목표 지점의 순서

    void Update()
    {
        // 목표 지점이 하나도 없으면 작동하지 않음
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];

        // 1. 목표 좌표를 향해 이동 (등속 이동)
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // 2. 윈드서퍼가 이동하는 방향을 자연스럽게 바라보도록 회전
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 3. 목표 지점에 거의 도달(0.2f 이내)했으면 다음 지점으로 타겟 변경
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentIndex++;

            // 마지막 지점까지 갔다면 다시 첫 번째 지점(0번)으로 루프
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }
        }
    }
}