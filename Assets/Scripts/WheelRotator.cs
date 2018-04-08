using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotiert die Räder des Autos, basierend auf der Geschwindigkeit
/// </summary>
public class WheelRotator : MonoBehaviour
{
	/// <summary>
	/// Der WheelCollider der die Rotation des Rads bestimmt
	/// </summary>
    public WheelCollider Collider;

	/// <summary>
	/// Der Rigidbody des Autos
	/// </summary>
	public Rigidbody CarRigidbody;
	
	// Update is called once per frame
	void Update ()
	{
	    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, (Collider.steerAngle * 0.5f) + 90,
	        transform.localEulerAngles.z + 360 * (CarRigidbody.velocity.magnitude / (Mathf.PI * 2 * Collider.radius)) * Time.deltaTime);
	}
}
