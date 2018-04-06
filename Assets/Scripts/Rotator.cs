using System;
using UnityEngine;

/// <summary>
/// Rotiert ein Objekt um ein anderes Objekt
/// </summary>
public class Rotator : MonoBehaviour
{
	/// <summary>
	/// Wie lange dauert eine Umdrehung?
	/// </summary>
	public Single Seconds;
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(transform.up, (360f / Seconds) * Time.deltaTime);
	}
}
