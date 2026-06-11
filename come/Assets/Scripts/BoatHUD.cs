using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoatHUD : MonoBehaviour
{
    [Header("보트")]
    public Transform boatTransform;
    public Rigidbody boatRb;

    [Header("속도")]
    public TMP_Text speedText;

    [Header("연료")]
    public Image    fuelFill;
    public TMP_Text fuelText;
    public float    maxFuel        = 100f;
    public float    collisionDrain = 5f;   // 충돌 1회당 감소 % (Inspector에서 조정 가능)

    [Header("충돌")]
    public TMP_Text collisionText;

    [Header("나침반")]
    public RectTransform compassNeedle;
    public TMP_Text      headingText;

    float _fuel;
    float _drainPerSec;   // 60초 동안 100% 소진
    int   _collisions;
    bool  _depleted;

    void Start()
    {
        _fuel = maxFuel;
        // Simulation_Manager의 timeLimit에 맞춰 드레인 속도 계산
        float limit = Simulation_Manager.Instance != null ? Simulation_Manager.Instance.timeLimit : 60f;
        _drainPerSec = maxFuel / limit;
        if (collisionText) collisionText.text = "HIT: 0";
    }

    public void ResetFuel()
    {
        _fuel      = maxFuel;
        _depleted  = false;
        _collisions = 0;
        if (fuelText)      fuelText.text     = "100%";
        if (collisionText) collisionText.text = "HIT: 0";
        if (fuelFill)
        {
            var r = fuelFill.rectTransform;
            r.anchorMax = new Vector2(1f, 1f);
            r.anchorMin = new Vector2(0f, 0f);
            r.offsetMin = r.offsetMax = Vector2.zero;
        }
    }

    // 충돌 1회 발생 시 BoatAutoNav에서 호출
    public void OnObstacleHit()
    {
        _collisions++;
        _fuel = Mathf.Max(0f, _fuel - collisionDrain);
        if (collisionText) collisionText.text = $"HIT: {_collisions}";
        RefreshFuelUI();
    }

    void Update()
    {
        if (boatRb == null) return;
        UpdateSpeed();
        UpdateFuel();
        UpdateCompass();
    }

    void UpdateSpeed()
    {
        float kmh = boatRb.velocity.magnitude * 3.6f;
        if (speedText) speedText.text = $"{kmh:F1}\nkm/h";
    }

    void UpdateFuel()
    {
        // 게임 진행 중에만 시간 비례로 감소
        if (Simulation_Manager.Instance != null && Simulation_Manager.Instance.IsRunning)
            _fuel = Mathf.Max(0f, _fuel - _drainPerSec * Time.deltaTime);

        RefreshFuelUI();

        // 연료 소진 → FAIL
        if (_fuel <= 0f && !_depleted)
        {
            _depleted = true;
            Simulation_Manager.Instance?.OnFuelEmpty();
        }
    }

    void RefreshFuelUI()
    {
        if (fuelFill)
        {
            // anchorMax.x 를 줄여서 바 길이를 비례 감소
            var r = fuelFill.rectTransform;
            r.anchorMax = new Vector2(_fuel / maxFuel, 1f);
            r.anchorMin = new Vector2(0f, 0f);
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
        }
        if (fuelText) fuelText.text = $"{Mathf.RoundToInt(_fuel)}%";
    }

    void UpdateCompass()
    {
        if (!boatTransform) return;
        float h = boatTransform.eulerAngles.y;

        if (compassNeedle)
            // 기존 -h에 +45f를 추가하여 반시계 방향으로 45도 회전시킵니다.
            compassNeedle.localEulerAngles = new Vector3(0f, 0f, -h + 45f);

        if (headingText)
        {
            string[] d = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            headingText.text = $"{Mathf.RoundToInt(h) % 360:000}°  {d[Mathf.RoundToInt(h / 45f) % 8]}";
        }
    }
}
