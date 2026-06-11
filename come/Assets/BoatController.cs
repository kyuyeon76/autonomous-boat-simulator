using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatControls : MonoBehaviour
{
    private Rigidbody RB;
    public float speed = 15f;
    public float turnSpeed = 10000f; // 회전 속도를 인스펙터에서 조절할 수 있게 분리

    // 입력을 저장할 변수들
    private bool isMovingForward;
    private bool isMovingBackward;
    private bool isTurningRight;
    private bool isTurningLeft;

    void Start()
    {
        RB = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
    }

    // 키보드 입력은 Update에서 감지
    void Update()
    {
        isMovingForward = Input.GetKey(KeyCode.W);
        isMovingBackward = Input.GetKey(KeyCode.S);
        isTurningRight = Input.GetKey(KeyCode.D);
        isTurningLeft = Input.GetKey(KeyCode.A);
    }

    // 물리적인 움직임은 FixedUpdate에서 적용
    void FixedUpdate()
    {
        if (isMovingForward)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            RB.AddForce(forward * speed, ForceMode.Acceleration);
            RB.AddRelativeTorque(-Vector3.right * 1f, ForceMode.Acceleration);
        }

        if (isMovingBackward)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            RB.AddForce(forward * -speed, ForceMode.Acceleration);
            RB.AddRelativeTorque(-Vector3.right * 1f, ForceMode.Acceleration);
        }

        if (isTurningRight)
        {
            RB.AddTorque(transform.up * turnSpeed);
        }

        if (isTurningLeft)
        {
            RB.AddTorque(transform.up * -turnSpeed);
        }
    }
}