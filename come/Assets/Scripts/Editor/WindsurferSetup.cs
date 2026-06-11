using UnityEngine;
using UnityEditor;

public class WindsurferSetup : EditorWindow
{
    [MenuItem("Tools/Setup Windsurfer Colliders")]
    static void Setup()
    {
        // WindsurferMover가 붙어있는 모든 오브젝트에 BoxCollider + 키네마틱 Rigidbody 추가
        var movers = FindObjectsOfType<WindsurferMover>();

        if (movers.Length == 0)
        {
            // WindsurferMover가 없으면 이름으로 탐색
            var all = FindObjectsOfType<GameObject>();
            int count = 0;
            foreach (var go in all)
            {
                if (go.name.Contains("Surfer") || go.name.Contains("Windsurfer"))
                {
                    AddCollidersTo(go);
                    count++;
                }
            }
            EditorUtility.DisplayDialog("완료", $"이름 기반 탐색: {count}개 오브젝트에 콜라이더 추가", "확인");
            return;
        }

        foreach (var mover in movers)
            AddCollidersTo(mover.gameObject);

        EditorUtility.DisplayDialog("완료",
            $"{movers.Length}개 Windsurfer에 BoxCollider + Rigidbody(isKinematic) 추가 완료\n\n" +
            "씬을 저장하세요 (Ctrl+S)", "확인");
    }

    static void AddCollidersTo(GameObject go)
    {
        // Rigidbody
        var rb = go.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = Undo.AddComponent<Rigidbody>(go);
            rb.isKinematic = true;
            rb.useGravity  = false;
        }
        else
        {
            rb.isKinematic = true;
            rb.useGravity  = false;
        }

        // BoxCollider
        if (go.GetComponent<BoxCollider>() == null)
        {
            var col = Undo.AddComponent<BoxCollider>(go);
            col.center = new Vector3(0f, 1.5f, 0f);   // 수면 위 중심
            col.size   = new Vector3(1.5f, 3f, 1.5f); // 윈드서퍼 대략 크기
        }

        EditorUtility.SetDirty(go);
    }
}
