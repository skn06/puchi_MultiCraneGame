using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ClawDescentContactSensor : MonoBehaviour
{
    [SerializeField] private ClawDescentContactManager contactManager;

    [Tooltip("このセンサーが属する爪番号")]
    [SerializeField, Range(1, 3)] private int fingerNumber = 1;

    private readonly HashSet<Collider> touchingTargets =
        new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (contactManager == null) return;
        if (!contactManager.IsStopTarget(other)) return;

        if (touchingTargets.Add(other))
        {
            contactManager.RegisterContact(
                fingerNumber,
                other
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (contactManager == null) return;

        if (touchingTargets.Remove(other))
        {
            contactManager.UnregisterContact(
                fingerNumber,
                other
            );
        }
    }

    private void OnDisable()
    {
        if (contactManager != null)
        {
            foreach (Collider target in touchingTargets)
            {
                if (target != null)
                {
                    contactManager.UnregisterContact(
                        fingerNumber,
                        target
                    );
                }
            }
        }

        touchingTargets.Clear();
    }
}