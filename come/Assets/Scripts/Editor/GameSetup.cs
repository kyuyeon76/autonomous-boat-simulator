using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class GameSetup : EditorWindow
{
    [MenuItem("Tools/Setup Simulation Manager")]
    static void Setup()
    {
        // ── Simulation_Manager 오브젝트 ───────────────
        var gmObj = new GameObject("Simulation_Manager");
        var gm    = gmObj.AddComponent<Simulation_Manager>();
        Undo.RegisterCreatedObjectUndo(gmObj, "Setup Game");

        // ── Canvas ────────────────────────────────────
        var canvasObj = new GameObject("SimulationUI");
        var canvas    = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(canvasObj, "Setup Game");

        // ── UI 패널 생성 ──────────────────────────────
        var titlePanel  = MakeTitlePanel(canvasObj, "TitleUI");
        var startPanel  = MakePanel(canvasObj, "StartUI",  "START",  new Color(1f, 0.85f, 0f));
        var timerPanel  = MakeTimerPanel(canvasObj, "TimerUI", out TMP_Text timerText, out TMP_Text levelText);
        var finishPanel = MakePanel(canvasObj, "FinishUI", "FINISH", new Color(0.2f, 1f, 0.3f));
        var failPanel   = MakePanel(canvasObj, "FailUI",   "FAIL",   new Color(1f, 0.2f, 0.2f));

        // 초기 상태: 타이틀만 활성, 나머지 숨김
        titlePanel.SetActive(true);
        startPanel.SetActive(false);
        timerPanel.SetActive(false);
        finishPanel.SetActive(false);
        failPanel.SetActive(false);

        // ── Simulation_Manager에 UI 연결 ──────────────
        gm.titleUI   = titlePanel;
        gm.startUI   = startPanel;
        gm.timerUI   = timerPanel;
        gm.timerText = timerText;
        gm.levelText = levelText;
        gm.finishUI  = finishPanel;
        gm.failUI    = failPanel;
        EditorUtility.SetDirty(gm);

        // ── 목적지 오브젝트 ───────────────────────────
        var destObj = new GameObject("Destination");
        destObj.transform.position = new Vector3(0f, 0f, 5f); // 기본 위치 - 직접 이동 필요
        var col = destObj.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = 5f;
        destObj.AddComponent<DestinationTrigger>();
        Undo.RegisterCreatedObjectUndo(destObj, "Setup Game");

        EditorUtility.DisplayDialog("완료",
            "Simulation_Manager, UI, Destination 생성 완료!\n\n" +
            "남은 작업:\n" +
            "1. 보트 오브젝트 Tag를 'Player'로 설정\n" +
            "2. 체크포인트 오브젝트에 CheckpointTrigger 스크립트 추가\n" +
            "3. 씬에서 'Destination' 오브젝트를 원하는 목적지로 이동", "확인");
    }

    static GameObject MakeTitlePanel(GameObject canvas, string name)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);

        var rect       = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 반투명 배경
        var bg = panel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0f, 0.05f, 0.15f, 0.82f);

        // 타이틀 텍스트 (상단)
        var titleObj  = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        var tRect     = titleObj.AddComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0f, 0.52f);
        tRect.anchorMax = new Vector2(1f, 0.80f);
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;
        var tTmp        = titleObj.AddComponent<TextMeshProUGUI>();
        tTmp.text       = "BOAT RACE";
        tTmp.fontSize   = 110;
        tTmp.fontStyle  = FontStyles.Bold;
        tTmp.color      = new Color(0.2f, 0.85f, 1f);
        tTmp.alignment  = TextAlignmentOptions.Center;

        // PRESS SPACE 텍스트 (하단, 깜빡임 없이 항상 표시)
        var promptObj  = new GameObject("PressSpaceText");
        promptObj.transform.SetParent(panel.transform, false);
        var pRect      = promptObj.AddComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0f, 0.35f);
        pRect.anchorMax = new Vector2(1f, 0.50f);
        pRect.offsetMin = Vector2.zero;
        pRect.offsetMax = Vector2.zero;
        var pTmp        = promptObj.AddComponent<TextMeshProUGUI>();
        pTmp.text       = "PRESS  SPACE  TO  START";
        pTmp.fontSize   = 36;
        pTmp.color      = Color.white;
        pTmp.alignment  = TextAlignmentOptions.Center;

        return panel;
    }

    static GameObject MakePanel(GameObject canvas, string name, string text, Color color)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);

        var rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var tmp        = panel.AddComponent<TextMeshProUGUI>();
        tmp.text       = text;
        tmp.fontSize   = 120;
        tmp.fontStyle  = FontStyles.Bold;
        tmp.color      = color;
        tmp.alignment  = TextAlignmentOptions.Center;

        return panel;
    }

    static GameObject MakeTimerPanel(GameObject canvas, string name, out TMP_Text timerText, out TMP_Text levelText)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);

        var panelRect       = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.8f);
        panelRect.anchorMax = new Vector2(0.7f, 0.95f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 타이머 숫자 (오른쪽)
        var timerObj  = new GameObject("TimerText");
        timerObj.transform.SetParent(panel.transform, false);
        var timerRect = timerObj.AddComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.5f, 0f);
        timerRect.anchorMax = new Vector2(1f,   1f);
        timerRect.offsetMin = Vector2.zero;
        timerRect.offsetMax = Vector2.zero;
        var timerTmp        = timerObj.AddComponent<TextMeshProUGUI>();
        timerTmp.text       = "60";
        timerTmp.fontSize   = 80;
        timerTmp.fontStyle  = FontStyles.Bold;
        timerTmp.color      = Color.white;
        timerTmp.alignment  = TextAlignmentOptions.Center;
        timerText           = timerTmp;

        // Level 텍스트 (왼쪽)
        var levelObj  = new GameObject("LevelText");
        levelObj.transform.SetParent(panel.transform, false);
        var levelRect = levelObj.AddComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0f,  0f);
        levelRect.anchorMax = new Vector2(0.5f, 1f);
        levelRect.offsetMin = Vector2.zero;
        levelRect.offsetMax = Vector2.zero;
        var levelTmp        = levelObj.AddComponent<TextMeshProUGUI>();
        levelTmp.text       = "";
        levelTmp.fontSize   = 28;
        levelTmp.fontStyle  = FontStyles.Bold;
        levelTmp.color      = Color.yellow;
        levelTmp.alignment  = TextAlignmentOptions.Center;
        levelText           = levelTmp;

        return panel;
    }
}
