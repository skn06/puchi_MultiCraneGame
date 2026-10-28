using UnityEngine;

public class HorizontalBarSidesFollower : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float xOffset;

    private void Start()
    {
        if (target == null) return;

        xOffset = transform.position.x - target.position.x;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 position = transform.position;

        // S/Fで変化するX方向だけ追従する。
        // YとZは変えないので、E/Dの前後移動には追従しない。
        position.x = target.position.x + xOffset;

        transform.position = position;
    }
}