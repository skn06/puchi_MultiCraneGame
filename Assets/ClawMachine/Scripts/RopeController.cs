using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeController : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    private void LateUpdate()
    {
        if (topPoint == null || bottomPoint == null) return;

        lineRenderer.SetPosition(0, topPoint.position);
        lineRenderer.SetPosition(1, bottomPoint.position);
    }
}