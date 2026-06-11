using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapHUD : MonoBehaviour
{
    public Transform     boat;
    public RectTransform mapContent;  // 검정 패널 (HUDSetup이 자동 연결)
    public float         viewRange = 300f;  // 화면에 보이는 월드 반경

    readonly List<(Transform t, RectTransform dot)> _dots = new();

    void Start()
    {
        foreach (var groupName in new[] { "Obstacles_Left", "Obstacles_Right" })
        {
            var root = GameObject.Find(groupName);
            if (root == null) continue;
            foreach (Transform child in root.transform)
                SpawnDot(child);
        }
    }

    void SpawnDot(Transform t)
    {
        var obj = new GameObject("MDot");
        obj.transform.SetParent(mapContent, false);
        var r = obj.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(8f, 8f);
        obj.AddComponent<Image>().color = Color.red;
        _dots.Add((t, r));
    }

    void LateUpdate()
    {
        if (!boat || !mapContent) return;

        var sz = mapContent.rect.size;
        float sx = sz.x / (viewRange * 2f);
        float sy = sz.y / (viewRange * 2f);

        foreach (var (t, dot) in _dots)
        {
            var off = t.position - boat.position;
            float px = off.x * sx;
            float py = off.z * sy;
            bool vis = Mathf.Abs(off.x) < viewRange && Mathf.Abs(off.z) < viewRange;
            dot.gameObject.SetActive(vis);
            if (vis) dot.anchoredPosition = new Vector2(px, py);
        }
    }
}
