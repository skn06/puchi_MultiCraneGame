using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ClawDescentBodySensor : MonoBehaviour
{
    [SerializeField] private ClawDescentContactManager contactManager;

    private readonly HashSet<Collider> touchingTargets =
        new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (contactManager == null) return;
        if (!contactManager.IsStopTarget(other)) return;

        if (touchingTargets.Add(other))
        {
            contactManager.RegisterBodyContact(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (contactManager == null) return;

        if (touchingTargets.Remove(other))
        {
            contactManager.UnregisterBodyContact(other);
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
                    contactManager.UnregisterBodyContact(target);
                }
            }
        }

        touchingTargets.Clear();
    }
}