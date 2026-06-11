using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;
    public float     height = 200f;

    void LateUpdate()
    {
        if (!target) return;
        var p = target.position;
        transform.position = new Vector3(p.x, p.y + height, p.z);
    }
}
