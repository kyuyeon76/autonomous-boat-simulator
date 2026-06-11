using System.Collections;
using UnityEngine;
using TMPro;

public class Simulation_Manager : MonoBehaviour
{
    public static Simulation_Manager Instance { get; private set; }

    [Header("UI")]
    public GameObject titleUI;
    public GameObject startUI;
    public GameObject timerUI;
    public TMP_Text   timerText;
    public TMP_Text   levelText;
    public GameObject finishUI;
    public GameObject failUI;

    [Header("설정")]
    public float timeLimit = 60f;

    float _timer;
    bool  _running;
    bool  _titleShown;

    public bool IsRunning => _running;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowTitle();
    }

    void ShowTitle()
    {
        _titleShown = true;
        if (titleUI)  titleUI.SetActive(true);
        if (startUI)  startUI.SetActive(false);
        if (timerUI)  timerUI.SetActive(false);
        if (finishUI) finishUI.SetActive(false);
        if (failUI)   failUI.SetActive(false);
    }

    void Update()
    {
        // 타이틀 화면 — Space로 게임 대기 상태 진입
        if (_titleShown)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _titleShown = false;
                if (titleUI) titleUI.SetActive(false);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.T)) OnCheckpointPassed(1);
        if (Input.GetKeyDown(KeyCode.F)) OnDestinationReached();

        if (!_running) return;

        _timer -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(Mathf.Max(_timer, 0f)).ToString();

        if (_timer <= 0f)
        {
            _running = false;
            StartCoroutine(ShowPanel(failUI));
        }
    }

    // 체크포인트 통과 시 호출 (level: 1 or 2)
    public void OnCheckpointPassed(int level)
    {
        if (_running) return;
        _timer = timeLimit;
        _running = true;
        if (levelText != null) levelText.text = "Level." + level;
        StartCoroutine(ShowStartThenTimer());
    }

    // 연료 소진 시 호출
    public void OnFuelEmpty()
    {
        if (!_running) return;
        _running = false;
        StartCoroutine(ShowPanel(failUI));
    }

    // R키 재시작 시 호출
    public void ResetGame()
    {
        _running    = false;
        _titleShown = false;
        _timer      = timeLimit;
        if (titleUI)  titleUI.SetActive(false);
        if (startUI)  startUI.SetActive(false);
        if (timerUI)  timerUI.SetActive(false);
        if (finishUI) finishUI.SetActive(false);
        if (failUI)   failUI.SetActive(false);
        if (levelText != null) levelText.text = "";
    }

    // 목적지 도착 시 호출
    public void OnDestinationReached()
    {
        if (!_running) return;
        _running = false;
        timerUI.SetActive(false);
        StartCoroutine(ShowPanel(finishUI));
    }

    IEnumerator ShowStartThenTimer()
    {
        startUI.SetActive(true);
        timerUI.SetActive(false);
        yield return new WaitForSeconds(2f);
        startUI.SetActive(false);
        timerUI.SetActive(true);
    }

    IEnumerator ShowPanel(GameObject panel)
    {
        timerUI.SetActive(false);
        panel.SetActive(true);
        yield return null;
    }
}
