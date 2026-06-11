using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HUDSetup : EditorWindow
{
    [MenuItem("Tools/Setup HUD")]
    static void Setup()
    {
        // ── Canvas ────────────────────────────────────
        var cvObj = new GameObject("HUD");
        var cv    = cvObj.AddComponent<Canvas>();
        cv.renderMode   = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 5;
        var scaler = cvObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        cvObj.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(cvObj, "HUD");

        // ── BoatHUD / MinimapHUD 컴포넌트 ────────────
        var hudObj = new GameObject("BoatHUD");
        var hud    = hudObj.AddComponent<BoatHUD>();
        var mmHud  = hudObj.AddComponent<MinimapHUD>();
        Undo.RegisterCreatedObjectUndo(hudObj, "HUD");

        BuildSpeedFuel(cvObj, hud);
        BuildMinimap(cvObj, mmHud);
        BuildCompass(cvObj, hud);

        EditorUtility.SetDirty(hudObj);
        EditorUtility.DisplayDialog("HUD 생성 완료",
            "남은 작업:\n" +
            "1. BoatHUD 오브젝트 → Boat Transform / Boat Rb → 보트 드래그\n" +
            "2. BoatHUD 오브젝트 → MinimapHUD → Boat → 보트 드래그\n" +
            "3. Ctrl+S 씬 저장", "확인");
    }

    // ── 속도 + 연료 (오른쪽 상단) ───────────────────
    static void BuildSpeedFuel(GameObject cv, BoatHUD hud)
    {
        var panel = Panel("SpeedFuelPanel", cv.transform,
            new Vector2(0.78f, 0.78f), new Vector2(1f, 1f));

        var spdObj = Child("SpeedText", panel.transform,
            new Vector2(0f, 0.45f), Vector2.one);
        var spdTmp = spdObj.AddComponent<TextMeshProUGUI>();
        spdTmp.text = "0.0\nkm/h"; spdTmp.fontSize = 30;
        spdTmp.fontStyle = FontStyles.Bold; spdTmp.color = Color.white;
        spdTmp.alignment = TextAlignmentOptions.Center;
        hud.speedText = spdTmp;

        var lblObj = Child("FuelLabel", panel.transform,
            new Vector2(0.05f, 0.28f), new Vector2(0.45f, 0.46f));
        var lblTmp = lblObj.AddComponent<TextMeshProUGUI>();
        lblTmp.text = "FUEL"; lblTmp.fontSize = 18;
        lblTmp.color = Color.white; lblTmp.alignment = TextAlignmentOptions.Center;

        var pctObj = Child("FuelPct", panel.transform,
            new Vector2(0.55f, 0.28f), new Vector2(0.95f, 0.46f));
        var pctTmp = pctObj.AddComponent<TextMeshProUGUI>();
        pctTmp.text = "100%"; pctTmp.fontSize = 18;
        pctTmp.color = Color.green; pctTmp.alignment = TextAlignmentOptions.Center;
        hud.fuelText = pctTmp;

        var bgObj = Child("FuelBg", panel.transform,
            new Vector2(0.05f, 0.13f), new Vector2(0.95f, 0.28f));
        bgObj.AddComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

        var fillObj = Child("FuelFill", bgObj.transform, Vector2.zero, Vector2.one);
        var fillImg = fillObj.AddComponent<Image>();
        fillImg.color = Color.green;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;
        hud.fuelFill = fillImg;

        // 충돌 횟수
        var hitObj = Child("CollisionText", panel.transform,
            new Vector2(0f, 0.01f), new Vector2(1f, 0.12f));
        var hitTmp = hitObj.AddComponent<TextMeshProUGUI>();
        hitTmp.text = "HIT: 0"; hitTmp.fontSize = 16;
        hitTmp.color = new Color(1f, 0.5f, 0f); hitTmp.alignment = TextAlignmentOptions.Center;
        hud.collisionText = hitTmp;
    }

    // ── 미니맵 (왼쪽 상단) ──────────────────────────
    static void BuildMinimap(GameObject cv, MinimapHUD mmHud)
    {
        var panel = Panel("MinimapPanel", cv.transform,
            new Vector2(0f, 0.75f), new Vector2(0.19f, 1f));

        var lbl = Child("MapLabel", panel.transform,
            new Vector2(0f, 0.87f), Vector2.one);
        var lTmp = lbl.AddComponent<TextMeshProUGUI>();
        lTmp.text = "MAP"; lTmp.fontSize = 16;
        lTmp.color = Color.white; lTmp.alignment = TextAlignmentOptions.Center;

        // 검정 배경 + 장애물 점 영역
        var mapObj = Child("MinimapContent", panel.transform,
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.85f));
        mapObj.AddComponent<Image>().color = Color.black;
        mapObj.AddComponent<Mask>().showMaskGraphic = true;

        // 보트 위치 표시 (항상 중앙, 흰색)
        var boatDot = new GameObject("BoatDot");
        boatDot.transform.SetParent(mapObj.transform, false);
        var bdRect = boatDot.AddComponent<RectTransform>();
        bdRect.anchorMin = bdRect.anchorMax = new Vector2(0.5f, 0.5f);
        bdRect.sizeDelta = new Vector2(10f, 10f);
        boatDot.AddComponent<Image>().color = Color.white;

        mmHud.mapContent = (RectTransform)mapObj.transform;
    }

    // ── 나침반 (왼쪽 하단) ──────────────────────────
    static void BuildCompass(GameObject cv, BoatHUD hud)
    {
        var panel = Panel("CompassPanel", cv.transform,
            new Vector2(0f, 0f), new Vector2(0.14f, 0.26f));

        var title = Child("CompassTitle", panel.transform,
            new Vector2(0f, 0.84f), Vector2.one);
        var tTmp = title.AddComponent<TextMeshProUGUI>();
        tTmp.text = "COMPASS"; tTmp.fontSize = 13;
        tTmp.color = Color.white; tTmp.alignment = TextAlignmentOptions.Center;

        var ring = Child("Ring", panel.transform,
            new Vector2(0.05f, 0.18f), new Vector2(0.95f, 0.84f));

        // 방위 레이블 (N=빨강, 나머지=흰색)
        var dirs = new (string lbl, float ax, float ay, Color col)[]
        {
            ("N", 0.50f, 0.88f, Color.red),
            ("E", 0.88f, 0.50f, Color.white),
            ("S", 0.50f, 0.12f, Color.white),
            ("W", 0.12f, 0.50f, Color.white),
        };
        foreach (var (lbl, ax, ay, col) in dirs)
        {
            var d = Child(lbl, ring.transform,
                new Vector2(ax - 0.12f, ay - 0.12f),
                new Vector2(ax + 0.12f, ay + 0.12f));
            var tmp = d.AddComponent<TextMeshProUGUI>();
            tmp.text = lbl; tmp.fontSize = 16;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = col; tmp.alignment = TextAlignmentOptions.Center;
        }

        // 바늘 (중앙에서 회전)
        var needleObj  = new GameObject("Needle");
        needleObj.transform.SetParent(ring.transform, false);
        var needleRect = needleObj.AddComponent<RectTransform>();
        needleRect.anchorMin = needleRect.anchorMax = new Vector2(0.5f, 0.5f);
        needleRect.pivot     = new Vector2(0.5f, 0.5f);
        needleRect.sizeDelta = new Vector2(5f, 44f);
        needleObj.AddComponent<Image>().color = new Color(1f, 0.8f, 0f); // 골드
        hud.compassNeedle = needleRect;

        // 현재 각도 텍스트
        var hdg = Child("HeadingText", panel.transform,
            new Vector2(0f, 0.01f), new Vector2(1f, 0.16f));
        var hdgTmp = hdg.AddComponent<TextMeshProUGUI>();
        hdgTmp.text = "000°  N"; hdgTmp.fontSize = 15;
        hdgTmp.color = Color.yellow; hdgTmp.alignment = TextAlignmentOptions.Center;
        hud.headingText = hdgTmp;
    }

    // ── 헬퍼 ─────────────────────────────────────────
    static GameObject Panel(string name, Transform parent, Vector2 ancMin, Vector2 ancMax)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var r = obj.AddComponent<RectTransform>();
        r.anchorMin = ancMin; r.anchorMax = ancMax;
        r.offsetMin = new Vector2(8f, 8f);
        r.offsetMax = new Vector2(-8f, -8f);
        obj.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);
        return obj;
    }

    static GameObject Child(string name, Transform parent, Vector2 ancMin, Vector2 ancMax)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var r = obj.AddComponent<RectTransform>();
        r.anchorMin = ancMin; r.anchorMax = ancMax;
        r.offsetMin = r.offsetMax = Vector2.zero;
        return obj;
    }
}
