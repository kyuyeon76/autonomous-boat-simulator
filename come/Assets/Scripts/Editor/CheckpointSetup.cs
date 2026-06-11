using UnityEngine;
using UnityEditor;

public class CheckpointSetup : EditorWindow
{
    [MenuItem("Tools/Setup Checkpoints")]
    static void Setup()
    {
        string[] names = { "checkpoint", "checkpoint (1)" };
        int done = 0;

        foreach (var name in names)
        {
            var obj = GameObject.Find(name);
            if (obj == null) { Debug.LogWarning($"{name} 오브젝트를 찾지 못했어요."); continue; }

            // 이미 Collider 있으면 스킵
            if (obj.GetComponent<Collider>() != null)
            {
                Debug.Log($"{name}: 이미 Collider 있음, 스킵");
                continue;
            }

            // BoxCollider 추가 (링 통과 범위)
            var col     = obj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size    = new Vector3(12f, 8f, 2f); // 링 크기에 맞게 조정 가능
            col.center  = Vector3.zero;

            // CheckpointTrigger 없으면 추가
            if (obj.GetComponent<CheckpointTrigger>() == null)
                obj.AddComponent<CheckpointTrigger>();

            EditorUtility.SetDirty(obj);
            done++;
            Debug.Log($"{name}: BoxCollider(Trigger) + CheckpointTrigger 추가 완료");
        }

        EditorUtility.DisplayDialog("완료",
            $"{done}개 체크포인트에 Collider 추가 완료!\n\n" +
            "체크포인트 선택 후 씬 뷰에서 초록 박스 크기가\n링과 맞는지 확인해주세요.\n" +
            "(Inspector → Box Collider → Size 조정 가능)", "확인");
    }
}
