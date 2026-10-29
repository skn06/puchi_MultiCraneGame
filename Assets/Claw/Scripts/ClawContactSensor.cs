using System.Collections.Generic;
using UnityEngine;

public class ClawContactSensor : MonoBehaviour
{
    private readonly HashSet<Rigidbody> touchingPrizes
        = new HashSet<Rigidbody>();

    public bool IsTouchingPrize => touchingPrizes.Count > 0;

    // 現在触れている景品を1つ取得する。
    public Rigidbody AnyTouchingPrize
    {
        get
        {
            foreach (Rigidbody prize in touchingPrizes)
            {
                if (prize != null)
                {
                    return prize;
                }
            }

            return null;
        }
    }

    // 指定した景品にこの爪が触れているかを返す。
    public bool IsTouching(Rigidbody prize)
    {
        return prize != null && touchingPrizes.Contains(prize);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody prize = collision.rigidbody;

        if (prize != null && prize.CompareTag("Prize"))
        {
            touchingPrizes.Add(prize);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody prize = collision.rigidbody;

        if (prize != null)
        {
            touchingPrizes.Remove(prize);
        }
    }

    private void OnDisable()
    {
        touchingPrizes.Clear();
    }
}