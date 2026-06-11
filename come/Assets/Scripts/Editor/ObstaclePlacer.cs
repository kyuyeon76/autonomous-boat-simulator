using UnityEngine;
using UnityEditor;

public class ObstaclePlacer : EditorWindow
{
    [MenuItem("Tools/Place Path Obstacles")]
    static void PlaceObstacles()
    {
        string[] rockPrefabPaths = {
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Medium_01.prefab",
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Medium_02.prefab",
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Medium_03.prefab",
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Medium_04.prefab",
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Small_01.prefab",
            "Assets/Objects/environment/Cliffs/Prefabs/Rock_Small_02.prefab",
        };

        var leftParent  = new GameObject("Obstacles_Left");
        var rightParent = new GameObject("Obstacles_Right");

        Random.InitState(42);

        SpawnRocks(10, rockPrefabPaths, leftParent);
        SpawnRocks( 5, rockPrefabPaths, rightParent);

        Undo.RegisterCreatedObjectUndo(leftParent,  "Place Left Obstacles");
        Undo.RegisterCreatedObjectUndo(rightParent, "Place Right Obstacles");

        EditorUtility.DisplayDialog("완료",
            "Obstacles_Left: 10개\nObstacles_Right: 5개\n\n" +
            "Hierarchy에서 각 오브젝트 안의 바위들을\n항로에 맞게 직접 배치해주세요!", "확인");
    }

    static void SpawnRocks(int count, string[] prefabPaths, GameObject parent)
    {
        for (int i = 0; i < count; i++)
        {
            string path = prefabPaths[Random.Range(0, prefabPaths.Length)];
            var prefab  = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            // 원점 근처에 일렬로 생성 (나중에 직접 이동)
            obj.transform.position   = new Vector3(i * 3f, 0f, 0f);
            obj.transform.rotation   = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            obj.transform.localScale = Vector3.one * Random.Range(0.3f, 0.6f);
            obj.transform.SetParent(parent.transform);
            EditorUtility.SetDirty(obj);
        }
    }
}
