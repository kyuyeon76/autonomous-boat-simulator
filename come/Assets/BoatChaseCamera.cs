using UnityEngine;

public class BoatChaseCamera : MonoBehaviour
{
    public Transform target;          // 따라갈 배
    public float distance = 7.0f;     // 배 뒤로 얼마나 떨어질지
    public float height = 3.0f;       // 배 위로 얼마나 높이 띄울지
    public float positionDamping = 3.0f; // 위치를 따라가는 부드러운 정도
    public float rotationDamping = 2.0f; // 회전을 따라가는 부드러운 정도

    // 카메라는 항상 타겟의 이동이 끝난 후(LateUpdate)에 움직여야 덜덜 떨리지 않습니다.
    void LateUpdate()
    {
        if (target == null) return;

        // 1. 타겟(배)의 현재 좌우 회전각(Y축)만 가져옵니다. (위아래 기울기는 무시)
        float wantedRotationAngle = target.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;

        // 2. 카메라의 현재 회전각에서 타겟의 회전각으로 부드럽게(LerpAngle) 변환합니다.
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // 3. 계산된 각도를 바탕으로 카메라가 바라볼 새로운 회전 방향을 만듭니다.
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // 4. 타겟의 위치에서 방금 계산한 회전 방향을 기준으로 distance만큼 뒤로 뺍니다.
        Vector3 desiredPosition = target.position - (currentRotation * Vector3.forward * distance);

        // 5. 카메라의 높이를 타겟보다 height만큼 위로 설정합니다.
        desiredPosition.y = target.position.y + height;

        // 6. 카메라의 실제 위치를 계산된 목표 위치로 부드럽게(Lerp) 이동시킵니다.
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);

        // 7. 카메라는 항상 타겟을 쳐다보게 합니다. (이 함수가 지평선을 자동으로 수평으로 잡아줍니다)
        transform.LookAt(target);
    }
}
