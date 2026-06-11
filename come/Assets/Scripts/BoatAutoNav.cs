using UnityEngine;

// 보트에 붙이는 스크립트
// R = 재시작   V = 자율운항 토글 (웨이포인트 순서 방문 → 목적지)
[RequireComponent(typeof(Rigidbody))]
public class BoatAutoNav : MonoBehaviour
{
    [Header("재시작 위치")]
    [SerializeField] Vector3    _savedPos;
    [SerializeField] Quaternion _savedRot;

    [Header("자율운항 - 경유 웨이포인트 (순서대로 배치)")]
    [Tooltip("씬에 빈 오브젝트를 만들어 순서대로 드래그. 모두 방문 후 Destination으로 이동.")]
    public Transform[] waypoints;
    public float waypointRadius = 15f;  // 웨이포인트 도달 판정 반경

    [Header("자율운항 - 이동")]
    public float autoSpeed    = 15f;
    public float turnSpeed    = 4f;
    public float detectDist   = 22f;
    public float arriveRadius = 8f;

    [Header("충돌 쿨타임")]
    [Tooltip("충돌 후 다음 충돌 판정까지 대기 시간 (초)")]
    public float collisionCooldown = 2f;

    static readonly float[] RayAngles = { 0f, 20f, -20f, 45f, -45f, 70f, -70f, 90f, -90f };

    Rigidbody _rb;
    Transform _dest;
    BoatHUD   _hud;
    bool      _autoMode;
    LayerMask _mask;
    float     _lastHitTime = -99f;
    int       _wpIndex;   // 현재 목표 웨이포인트 인덱스

    // ── 현재 목표 지점 ───────────────────────────────
    Transform CurrentTarget =>
        (waypoints != null && _wpIndex < waypoints.Length)
            ? waypoints[_wpIndex]
            : _dest;

    bool IsHeadingToWaypoint =>
        waypoints != null && _wpIndex < waypoints.Length;

    void Awake()
    {
        _rb   = GetComponent<Rigidbody>();
        _mask = ~(1 << gameObject.layer);
        _savedPos = transform.position;
        _savedRot = transform.rotation;
        Debug.Log($"[BoatAutoNav] 스폰 위치 저장: {_savedPos}");
    }

    void Start()
    {
        var dest = FindObjectOfType<DestinationTrigger>();
        if (dest) _dest = dest.transform;
        _hud = FindObjectOfType<BoatHUD>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.impulse.magnitude > 1f && Time.time - _lastHitTime > collisionCooldown)
        {
            _lastHitTime = Time.time;
            _hud?.OnObstacleHit();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Restart();
        if (Input.GetKeyDown(KeyCode.V)) ToggleAuto();

        if (_autoMode &&
            (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
             Mathf.Abs(Input.GetAxisRaw("Vertical"))   > 0.1f))
            _autoMode = false;
    }

    void FixedUpdate()
    {
        if (!_autoMode) return;
        if (CurrentTarget == null) return;
        AutoDrive();
    }

    void ToggleAuto()
    {
        _autoMode = !_autoMode;
        if (_autoMode)
        {
            _wpIndex = 0;
            int total = waypoints?.Length ?? 0;
            Debug.Log($"[자율운항] ON  웨이포인트 {total}개 → 목적지 순서로 이동");
        }
        else
        {
            Debug.Log("[자율운항] OFF");
        }
    }

    void Restart()
    {
        _autoMode = false;
        _wpIndex  = 0;
        _rb.velocity        = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.position = _savedPos;
        _rb.rotation = _savedRot;
        Debug.Log($"[BoatAutoNav] 재시작 → {_savedPos}");
        Simulation_Manager.Instance?.ResetGame();
        _hud?.ResetFuel();
    }

    void AutoDrive()
    {
        Transform target  = CurrentTarget;
        Vector3   toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float threshold = IsHeadingToWaypoint ? waypointRadius : arriveRadius;

        // 목표 도달 판정
        if (toTarget.magnitude < threshold)
        {
            if (IsHeadingToWaypoint)
            {
                Debug.Log($"[자율운항] WP {_wpIndex + 1}/{waypoints.Length} 도달");
                _wpIndex++;
            }
            return;
        }

        // 장애물 회피 레이캐스트
        float steerBias  = 0f;
        bool  frontBlock = false;

        foreach (float a in RayAngles)
        {
            Vector3 dir = Quaternion.Euler(0f, a, 0f) * transform.forward;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, detectDist, _mask))
            {
                float weight = 1f - (hit.distance / detectDist);
                if (Mathf.Abs(a) < 1f) frontBlock = true;
                else steerBias -= Mathf.Sign(a) * (1f + weight * 2f);
            }
        }
        if (frontBlock && Mathf.Abs(steerBias) < 0.1f) steerBias = 1f;

        float targetAngle = Vector3.SignedAngle(transform.forward, toTarget.normalized, Vector3.up);
        float steer       = Mathf.Clamp(targetAngle * 0.05f + steerBias, -1f, 1f);

        _rb.AddTorque(Vector3.up * steer * turnSpeed, ForceMode.Acceleration);
        _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, 5f * Time.fixedDeltaTime);

        _rb.AddForce(transform.forward * autoSpeed, ForceMode.Acceleration);
        Vector3 flat = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (flat.magnitude > autoSpeed)
            _rb.velocity = flat.normalized * autoSpeed + Vector3.up * _rb.velocity.y;
    }

    void OnDrawGizmosSelected()
    {
        // 레이캐스트 방향
        Gizmos.color = Color.cyan;
        foreach (float a in RayAngles)
            Gizmos.DrawRay(transform.position,
                Quaternion.Euler(0f, a, 0f) * transform.forward * detectDist);

        // 웨이포인트 경로
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Vector3 prev = transform.position;
            foreach (var wp in waypoints)
            {
                if (wp == null) continue;
                Gizmos.DrawLine(prev, wp.position);
                Gizmos.DrawWireSphere(wp.position, waypointRadius);
                prev = wp.position;
            }
            if (_dest != null) Gizmos.DrawLine(prev, _dest.position);
        }
        else if (_dest != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _dest.position);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_savedPos, 1.5f);
    }
}
