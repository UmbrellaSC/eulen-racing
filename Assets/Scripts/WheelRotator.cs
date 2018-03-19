using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    public WheelCollider Collider;
	
	// Update is called once per frame
	void Update ()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Collider.steerAngle + 90, (360 * (Collider.rpm / 60)) * Time.deltaTime);
    }
}
