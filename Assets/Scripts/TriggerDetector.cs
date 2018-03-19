using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    /// <summary>
    /// Der Funktionszeiger der beim Eintreten eines Objektes in den Collider aufgerufen wird
    /// </summary>
    public Action<Collider> Callback;

    /// <summary>
    /// Der Collider der für die Positionsbestimmung zuständig ist.
    /// </summary>
    public Collider Collider;

    void Start()
    {
        if (!Collider.isTrigger)
        {
            Collider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (Callback != null)
        {
            Callback(other);
        }
    }
}
