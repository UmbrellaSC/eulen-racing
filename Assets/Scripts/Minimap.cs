using System;
using UnityEngine;

public class Minimap : MonoBehaviour
{
	/// <summary>
	/// Das Zentrum der wirklichen Karte, also des Teils in dem Straßen liegen
	/// </summary>
	public Vector3 Center;

	/// <summary>
	/// Der Radius des Bereichs in dem Straßen liegen
	/// </summary>
	public Single Radius;
	
	void Start () 
	{
		transform.position = new Vector3(Center.x, Radius * 2, Center.z);
	}
}
